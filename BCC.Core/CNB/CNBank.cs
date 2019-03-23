using System;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using System.Linq;
using BCC.Model.Models;

namespace BCC.Core.CNB
{
    public class CNBank : IExchangeRateBank
    {
        
        public CNBank(){

        }
        private readonly string URL_FOR_DAY_TICKET = "http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt";

        private readonly string URL_FOR_YEAR_TICKET = "http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/rok.txt";

        private readonly DateTime MIN_DATE = new DateTime(year: 1992, month: 1, day: 1);
        
     
        #region Helper methods
        private void ValidateDate(DateTime date) {
            if (date < this.MIN_DATE || date > DateTime.Now)
            {
                new CNBInvalidDate($"Invalid date. Minimum is {this.MIN_DATE}. Maxium is {DateTime.Now}");
            }
        }
        private string DownloadTicketText(string url)
        {
            string responseText = null;
            using (WebClient webClient = new WebClient())
            {
                
                responseText = webClient.DownloadString(url);
            }
            return responseText;
        }

        public List<ExchangeRateTicket> DownloadYearTicket(DateTime date)
        {
            this.ValidateDate(date);
            string dateString = date.ToString("yyyy");
            string url = $"{this.URL_FOR_YEAR_TICKET}?rok={dateString}";
            string text = this.DownloadTicketText(url);
            List<ExchangeRateTicket> tickets = this.ParseYearTicket(text);
            return tickets;
        }
        #endregion

        #region Interface methods

        public bool TodaysTicketIsAvailable()
        {
            //TODO: MM could cause potential isssue because of different time on server
            return (DateTime.Now.Hour > 14); 
        }

        public ExchangeRateTicket DownloadTodaysTicket()
        {
            DateTime date = DateTime.Now;
            this.ValidateDate(date);
            string dateString = date.ToString("dd.MM.yyyy");
            string url = $"{this.URL_FOR_DAY_TICKET}?date={dateString}";

            string text = this.DownloadTicketText(url);
            ExchangeRateTicket ticket = this.ParseDayTicket(text);
            return ticket;
        }

        public ExchangeRateTicket DownloadTicketForDate(DateTime date)
        {
            this.ValidateDate(date);
            var dateString = date.ToString("dd.MM.yyyy");
            string url = $"{this.URL_FOR_DAY_TICKET}?date={dateString}";
            string text = this.DownloadTicketText(url);
            ExchangeRateTicket ticket = this.ParseDayTicket(text);
            return ticket;
        }

        public List<ExchangeRateTicket> DownloadAllTickets()
        {
           return DownloadTicketForInterval(this.MIN_DATE, DateTime.Now);
        }

        public List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end)
        {
            ValidateDate(start);
            ValidateDate(end);
            int[] years = YearsInterval(start, end);
            List<ExchangeRateTicket> tickets = new List<ExchangeRateTicket>(years.Length * 50);
            foreach(int year in years)
            {
                DateTime date = new DateTime(year, 1, 1);
                List<ExchangeRateTicket> yearTickets = DownloadYearTicket(date);
                tickets.AddRange(yearTickets);
            }
            List<ExchangeRateTicket> ticketInterval = tickets.Where(x => x.TicketDate >= start && x.TicketDate <= end).ToList();
            return ticketInterval;
        }
        #endregion

        /// <summary>
        /// Takes two date and return array of years included in both dates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Array containing year in interval</returns>
        public static int[] YearsInterval(DateTime start , DateTime end)
        {
            if (end.Year < start.Year) throw new CNBInvalidDate($"start: {start}, end: {start}");

            int difference = start.Year - end.Year;
            int[] array = new int[difference+1];
            for (int index=0; index < difference + 1;index++)
            {
                array[index] = start.Year + index;
            }
            return array;
        }
        #region DayTicket



        private ExchangeRateTicket ParseDayTicket(string text)
        {
            string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
                throw new CNBInvalidData(message: "Invalid number of lines");

            string[] header = lines.ToList().GetRange(0, 2).ToArray();
            string[] body = lines.ToList().GetRange(2, lines.Length-2).ToArray();

            ExchangeRateTicket ticket = new ExchangeRateTicket();
            this.ParseDayTicketHeader(header, ref ticket);
            this.ParseDayTicketBody(body, ref ticket);

            return ticket;
        }

        private void ParseDayTicketHeader(string[] header, ref ExchangeRateTicket ticket) {
            string[] firstLine = header[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] headers = header[1].Split('|', StringSplitOptions.RemoveEmptyEntries);

            DateTime ticketDate = DateTime.ParseExact(firstLine[0], "dd.MM.yyyy", CultureInfo.CurrentCulture);
            ticket.TicketDate = ticketDate;
        }

        private void ParseDayTicketBody(string[] body,ref ExchangeRateTicket ticket)
        {
            foreach(string line in body)
            {
                string[] sections = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                string country = sections[0];
                string name = sections[1];
                int quantity = int.Parse(sections[2]);
                string isoName = sections[3];
                float buy = float.Parse(sections[4], CultureInfo.GetCultureInfo("cs-CZ"));

                ICurrencyData data = new ERDataBase(isoName, name,country, quantity, buy, null );
                ticket.AddExchangeRateData(data);
            }
        }


        #endregion

        #region Year ticket
        private List<ExchangeRateTicket> ParseYearTicket(string text){
            string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
                throw new CNBInvalidData(message: "Invalid number of lines");

            string header = lines[0];
            string[] body = lines.ToList().GetRange(1, lines.Length - 1).ToArray();
            
            List<ICurrencyMetada> metadata = this.ParseYearHeader(header);
            List<ExchangeRateTicket> tickets = this.ParseYearBody(body,ref metadata);
            return tickets;
        }
        private List<ICurrencyMetada> ParseYearHeader(string text)
        {
            string[] header = text.Split(new char[] { '|', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<ICurrencyMetada> currencyInfos = new List<ICurrencyMetada>(header.Length - 1);
            for (int i = 1; i < header.Length; i += 2)
            {
                int quntity = int.Parse(header[i]);
                string isoName = header[i + 1];
                currencyInfos.Add(new ERMetadataBase(isoName, null, null, quntity));
            }
            return currencyInfos;
        }

        private List<ExchangeRateTicket> ParseYearBody(string[] body,ref List<ICurrencyMetada> metadata)
        {
            List<ExchangeRateTicket> tickets = new List<ExchangeRateTicket>(body.Length);
            foreach (string line in body)
            {
                string[] sections = line.Split('|');
                DateTime date = DateTime.ParseExact(sections[0], "dd.MM.yyyy", CultureInfo.CurrentCulture);
                ExchangeRateTicket ticket = this.ParseYearExchangeRateData(sections, metadata);
                ticket.TicketDate = date;
                tickets.Add(ticket);
            }
            return tickets;
        }
        
        private ExchangeRateTicket ParseYearExchangeRateData(string[] section, List<ICurrencyMetada> currencyInfos)
        {
            ExchangeRateTicket ticket = new ExchangeRateTicket();
            for (int i = 1; i < section.Length; i++)
            {
                float buy = float.Parse(section[i], CultureInfo.GetCultureInfo("cs-CZ"));
                ICurrencyMetada metada = currencyInfos[i - 1];

                ERDataBase data = new ERDataBase(metada, buy, null);
                ticket.AddExchangeRateData(data);
            }
            return ticket;
        }
        
        #endregion

       
        public List<ICurrencyMetada> DownloadCurrencyMetada()
        {
            ExchangeRateTicket ticket = DownloadTodaysTicket();    
            return ticket.GetExchangeRateData().ToList<ICurrencyMetada>();
        }

       
    } 
}

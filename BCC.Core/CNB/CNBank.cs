using System;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using System.Linq;
using BCC.Model.Models;
using BCC.Core.Abstract;
using Microsoft.Extensions.Logging;

namespace BCC.Core.CNB
{
    public class CNBank : ABank<CNBank>, IExchangeRateBank
    {
        public CNBank(){

        }


        /// <summary>
        /// URL used to downloads ticket for specified date
        /// </summary>
        private readonly string URL_FOR_DAY_TICKET = "http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt";

        /// <summary>
        /// URL used to download tickets for specified year
        /// </summary>
        private readonly string URL_FOR_YEAR_TICKET = "http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/rok.txt";

        /// <summary>
        /// Minimal date for which currency ticket can be downloaded
        /// </summary>
        private readonly DateTime MIN_DATE = new DateTime(year: 1992, month: 1, day: 1);
        
        #region Helper methods

        /// <summary>
        /// Validates date
        /// </summary>
        /// <param name="date"> date to be validated</param>
        private void ValidateDate(DateTime date) {
            if (date < this.MIN_DATE || date > DateTime.Now)
            {
                new CNBInvalidDate($"Invalid date. Minimum is {this.MIN_DATE}. Maxium is {DateTime.Now}");
            }
        }
     
        /// <summary>
        /// Download currency ticket for year specified by date
        /// </summary>
        /// <param name="date">Target year for ticket</param>
        /// <exception cref="CNBDownloadException"/>
        /// <returns></returns>
        public List<ExchangeRateTicket> DownloadYearTicket(DateTime date)
        {
            this.ValidateDate(date);
            string url = $"{this.URL_FOR_YEAR_TICKET}?rok={date.ToString("yyyy")}";
            string text = DownloadTicketText(url);
            List<ExchangeRateTicket> tickets = this.ParseYearTicket(text);
            return tickets;
        }
        #endregion

        #region Interface methods

        /// <summary>
        /// Check if today new ticket is availaible
        /// </summary>
        /// <returns></returns>
        public bool TodaysTicketIsAvailable()
        {
            //TODO: MM could cause potential isssue because of different time on server
            return (DateTime.Now.Hour > 14); 
        }


        /// <summary>
        /// Download currency ticket for todays date
        /// </summary>
        /// <exception cref="CNBDownloadException"/>
        /// <returns>currency ticket to todays date</returns>
        public ExchangeRateTicket DownloadTodaysTicket()
        {
            //TODO: may be required to add check if ticket is availaible
           return DownloadTicketForDate(DateTime.Now);
        }

        /// <summary>
        /// Download currency ticket for specified data
        /// </summary>
        /// <param name="date">target currency ticket date</param>
        /// <returns>Currency ticket or null if there is no ticket</returns>
        public ExchangeRateTicket DownloadTicketForDate(DateTime date)
        {
            this.ValidateDate(date);
            var dateString = date.ToString("dd.MM.yyyy");
            string url = $"{this.URL_FOR_DAY_TICKET}?date={dateString}";
            string text = this.DownloadTicketText(url);
            ExchangeRateTicket ticket = this.ParseDayTicket(text);
            return ticket;
        }

        /// <summary>
        /// Download all the availaible tickets for this bank
        /// </summary>
        /// <exception cref="CNBDownloadException"/>
        /// <returns>List of currency tickets</returns>
        public List<ExchangeRateTicket> DownloadAllTickets()
        {
           return DownloadTicketForInterval(this.MIN_DATE, DateTime.Now);
        }

        /// <summary>
        /// Downloads all availaible tickets in specified interval
        /// </summary>
        /// <param name="start">start date</param>
        /// <param name="end">end ticket</param>
        /// <exception cref="CNBDownloadException"/>
        /// <returns>List of currency tickets for interval</returns>
        public List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end)
        {
            ValidateDate(start);
            ValidateDate(end);
            if (start > end) throw new CNBInvalidDate($"Start date: {start.ToShortTimeString()} is after end date: {end.ToShortTimeString()}");
            int[] years = YearsInterval(start, end).ToArray();
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

        public void  SetLogger(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<CNBank>();
        }

        /// <summary>
        /// Takes two date and return array of years included in both dates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Array containing year in interval</returns>
        public static List<int> YearsInterval(DateTime start , DateTime end)
        {
            if (end.Year < start.Year) throw new CNBInvalidDate($"start: {start}, end: {start}");

            int difference = Math.Abs(start.Year - end.Year);
            int[] array = new int[difference+1];
            for (int index=0; index < difference + 1;index++)
            {
                array[index] = start.Year + index;
            }
            return array.ToList();
        }
        #region DayTicket



        private ExchangeRateTicket ParseDayTicket(string text)
        {
            string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
                throw new CNBTicketParseException(message: "Invalid number of lines");

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

        private void ParseDayTicketBody(string[] body, ref ExchangeRateTicket ticket)
        {
            foreach (string line in body)
            {
                string[] sections = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                string country = sections[0];
                string name = sections[1];
                int quantity = int.Parse(sections[2]);
                string isoName = sections[3];
                float buy = float.Parse(sections[4], CultureInfo.GetCultureInfo("cs-CZ"));
                ICurrencyData data = new ERDataBase(isoName, name, country, quantity, buy, null);
                ticket.AddExchangeRateData(data);
            }
        }
        #endregion

        #region Year ticket
        private List<ExchangeRateTicket> ParseYearTicket(string text){
            string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
                throw new CNBTicketParseException(message: "Invalid number of lines");

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

        public List<ICurrencyMetada> DownloadCurrencyMetadata()
        {
            ExchangeRateTicket ticket = DownloadTodaysTicket();    
            return ticket.GetExchangeRateData().ToList<ICurrencyMetada>();
        }

        
    } 
}

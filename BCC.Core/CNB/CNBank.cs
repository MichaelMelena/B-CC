using System;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using System.Linq;

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

        #region Interface implementation
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
            throw new NotImplementedException();
        }

        public List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region DayTicket



        private ExchangeRateTicket ParseDayTicket(string text)
        {
            string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
                throw new CNBInvalidData(message: "Invalid number of lines");

            string[] header = lines.ToList().GetRange(0, 2).ToArray();
            string[] body = lines.ToList().GetRange(2, lines.Length-2).ToArray();

            ExchangeRateTicket ticket = new ExchangeRateTicket();
            this.parseDayTicketHeader(header, ref ticket);
            this.ParseDayTicketBody(body, ref ticket);

            return ticket;
        }

        private void parseDayTicketHeader(string[] header, ref ExchangeRateTicket ticket) {
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
                string name = sections[0];
                string shortName = sections[1];
                int quantity = int.Parse(sections[2]);
                string isoName = sections[3];
                double buy = Double.Parse(sections[4], CultureInfo.GetCultureInfo("cs-CZ"));

                ExchangeRateData data = new ExchangeRateData(buy, null, quantity, name, shortName, isoName);
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
            
            List<YearCurrencyInfo> currencyInfos = this.ParseYearHeader(header);
            List<ExchangeRateTicket> tickets = this.ParseYearBody(body,ref currencyInfos);
            return tickets;
        }
        private List<YearCurrencyInfo> ParseYearHeader(string text)
        {
            string[] header = text.Split(new char[] { '|', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<YearCurrencyInfo> currencyInfos = new List<YearCurrencyInfo>(header.Length - 1);
            for (int i = 1; i < header.Length; i += 2)
            {
                int quntity = int.Parse(header[i]);
                string isoName = header[i + 1];
                currencyInfos.Add(new YearCurrencyInfo(quntity, isoName));
            }
            return currencyInfos;
        }

        private List<ExchangeRateTicket> ParseYearBody(string[] body,ref List<YearCurrencyInfo> currencyInfos)
        {
            List<ExchangeRateTicket> tickets = new List<ExchangeRateTicket>(body.Length);
            foreach (string line in body)
            {
                string[] sections = line.Split('|');
                DateTime date = DateTime.ParseExact(sections[0], "dd.MM.yyyy", CultureInfo.CurrentCulture);
                ExchangeRateTicket ticket = this.ParseYearExchangeRateData(sections, currencyInfos);
                ticket.TicketDate = date;
                tickets.Add(ticket);
            }
            return tickets;
        }
        
        private ExchangeRateTicket ParseYearExchangeRateData(string[] section, List<YearCurrencyInfo> currencyInfos)
        {
            ExchangeRateTicket ticket = new ExchangeRateTicket();
            for (int i = 1; i < section.Length; i++)
            {
                double buy = Double.Parse(section[i], CultureInfo.GetCultureInfo("cs-CZ"));
                YearCurrencyInfo currencyInfo = currencyInfos[i - 1];
                ExchangeRateData data = new ExchangeRateData(buy, null, currencyInfo.Quantity, null, null, currencyInfo.IsoName);
                ticket.AddExchangeRateData(data);
            }
            return ticket;
        }

        #endregion

        private struct YearCurrencyInfo
        {
            public YearCurrencyInfo(int quantity, string isoName)
            {
                this.Quantity = quantity;
                this.IsoName = isoName;
            }
            public int Quantity { get; private set; }
            public string IsoName { get; private set; }
        }
    } 
}

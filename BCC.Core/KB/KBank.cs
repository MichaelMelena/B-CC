using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace BCC.Core.KB
{
    public class KBank : IExchangeRateBank
    {
        public KBank()
        {

        }

        // konstanty
        private readonly string URL_CURRENT_DAY = "https://api.kb.cz/openapi/v1/exchange-rates";
        private readonly string URL_SPECIFIC_DATE = "https://api.kb.cz/openapi/v1/exchange-rates?ratesValidityDate=";
        private readonly DateTime MIN_DATE = new DateTime(year: 2010, month: 5, day: 26);
        // 6am - It's the first hour in the day which has upadated ticket for that day. 5am gets you ticket from previous day.

        #region Helper methods
        // Validates if date is in interval <minimal date - today date>
        private void ValidateDate(DateTime date)
        {
            if (date < this.MIN_DATE || date > DateTime.Today)
            {
                new KBInvalidDate($"Invalid date. Minimum is {this.MIN_DATE}. Maximum is {DateTime.Now}");
            }
        }

        // Downloads ticket using URL in Json format
        private bool DownloadTicketText(string url, out string jsonInput)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    jsonInput = client.DownloadString(url);
                }
            }
            catch(WebException ex)
            {
                using (StreamReader sr = new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()))
                {
                    jsonInput = sr.ReadToEnd();
                }
                return false;
            }
            return true;
        }
        #endregion

        #region JSON
        private class ExchangeRateKB
        {
            public string currencyISO { get; set; }
            public string currencyShortName { get; set; }
            public string currencyFullName { get; set; }
            public string country { get; set; }
            public string countryISO { get; set; }
            public DateTime ratesValidityDate { get; set; }
            public int currencyUnit { get; set; }
            public float middle { get; set; }
            public float cashBuy { get; set; }
            public float cashSell { get; set; }
            public float noncashBuy { get; set; }
            public float noncashSell { get; set; }
        }

        private class RootObject
        {
            public List<ExchangeRateKB> exchangeRates { get; set; }
        }
        #endregion

        #region Interface implementation
        // Gets the ticket for this day
        public ExchangeRateTicket DownloadTodaysTicket()
        {
            string jsonInput = null;
            if (DownloadTicketText(URL_CURRENT_DAY, out jsonInput))
            {
                ExchangeRateTicket ticketOutput = this.ParseDayTicket(jsonInput, DateTime.Today);
                return ticketOutput;
            }
            else
            {
                new KBInvalidDate(jsonInput);
                return null;
            }
        }

        // Gets the ticket for a specific day
        public ExchangeRateTicket DownloadTicketForDate(DateTime date)
        {
            ValidateDate(date);
            string jsonInput = null;
            string urlEnding = date.ToString("yyyy-MM-dd") + "T06:00:00.00Z"; //T06 = 6am - It's the first hour in the day which has upadated ticket for that day. 5am gets you ticket from previous day.
            if (DownloadTicketText(URL_SPECIFIC_DATE + urlEnding, out jsonInput))
            {
                ExchangeRateTicket ticketOutput = this.ParseDayTicket(jsonInput, date);
                return ticketOutput;
            }
            else
            {
                new KBInvalidDate(jsonInput);
                return null;
            }
        }

        // Get tickets in interval
        public List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end)
        {
            ValidateDate(start);
            ValidateDate(end);
            List<ExchangeRateTicket> tickets = new List<ExchangeRateTicket>();
            if (DateTime.Compare(start, end) < 0)
            {
                while (true)
                {
                    tickets.Add(DownloadTicketForDate(start));
                    start = start.AddDays(1);
                    if (DateTime.Compare(start, end) > 0) break;
                }
                return tickets;
            }
            else
            {
                new KBInvalidDate($"Invalid dates. First date is newer than the second one.");
                return null;
            }
        }

        // Get all tickets
        public List<ExchangeRateTicket> DownloadAllTickets()
        {
            DateTime start = MIN_DATE;
            List<ExchangeRateTicket> tickets = new List<ExchangeRateTicket>();
            if (DateTime.Compare(start, DateTime.Today) < 0)
            {
                while (true)
                {
                    tickets.Add(DownloadTicketForDate(start));
                    start = start.AddDays(1);
                    if (DateTime.Compare(start, DateTime.Today) > 0) break;
                }
                return tickets;
            }
            else
            {
                new KBInvalidDate($"Invalid dates. First date is newer than the second one.");
                return null;
            }
        }
 
        #endregion

        #region DayTicket
        // Converting of raw data to managable form
        private ExchangeRateTicket ParseDayTicket(string text, DateTime date)
        {
            List<RootObject> obj = JsonConvert.DeserializeObject<List<RootObject>>(text);
            ExchangeRateTicket ticket = new ExchangeRateTicket();
            List<ICurrencyData> data = new List<ICurrencyData>();
            foreach (var cur in obj[0].exchangeRates)
            {
                data.Add(new ERDataBase(cur.currencyISO, cur.currencyFullName, cur.country, cur.currencyUnit, cur.noncashBuy, cur.noncashSell));
                ticket.AddExchangeRateData(data[data.Count - 1]);
            }
            ticket.TicketDate = date;
            return ticket;
        }
        #endregion

        public List<ICurrencyMetada> DownloadCurrencyMetada()
        {
            ExchangeRateTicket ticket = DownloadTodaysTicket();
            ICurrencyData[] data = ticket.GetExchangeRateData();
            List<ICurrencyMetada> metaData = new List<ICurrencyMetada>(data.Length);
            return metaData;
        }
    }
}

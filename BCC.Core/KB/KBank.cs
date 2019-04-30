using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using BCC.Core.Abstract;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BCC.Core.KB
{
    public class KBank :ABank<KBank>, IExchangeRateBank
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
                throw new KBInvalidDate($"Invalid date. Minimum is {this.MIN_DATE}. Maximum is {DateTime.Now} (Current time)");
            }
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

        public void SetLogger(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<KBank>();
        }
        // Gets the ticket for this day
        public ExchangeRateTicket DownloadTodaysTicket()
        {
            string jsonInput = DownloadTicketText(URL_CURRENT_DAY);
            ExchangeRateTicket ticketOutput = this.ParseDayTicket(jsonInput, DateTime.Today);
            return ticketOutput;
        }

        // Gets the ticket for a specific day
        public ExchangeRateTicket DownloadTicketForDate(DateTime date)
        {
            ValidateDate(date);
            string urlEnding = date.ToString("yyyy-MM-dd") + "T06:00:00.00Z"; //T06 = 6am - It's the first hour in the day which has upadated ticket for that day. 5am gets you ticket from previous day.
            string jsonInput = DownloadTicketText($"{URL_SPECIFIC_DATE}{urlEnding}");
            ExchangeRateTicket ticketOutput = this.ParseDayTicket(jsonInput, date);
            return ticketOutput;
        }

        // Get tickets in interval
        public List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end)
        {
            ValidateDate(start);
            ValidateDate(end);
            if (start > end) throw new KBInvalidDate($"Start date: {start.ToShortTimeString()} is after end date: {end.ToShortTimeString()}");
            List<ExchangeRateTicket> tickets = new List<ExchangeRateTicket>();
            for (; start <= end; start = start.AddDays(1))
            {
                tickets.Add(DownloadTicketForDate(start));
            }
            return tickets;
        }

        // Get all tickets
        public List<ExchangeRateTicket> DownloadAllTickets()
        {
            return DownloadTicketForInterval(MIN_DATE.AddDays(1), DateTime.Now.AddDays(-1));
        }

        public bool TodaysTicketIsAvailable()
        {
            return (DateTime.Now.Hour > 6);
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

        public List<ICurrencyMetada> DownloadCurrencyMetadata()
        {
            ExchangeRateTicket ticket = DownloadTodaysTicket();
            return ticket.GetExchangeRateData().ToList<ICurrencyMetada>();
        }

    }
}

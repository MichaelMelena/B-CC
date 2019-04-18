﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using BCC.Core;
using Microsoft.Extensions.Logging;
using BCC.Core.Abstract;

namespace BCC.Core.RB
{
    public class RBank : ABank<RBank>, IExchangeRateBank
    {
        public RBank()
        {
        }

        Regex reg = new Regex(
                "<input type=\"text\" value=\"(?<quantity>\\d*)\" name=\"count\" data-value=\"\\d*\" /> </div> </td>[^<]*" +
                "<td class=\"code\" data-value=\"\\w{3}\">(?<iso>\\w{3})</td>[^<]*" +
                "<td class=\"value\" data-value=\"\\d*\\.\\d*\">(?<buy>\\d*\\.\\d*)</td>[^<]*" +
                "<td class=\"value\" data-value=\"\\d*\\.\\d*\">(?<sell>\\d*\\.\\d*)</td>[^<]*"
            );

        private readonly string URL = "https://www.rb.cz/informacni-servis/kurzovni-listek?date=";
        private DateTime min_date = DateTime.Today.AddDays(-181);
        // Whole URL is in format: URL + "yyyy-MM-dd"
        
        #region Helper methods
        private void ValidateDate(DateTime date)
        {
            if (date < this.min_date || date > DateTime.Today)
            {
                new RBInvalidDate($"Invalid date. Minimum is {this.min_date}. Maximum is {DateTime.Today}");
            }
        }
        private void RefreshMinDate()
        {
            min_date = DateTime.Today.AddDays(-181);
        }

        public static bool DownloadText(string url, out string input)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    input = wc.DownloadString(url);
                }
            }
            catch (WebException ex)
            {
                using (StreamReader sr = new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()))
                {
                    input = sr.ReadToEnd();
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Interface implementation

        public void SetLogger(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<RBank>();
        }

        public ExchangeRateTicket DownloadTodaysTicket()
        {
            RefreshMinDate();
            DateTime date = DateTime.Today;
            string url = String.Format("{0}{1}", URL, date.ToString("yyyy-MM-dd"));
            string input = null;
            if (DownloadText(url, out input))
            {
                ExchangeRateTicket ticket = ParseDayTicket(input, date);
                if (ticket != null) return ticket;
                else new RBInvalidDate("This ticket doesn't exist.");
                return null;
            }
            else
            {
                new RBInvalidDate(input);
                return null;
            }
        }

        public ExchangeRateTicket DownloadTicketForDate(DateTime date)
        {
            RefreshMinDate();
            ValidateDate(date);
            string url = String.Format("{0}{1}", URL, date.ToString("yyyy-MM-dd"));
            string input = null;
            if (DownloadText(url, out input))
            {
                ExchangeRateTicket ticket = ParseDayTicket(input, date);
                if (ticket != null) return ticket;
                else new RBInvalidDate("This ticket doesn't exist.");
                return null;
            }
            else
            {
                new RBInvalidDate(input);
                return null;
            }
        }

        public List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end)
        {
            RefreshMinDate();
            ValidateDate(start);
            ValidateDate(end);
            if (DateTime.Compare(start, end) < 0)
            {
                string url;
                List<ExchangeRateTicket> tickets = new List<ExchangeRateTicket>();
                string input = null;
                while (true)
                {
                    url = String.Format("{0}{1}", URL, start.ToString("yyyy-MM-dd"));
                    if (DownloadText(url, out input))
                    {
                        tickets.Add(ParseDayTicket(input, start));
                    }
                    else
                    {
                        new RBInvalidDate(input);
                        tickets.Add(null);
                    }
                    start = start.AddDays(1);
                    if (DateTime.Compare(start, end) > 0) break;
                }
                return tickets;
            }
            else
            {
                new RBInvalidDate($"Invalid dates. First date is newer than the second one.");
                return null;
            }
        }

        public List<ExchangeRateTicket> DownloadAllTickets()
        {
            RefreshMinDate();
            DateTime start = min_date;
            DateTime end = DateTime.Today;
            if (DateTime.Compare(start, end) < 0)
            {
                string url;
                List<ExchangeRateTicket> tickets = new List<ExchangeRateTicket>();
                string input = null;
                while (true)
                {
                    url = String.Format("{0}{1}", URL, start.ToString("yyyy-MM-dd"));
                    if (DownloadText(url, out input))
                    {
                        tickets.Add(ParseDayTicket(input, start));
                    }
                    else
                    {
                        new RBInvalidDate(input);
                        tickets.Add(null);
                    }
                    start = start.AddDays(1);
                    if (DateTime.Compare(start, end) > 0) break;
                }
                return tickets;
            }
            else
            {
                new RBInvalidDate($"Invalid dates. First date is newer than the second one.");
                return null;
            }
        }
        #endregion

        #region DayTicket
        // Converting of raw data to managable form
        private ExchangeRateTicket ParseDayTicket(string text, DateTime date)
        {
            ExchangeRateTicket ticket = new ExchangeRateTicket();
            List<ICurrencyData> data = new List<ICurrencyData>();
            Match match = reg.Match(text);
            string entryMatch = null;
            for (int i = 0; i < reg.Matches(text).Count; i++)
            {
                if (i == 0) entryMatch = match.Groups["iso"].Value;
                else if (match.Groups["iso"].Value == entryMatch) break;
                data.Add(new ERDataBase(match.Groups["iso"].Value, null, null, int.Parse(match.Groups["quantity"].Value), float.Parse(match.Groups["buy"].Value, System.Globalization.CultureInfo.InvariantCulture), float.Parse(match.Groups["sell"].Value, System.Globalization.CultureInfo.InvariantCulture)));
                ticket.AddExchangeRateData(data[data.Count - 1]);
                match = match.NextMatch();
            }
            ticket.TicketDate = date;
            return ticket;
        }
        #endregion

        public List<ICurrencyMetada> DownloadCurrencyMetadata()
        {
            ExchangeRateTicket ticket = DownloadTodaysTicket();
            ICurrencyData[] data = ticket.GetExchangeRateData();
            List<ICurrencyMetada> metaData = new List<ICurrencyMetada>(data.Length);
            return metaData;
        }

        public bool TodaysTicketIsAvailable()
        {
            return (DateTime.Now.Hour > 6);//TODO: not a valid value just placeholder
        }

    }
}

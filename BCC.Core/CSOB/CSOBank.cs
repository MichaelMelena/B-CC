using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using BCC.Core.Abstract;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BCC.Core.CSOB
{
    public class CSOBank :ABank<CSOBank>, IExchangeRateBank
    {
        public CSOBank() {}

        // Whole URL is in format: URL_FRONT + "yyyy-MM-dd" + URL_END
        private readonly string URL_FRONT = "https://www.csob.cz/portal/lide/kurzovni-listek/-/date/";
        private readonly string URL_END = "/kurzovni-listek.xml";
        private readonly DateTime MIN_DATE = new DateTime(year: 1999, month: 01, day: 01);

        #region Helper methods
        private void ValidateDate(DateTime date)
        {
            if (date < this.MIN_DATE || date > DateTime.Now)
            {
                throw new CSOBInvalidDate($"Invalid date. Minimum is {this.MIN_DATE}. Maximum is {DateTime.Today}(Current time)");
            }
        }

        #region XML Classes
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class ExchangeRate
        {
            private ExchangeRateCountry[] countryField;
            private System.DateTime dateField;
            private string sourceField;

            [System.Xml.Serialization.XmlElementAttribute("Country")]
            public ExchangeRateCountry[] Country
            {
                get { return this.countryField; }
                set { this.countryField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public System.DateTime Date
            {
                get { return this.dateField; }
                set { this.dateField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Source
            {
                get { return this.sourceField; }
                set { this.sourceField = value; }
            }
        }

        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ExchangeRateCountry
        {
            private ExchangeRateCountryFXcashless fXcashlessField;
            private ExchangeRateCountryFXcash fXcashField;
            private string idField;
            private string countryField;
            private byte quotaField;

            public ExchangeRateCountryFXcashless FXcashless
            {
                get { return this.fXcashlessField; }
                set { this.fXcashlessField = value; }
            }

            public ExchangeRateCountryFXcash FXcash
            {
                get { return this.fXcashField; }
                set { this.fXcashField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string ID
            {
                get { return this.idField; }
                set { this.idField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Country
            {
                get { return this.countryField; }
                set { this.countryField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte quota
            {
                get { return this.quotaField; }
                set { this.quotaField = value; }
            }
        }

        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ExchangeRateCountryFXcashless
        {
            private decimal buyField;
            private decimal saleField;
            private decimal middleField;
            private string changeField;

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public decimal Buy
            {
                get { return this.buyField; }
                set { this.buyField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public decimal Sale
            {
                get { return this.saleField; }
                set { this.saleField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public decimal Middle
            {
                get { return this.middleField; }
                set { this.middleField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Change
            {
                get { return this.changeField; }
                set { this.changeField = value; }
            }
        }

        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ExchangeRateCountryFXcash
        {
            private string buyField;
            private string saleField;
            private string middleField;
            private string changeField;

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Buy
            {
                get { return this.buyField; }
                set { this.buyField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Sale
            {
                get { return this.saleField; }
                set { this.saleField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Middle
            {
                get { return this.middleField; }
                set { this.middleField = value; }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Change
            {
                get { return this.changeField; }
                set { this.changeField = value; }
            }
        }
        #endregion

        #endregion

        #region Interface implementation
        public ExchangeRateTicket DownloadTodaysTicket()
        {
            return DownloadTicketForDate(DateTime.Now);
        }

        public ExchangeRateTicket DownloadTicketForDate(DateTime date)
        {
            ValidateDate(date);
            string url = $"{URL_FRONT}{date.ToString("yyyy-MM-dd")}{URL_END}";
            string input = DownloadTicketText(url);
            ExchangeRateTicket ticket = ParseDayTicket(input, date);
            if (ticket == null) throw new CSOBInvalidData("This ticket doesn't exist.");
            return ticket;
        }

        public List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end)
        {
            ValidateDate(start);
            ValidateDate(end);
            if (start > end) throw new CSOBInvalidDate($"Start date: {start.ToShortTimeString()} is after end date: {end.ToShortTimeString()}");
            List<ExchangeRateTicket> tickets = new List<ExchangeRateTicket>();
            for (; start <= end; start = start.AddDays(1))
            {
                tickets.Add(DownloadTicketForDate(start));
            }
            return tickets;
        }

        public List<ExchangeRateTicket> DownloadAllTickets()
        {
            return DownloadTicketForInterval(MIN_DATE, DateTime.Now);
        }

        public bool TodaysTicketIsAvailable()
        {
            return (DateTime.Now.Hour > 6 ); //should be zero
        }

        public void SetLogger(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<CSOBank>();
        }
        #endregion

        #region DayTicket
        // Converting of raw data to managable form
        private ExchangeRateTicket ParseDayTicket(string text, DateTime date)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            ExchangeRate er = null;
            ExchangeRateTicket ticket = new ExchangeRateTicket();
            List<ICurrencyData> data = new List<ICurrencyData>();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ExchangeRate));
                er = (ExchangeRate)serializer.Deserialize(ms);
            }
            if (er.Country != null)
            {
                foreach (var cur in er.Country)
                {
                    data.Add(new ERDataBase(cur.ID, null, cur.Country, cur.quota, (float)cur.FXcashless.Buy, (float)cur.FXcashless.Sale));
                    ticket.AddExchangeRateData(data[data.Count - 1]);
                }
                ticket.TicketDate = date;
                return ticket;
            }
            else
            {
                return null;
            }
        }
        #endregion

        public List<ICurrencyMetada> DownloadCurrencyMetadata()
        {
            ExchangeRateTicket ticket = DownloadTodaysTicket();
            return ticket.GetExchangeRateData().ToList<ICurrencyMetada>();
        }
    }
}

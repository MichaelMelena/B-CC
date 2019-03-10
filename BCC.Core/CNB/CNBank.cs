using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Globalization;
using System.Linq;

namespace BCC.Core.CNB
{
    public class CNBank : IExchangeRateBank
    {
        public CNBank()
        {

        }
        private readonly string URL_FOR_DAY_TICKET = "http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt";


        private readonly string URL_FOR_YEAR_TICKET = "http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/rok.txt";

        private readonly DateTime MIN_DATE = new DateTime(year: 1992, month: 1, day: 1);

        #region DayTicket
        private string DownloadDayTicketText(DateTime date){

            if (date < this.MIN_DATE){
                new CNBInvalidDate("Chosen date is older than required minimum");
            }
            string responseText = null;
            using (WebClient webClient = new WebClient())
            {
                var dateString = date.ToString("dd.MM.yyyy");
                string url = $"{this.URL_FOR_DAY_TICKET}?date={dateString}";
                responseText =  webClient.DownloadString(url);
            }
            return responseText;
        }
        private ExchangeRateTicket ParseDayTicket(string text)
        {
            string[] lines = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
        }

        private void ParseDayTicketBody(string[] body,ref ExchangeRateTicket ticket)
        {
            foreach(string line in body)
            {
                string[] sections = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                string name = sections[0];
                string shortName = sections[1];
                int quantity = int.Parse(sections[2]);
                string isoName = sections[3];
                double buy = Double.Parse(sections[4], CultureInfo.GetCultureInfo("cs-CZ"));

                ExchangeRateData data = new ExchangeRateData(buy, null, quantity, name, shortName, isoName);
                ticket.AddExchangeRateData(data);
            }
        }

        public ExchangeRateTicket DownloadTicketForDate(DateTime date)
        {
            string text = this.DownloadDayTicketText(date);
            ExchangeRateTicket ticket = this.ParseDayTicket(text);
            return ticket;
        }
        #endregion


        public List<ExchangeRateTicket> DownloadAllTickets()
        {
            throw new NotImplementedException();
        }

        

         public List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

         public ExchangeRateTicket DownloadTodaysTicket()
        {
            string text = this.DownloadDayTicketText(DateTime.Now);
            ExchangeRateTicket ticket = this.ParseDayTicket(text);
            return ticket;
        }
    }


    
}

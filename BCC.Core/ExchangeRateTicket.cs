using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public class ExchangeRateTicket
    {
        private List<ExchangeRateData> Data { get; set; }

        public DateTime TicketDate { get; private set; }

        public ExchangeRateTicket()
        {
            Data = new List<ExchangeRateData>();
        }
        public ExchangeRateTicket(DateTime ticketDate):this()
        {
            this.TicketDate = ticketDate;
        }
        public override string ToString()
        {
            return base.ToString();
        }
        
        public void AddExchangeRateData(ExchangeRateData data){
            this.Data.Add(data);
        }
    }
}

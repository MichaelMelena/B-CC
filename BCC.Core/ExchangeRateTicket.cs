using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public class ExchangeRateTicket: IComparable<ExchangeRateTicket>, IComparable<DateTime>
    {
        private List<ICurrencyData> Data { get; set; }

        public DateTime TicketDate { get; set; }
        
        public ExchangeRateTicket()
        {
            Data = new List<ICurrencyData>();
        }
        public ExchangeRateTicket(DateTime ticketDate):this()
        {
            this.TicketDate = ticketDate;
        }
               
        public void AddExchangeRateData(ICurrencyData data){
            this.Data.Add(data);
        }
        public ICurrencyData[] GetExchangeRateData()
        {
            return this.Data.ToArray();
        }

        public int CompareTo(ExchangeRateTicket other)
        {
            return DateTime.Compare(this.TicketDate, other.TicketDate);
        }

        public int CompareTo(DateTime other)
        {
            return DateTime.Compare(this.TicketDate, other);
        }
    }
}

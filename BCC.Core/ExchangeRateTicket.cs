using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public class ExchangeRateTicket: IComparable<ExchangeRateTicket>, IComparable<DateTime>
    {
        private List<ExchangeRateData> Data { get; set; }

        /// <summary>
        /// Enables to set ticket date only if it is null (can be set only once)
        /// </summary>


        public DateTime TicketDate { get; set; }

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
        public ExchangeRateData[] GetExchangeRateData()
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

        public class SortByBuy : IComparer<ExchangeRateData>
        {
            public int Compare(ExchangeRateData x, ExchangeRateData y)
            {   if (x == null) return -1;
                if (y == null) return 1; 
                double  difference = (x.Buy - y.Buy);
                if (difference < 1e-10) return 0;
                else if (difference > 0) return 1;
                else return -1;
            }
        }

    }
}

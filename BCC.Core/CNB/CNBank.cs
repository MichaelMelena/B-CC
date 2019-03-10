using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core.CNB
{
    class CNBank : IExchangeRateBank
    {
        public CNBank()
        {

        }

        public List<ExchangeRateTicket> DownloadAllTickets()
        {
            throw new NotImplementedException();
        }

        public ExchangeRateTicket DownloadTicketForDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public List<ExchangeRateTicket> DownloadTicketForInterva(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public ExchangeRateTicket DownloadTodaysTicket()
        {
            throw new NotImplementedException();
        }
    }
}

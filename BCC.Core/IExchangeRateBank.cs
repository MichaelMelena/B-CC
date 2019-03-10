using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    interface IExchangeRateBank
    {
        ExchangeRateTicket DownloadTodaysTicket();

        ExchangeRateTicket DownloadTicketForDate(DateTime date);

        List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end);

        List<ExchangeRateTicket> DownloadAllTickets(); 
    }
}

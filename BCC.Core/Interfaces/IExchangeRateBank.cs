using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Text;


namespace BCC.Core
{
    public interface IExchangeRateBank
    {

     
        ExchangeRateTicket DownloadTodaysTicket();

        ExchangeRateTicket DownloadTicketForDate(DateTime date);

        List<ExchangeRateTicket> DownloadTicketForInterval(DateTime start, DateTime end);

        List<ExchangeRateTicket> DownloadAllTickets();

        List<ICurrencyMetada> DownloadCurrencyMetadata();

        void SetLogger(ILoggerFactory loggerFactory);

        bool TodaysTicketIsAvailable();

       
    }
}

using System;
using System.Collections.Generic;


namespace BCC.Core
{
    public class ExchangeRateManager
    {
        private List<IExchangeRateBank> Banks { get; set; }

        public void DownloadTodaysTicket()
        {
            throw new NotImplementedException();
        }

        public void DownloadTicketsForDate(DateTime date) {
            throw new NotImplementedException();
        }

        public void DownloadTicketsForInterva(DateTime start, DateTime end) {
            throw new NotImplementedException();
        }

        public void DownloadAllTicket(){
            throw new NotImplementedException();
        }

        private void SaveExchageRateData(ExchangeRateData data){
            throw new NotImplementedException();
        }

        private void SaveExchangeRateTicket(ExchangeRateTicket ticket){
            throw new NotImplementedException();
        }
    }
}

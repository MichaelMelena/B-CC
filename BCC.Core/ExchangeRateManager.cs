using System;
using System.Collections.Generic;
using System.Threading;
using BCC.Core.CNB;
using BCC.Model.Models;
using System.Reflection;
using System.Linq;

namespace BCC.Core
{
    public class ExchangeRateManager
    {

        private static readonly Lazy<ExchangeRateManager> singelton = new Lazy<ExchangeRateManager>(() => new ExchangeRateManager());

        static ExchangeRateManager()
        {
            
        }

        public static ExchangeRateManager Instance
        {
            get
            {
                return singelton.Value;
            }
        }

        private Timer Timer { get; set; }

        private IDictionary<string,IExchangeRateBank> Banks { get; set; }

        private ExchangeRateManager()
        {
            this.Banks = new Dictionary<string, IExchangeRateBank>();

            List<BankConnector> connectors = null;
            using (BCCContext context = new BCCContext())
            {
                connectors =  context.BankConnector.Where(x => x.Enabled == true).ToList();
            }
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (BankConnector connector in connectors)
            {
                Type type = assembly.GetType(connector.DllName);
                IExchangeRateBank bank = (IExchangeRateBank)Activator.CreateInstance(type);
                this.Banks.Add(connector.BankShortName, bank);
            }



            //tries to populate database with 20 exchange rate tickets for each bank
            bool isFirstTime = IsFirstTimeSetup();
            if (isFirstTime) FirstTimeSetup();

            //Timer = new Timer(TimerTick, null, 0, 600_000);
        }
        
        private void TimerTick(object state)
        {
           
        }

        private bool IsFirstTimeSetup()
        {
            return false;
        }
        private void FirstTimeSetup()
        {

        }
        

        public void DownloadTodaysTicket()
        {
            foreach (KeyValuePair<string,IExchangeRateBank> bank in Banks)
            {
                ExchangeRateTicket ticket =  bank.Value.DownloadTodaysTicket();
                MapERTicketToDB(ticket, bank.Key);
            }
        }
        
        private void MapERTicketToDB(ExchangeRateTicket eRTicket,string bankName)
        {
            if (eRTicket == null || bankName == null) throw new BCCERMNullReference();

            using (BCCContext context = new BCCContext())
            {
                Ticket ticket = context.Ticket.Where(x => x.Date == eRTicket.TicketDate && x.BankShortName == bankName).FirstOrDefault();
                if (ticket == null)
                {
                    ticket = new Ticket()
                    {
                        BankShortName = bankName,
                        Date = eRTicket.TicketDate
                    };
                    context.Ticket.Add(ticket);
                    context.SaveChanges();
                }
                ExchangeRateData[] eRData = eRTicket.GetExchangeRateData();
                foreach (ExchangeRateData data in eRData)
                {
                    Currency currency = new Currency();
                    currency.TicketId = ticket.Id;
                    currency.IsoName = data.IsoName;
                    currency.Buy = (float)data.Buy;
                    currency.Sell =(float?)data.Sell;
                    context.Add(currency);
                }
                context.SaveChanges();
            }
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

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using BCC.Core.CNB;
using BCC.Model.Models;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace BCC.Core
{
    public class ExchangeRateManager: IExchangeRateManager
    {
        #region Constatns

        /// <summary>
        /// Constat representing initial size of the <see cref="Banks"/> collection
        /// </summary>
        private readonly int INITIAL_BANKS_SIZE = 4;

        private readonly BCCContext _context;

        private readonly BCCContext _threadContext;

        private readonly IServiceProvider _serviceProvider;
        
        /// <summary>
        /// Constats stating the number of concurent accesses to <see cref="Banks"/>
        /// </summary>
        private readonly int BANKS_CONCURENCY_LEVEL = 2;

        #endregion

        #region Properties
        private Timer Timer { get; set; }

        private IDictionary<string,IExchangeRateBank> Banks { get; set; }
        #endregion

       

        public ExchangeRateManager(BCCContext context, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _threadContext = serviceProvider.GetService<BCCContext>();
            _context = context;
            this.Banks = new ConcurrentDictionary<string, IExchangeRateBank>(BANKS_CONCURENCY_LEVEL, INITIAL_BANKS_SIZE);

            List<BankConnector> connectors = null;
            
                connectors =  _context.BankConnector.Where(x => x.Enabled == true).ToList();
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (BankConnector connector in connectors)
            {
                try
                {
                    Type type = assembly.GetType(connector.DllName);
                    IExchangeRateBank bank = (IExchangeRateBank)Activator.CreateInstance(type);
                    this.Banks.Add(connector.BankShortName, bank);
                }
                catch (BCCCoreException ex)
                {
                    //TODO: MM add logging
                }

            }
            //tries to populate database with 20 exchange rate tickets for each bank
            bool isFirstTime = IsFirstTimeSetup();
            if (isFirstTime) FirstTimeSetup();

            //Timer = new Timer(TimerTick, null, 0, 600_000);
        }


        #region Interface methods

        public void DownloadTodaysTicket()
        {



        }

        #endregion

        #region Backround operations
        private void TimerTick(object state)
        {
            using(BCCContext context = _serviceProvider.GetService<BCCContext>())
            {
                foreach(string bankName in Banks.Keys)
                {
                    DownloadTodaysBankTicket(bankName, context);
                }
            }
        }
        private bool IsFirstTimeSetup()
        {
            return false;
        }

        private void FirstTimeSetup()
        {

        }

        #endregion


        #region Bank operations

        private void DownloadCurrencyMetadata(BCCContext context)
        {
            foreach (string bankName in Banks.Keys)
            {
                DownloadBankCurrencyMetadata(context, bankName);
            }
        }
        private void DownloadTodaysBankTicket(string bankName, BCCContext context)
        {
            try
            {
                Ticket ticket = context.Ticket.FirstOrDefault(x => x.BankShortName == bankName && x.Date.Day == DateTime.Now.Day && x.Date.Month == DateTime.Now.Month && x.Date.Year == DateTime.Now.Year);
                if (ticket == null)
                {
                    IExchangeRateBank bank;
                    if (Banks.TryGetValue(bankName, out bank))
                    {
                        ExchangeRateTicket erTicket = bank.DownloadTodaysTicket();
                        SaveERTciket(context, erTicket, bankName);
                    }
                }
            }
            catch (BCCCoreException ex)
            {
                //TODO: MM add logging
            }
        }

        private void DownloadBankCurrencyMetadata(BCCContext context, string bankName)
        {
            IExchangeRateBank bank;
            if (Banks.TryGetValue(bankName, out bank))
            {
                List<ICurrencyMetada> metaData = bank.DownloadCurrencyMetada();
                foreach (ICurrencyData meta in metaData)
                {
                    SaveBankCurrencyMetada(context, meta);
                }
                context.SaveChanges();
            }

        }

        #endregion


        #region DB Save

        private void SaveBankCurrencyMetada(BCCContext context, ICurrencyMetada metaData)
        {
            CurrencyMetadata ret = context.CurrencyMetadata.Where(x => x.IsoName == metaData.ISOName).FirstOrDefault();
            if (ret == null)
            {
                ret = new CurrencyMetadata()
                {
                    IsoName = metaData.ISOName,
                    Name = metaData.Name,
                    Country = metaData.Country,
                    Quantity = metaData.Quantity
                };
            }
            else
            {
                ret.Name = metaData.Name;
                ret.Country = metaData.Country;
                ret.Quantity = metaData.Quantity;
            }
        }


        private void SaveERTciket(BCCContext context, ExchangeRateTicket eRTicket, string bankName)
        {
            if (eRTicket == null || bankName == null) throw new BCCERMNullReference();

            Ticket ticket = context.Ticket.Where(x => x.Date == eRTicket.TicketDate && x.BankShortName == bankName).FirstOrDefault();
            if (ticket == null)
            {
                ticket = new Ticket()
                {
                    BankShortName = bankName,
                    Date = eRTicket.TicketDate
                };
                context.Ticket.Add(ticket);
                // TODO: MM odstranit pokud neni potreba context.SaveChanges();
            }
            ICurrencyData[] eRData = eRTicket.GetExchangeRateData();
            foreach (ICurrencyData data in eRData)
            {
                Currency currency = new Currency();
                currency.TicketId = ticket.Id;
                currency.IsoName = data.ISOName;
                currency.Buy = data.Buy;
                currency.Sell = data.Sell;
                if (context.Currency.Where(x => x.IsoName == currency.IsoName && x.TicketId == currency.TicketId) == null)
                {
                    context.Add(currency);
                }
            }
            context.SaveChanges();
        }

        #endregion
          
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using BCC.Core.CNB;
using BCC.Model.Models;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace BCC.Core
{
    public class BankManager: Microsoft.Extensions.Hosting.BackgroundService, IBankManager
    {
        #region Constatns

        /// <summary>
        /// Constat representing initial size of the <see cref="Banks"/> collection
        /// </summary>
        private readonly int INITIAL_BANKS_SIZE = 4;

        private readonly BCCContext _context;

        private readonly int TASK_DELAY = 60000;

        /// <summary>
        /// Constats stating the number of concurent accesses to <see cref="Banks"/>
        /// </summary>
        private readonly int BANKS_CONCURENCY_LEVEL = 2;


        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        #endregion

        #region Properties

        private IDictionary<string,IExchangeRateBank> Banks { get; set; }
        #endregion

        public BankManager(IServiceProvider serviceProvider)
        {
            _context = new BCCContext();
            this.Banks = new ConcurrentDictionary<string, IExchangeRateBank>(BANKS_CONCURENCY_LEVEL, INITIAL_BANKS_SIZE);


            Assembly assembly = Assembly.GetExecutingAssembly();
            ConnectorMaintainance(_context, assembly);
           
            
            //tries to populate database with 20 exchange rate tickets for each bank
            bool isFirstTime = IsFirstTimeSetup();
            if (isFirstTime) FirstTimeSetup();
        }

        private bool IsFirstTimeSetup()
        {
            return false;
        }

        private void FirstTimeSetup()
        {
            throw new NotImplementedException();
        }

     
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

        #endregion

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
                context.SaveChanges();
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
                if (context.Currency.FirstOrDefault(x => x.IsoName == currency.IsoName && x.TicketId == currency.TicketId) == null)
                {
                    context.Add(currency);
                }
            }
            context.SaveChanges();
        }



        private void ConnectorMaintainance(BCCContext context, Assembly assembly)
        {
            List<BankConnector> connectors = context.BankConnector.ToList();
            foreach (BankConnector connector in connectors)
            {// kontrola nastaveni

                if (Banks.Keys.Contains(connector.BankShortName))
                {
                    if (!connector.Enabled.HasValue || !connector.Enabled.Value)
                    {
                        //removes bank from active banks
                        Banks.Remove(connector.BankShortName);
                    }
                }
                else
                {
                    if (connector.Enabled.HasValue && connector.Enabled.Value)
                    {   //add bank to active banks
                        Type type = assembly.GetType(connector.DllName);
                        IExchangeRateBank bank = (IExchangeRateBank)Activator.CreateInstance(type);
                        Banks.Add(connector.BankShortName, bank);
                    }
                }
            }
        }
        #region BackgroundService
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {


                    //reacts to changes in connector config
                    ConnectorMaintainance(_context, assembly);
                    foreach (string bankName in Banks.Keys)
                    {
                        DownloadTodaysBankTicket(bankName, _context);
                    }

                    await Task.Delay(TASK_DELAY, _stoppingCts.Token);
                }
                catch (BCCCoreException ex)
                {
                    //TODO: MM add logging
                }
            }
            _executingTask = Task.CompletedTask;
            return;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it, 
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                                                              cancellationToken));
            }

        }

        public override void Dispose()
        {
            _stoppingCts.Cancel();
        }


        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using BCC.Model.Models;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using  BCC.Core.Utils;

namespace BCC.Core
{
    public class BankManager: Microsoft.Extensions.Hosting.BackgroundService, IBankManager
    {
        #region Constatns

        /// <summary>
        /// Constat representing initial size of the <see cref="Banks"/> collection
        /// </summary>
        private readonly int INITIAL_BANKS_SIZE = 4;

        private readonly int TICKET_HISTORY_LENGTH = 120;

       

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
        private readonly ILogger<BankManager> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILoggerFactory loggerFactory; 
        #endregion

        public BankManager(IServiceScopeFactory serviceScopeFactory,ILoggerFactory loggerFactory, ILogger<BankManager> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
            this.loggerFactory = loggerFactory;
            this.Banks = new ConcurrentDictionary<string, IExchangeRateBank>(BANKS_CONCURENCY_LEVEL, INITIAL_BANKS_SIZE);


            Assembly assembly = Assembly.GetExecutingAssembly();

            using (var scope = new DbScope(this.serviceScopeFactory))
            {
                ConnectorMaintenance(scope.Context, assembly);
            }
            

            this.logger.LogDebug("BankManager was initiated");
            
            //tries to populate database with 20 exchange rate tickets for each bank
            
        }

        private bool IsFirstTimeSetup()
        {
            using (var scope = new DbScope(serviceScopeFactory))
            {
                if (scope.Context.Ticket.ToList().Count <= 0)
                {
                    scope.Context.Ticket.RemoveRange(scope.Context.Ticket);
                    scope.Context.SaveChanges();
                    return true;
                }
                return false;
            }
           
        }

        private void FirstTimeSetup()
        {
            using (var scope = new  DbScope(serviceScopeFactory))
            {
                foreach (string bankName in Banks.Keys)
                {
                    IExchangeRateBank bank;
                    if (Banks.TryGetValue(bankName, out bank))
                    {

                        try
                        {
                            List<ICurrencyMetada> metadata = bank.DownloadCurrencyMetadata();
                            foreach (ICurrencyMetada meta in metadata)
                            {

                                SaveBankCurrencyMetadata(scope.Context, meta);
                            }
                        }
                        catch (BCCCoreException ex)
                        {
                           logger.LogDebug(ex,$"Failed downloading/processing currency metadata for Bank: {bankName}");
                        }


                    }
                }

                foreach (string bankName in Banks.Keys)
                {
                    IExchangeRateBank bank;
                    if (Banks.TryGetValue(bankName, out bank))
                    {
                        try
                        {
                            List<ExchangeRateTicket> tickets = bank.DownloadTicketForInterval(DateTime.Now.AddDays(-TICKET_HISTORY_LENGTH), DateTime.Now.AddDays(-1));
                            //List<ExchangeRateTicket> tickets = bank.DownloadAllTickets();
                            foreach (ExchangeRateTicket ticket in tickets)
                            {
                                SaveERTicket(scope.Context, ticket, bankName);
                            }
                        }
                        catch (BCCCoreException ex)
                        {
                            logger.LogError(ex,$"Error occured for Bank: {bank}");
                        }
                    }
                }
            }
           
               
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
                if (ticket == null  )
                {
                    IExchangeRateBank bank;
                    if (Banks.TryGetValue(bankName, out bank))
                    {
                        if (bank.TodaysTicketIsAvailable())
                        {
                            ExchangeRateTicket erTicket = bank.DownloadTodaysTicket();
                            SaveERTicket(context, erTicket, bankName);
                        }
                       
                    }
                }
            }
            catch (BCCCoreException ex)
            {
                logger.LogError(ex, $"Failed to download today's ticket for Bank: {bankName}");
            }
        }

        private void DownloadBankCurrencyMetadata(BCCContext context, string bankName)
        {
            IExchangeRateBank bank;
            if (Banks.TryGetValue(bankName, out bank))
            {
                List<ICurrencyMetada> metaData = bank.DownloadCurrencyMetadata();
                foreach (ICurrencyData meta in metaData)
                {
                    SaveBankCurrencyMetadata(context, meta);
                }
                context.SaveChanges();
            }
        }

        #endregion


        #region DB Save

        private void SaveBankCurrencyMetadata(BCCContext context, ICurrencyMetada metaData)
        {
            CurrencyMetadata ret = context.CurrencyMetadata.FirstOrDefault(x => x.IsoName == metaData.ISOName);
            if (ret == null)
            {
                
                if (string.IsNullOrWhiteSpace(metaData.ISOName)) return;
                if (metaData.Quantity < 1) return;
                ret = new CurrencyMetadata()
                {
                    IsoName = metaData.ISOName,
                    Name = metaData.Name,
                    Quantity = metaData.Quantity,
                    Country = metaData.Country
                };
                context.CurrencyMetadata.Add(ret);
                context.SaveChanges();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ret.Name) && !string.IsNullOrWhiteSpace(metaData.Name)) ret.Name = metaData.Name;
                if (string.IsNullOrWhiteSpace(ret.Country) && !string.IsNullOrWhiteSpace(ret.Country)) ret.Country = metaData.Country;
            }
        }

        #endregion

        private void SaveERTicket(BCCContext context, ExchangeRateTicket eRTicket, string bankName)
        {
            if (eRTicket == null || bankName == null) throw new BCCERMNullReference();

            Ticket ticket = context.Ticket.FirstOrDefault(x => x.Date == eRTicket.TicketDate && x.BankShortName == bankName);
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



        private void ConnectorMaintenance(BCCContext context, Assembly assembly)
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
                        bank.SetLogger(loggerFactory);

                        Banks.Add(connector.BankShortName, bank);
                    }
                }
            }
        }
        #region BackgroundService
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                bool isFirstTime = IsFirstTimeSetup();
                if (isFirstTime) FirstTimeSetup();
            }
            catch(BCCCoreException ex)
            {
                logger.LogDebug(ex,$"Failed on first time setup");
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = new DbScope(serviceScopeFactory))
                {
                    try
                    {
                        //reacts to changes in connector config
                        ConnectorMaintenance(scope.Context, assembly);
                        foreach (string bankName in Banks.Keys)
                        {
                            //TODO: MM tickets can be downloaded asynchronously
                            DownloadTodaysBankTicket(bankName, scope.Context);
                        }
                    }
                    catch (BCCCoreException ex)
                    {
                        logger.LogDebug(ex,$"Failure on one of the Banks");
                    }
                }
                await Task.Delay(TASK_DELAY, _stoppingCts.Token);

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

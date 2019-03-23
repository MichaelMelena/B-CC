using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BCC.Model.Models;
using System.Linq;
using Newtonsoft.Json;
using System.Dynamic;

namespace BCC.Core
{
    public class PresentationManger : IPresentationManager
    {
        private readonly BCCContext _context;
        public PresentationManger(BCCContext context)
        {
            this._context = context;
        }

        #region Interface methods
        public DataTable GetBestOfDateTableData(DateTime date)
        {
            string tableName = $"Best of {date.ToShortDateString()}";
            return CreateRecomendationTable(tableName, date, out bool err);
        }

        public DataTable GetBuyTableData(DateTime date)
        {
            return BSTable("Buy Table", true, date);
        }

        public DataTable GetCurrencyChangeTableData(string bankName, DateTime date)
        {
            throw new NotImplementedException();
        }

   
        public DataTable GetRecomendationTableData(DateTime date)
        {
            string tableName = $"Recomendations for {date.ToShortDateString()}";

            DataTable today = CreateRecomendationTable("Today", date, out bool todayErr);

            DataTable yesterday = CreateRecomendationTable("Yesterday", date.AddDays(-1), out bool yesterdayErr);
            if(todayErr || yesterdayErr)
            {
                return ErrorTable(tableName, "Couldnt create recomendation");
            }

            DataTable table = new DataTable(tableName);
            table.Columns.Add("Currency", typeof(string));
            table.Columns.Add("Buy Change", typeof(string));
            table.Columns.Add("Sell Change", typeof(string));
            table.Columns.Add("Action", typeof(string));
            table.Columns.Add("Bank", typeof(string));
            int i = 0;
            foreach (DataRow row in today.Rows)
            {
                object[] rowData = new object[table.Columns.Count];

                DataRow old = yesterday.Rows[i++];
                if (old != null)
                {
                    rowData[0] = row[0];
                    float oldBuy, todayBuy, oldSell, todaySell;
                    string buyBank = row[2].ToString();
                    string sellBank = row[4].ToString();
                    if (float.TryParse(old[1].ToString(), out oldBuy) && float.TryParse(row[1].ToString(), out todayBuy) && float.TryParse(old[3].ToString(), out oldSell) && float.TryParse(row[3].ToString(), out todaySell))
                    {
                        float diffBuy = todayBuy - oldBuy;
                        float diffSell = todaySell - oldSell;
                        float absBuy = Math.Abs(diffBuy);
                        float absSell = Math.Abs(diffSell);
                        rowData[1] = diffBuy.ToString("P");
                        rowData[2] = diffSell.ToString("P");

                        string recomend = null;
                        string bank = null;

                        if(diffBuy >= 0 && diffSell>=0)
                        {
                            recomend = "Sell";
                            bank = sellBank;
                        }
                        else if(diffBuy< 0 && diffSell > 0)
                        {
                            if(absBuy > absSell)
                            {
                                recomend = "Buy";
                                bank = buyBank;
                            }
                            else
                            {
                                recomend = "Sell";
                                bank = sellBank;
                            }
                        }
                        else if(diffBuy < 0 && diffSell < 0)
                        {
                            recomend = "Buy";
                            bank = buyBank;
                        }
                        else if(diffBuy > 0 && diffSell < 0)
                        {
                            if(absBuy > absSell)
                            {
                                recomend = "Hold (Sell)";
                                bank = sellBank;
                            }
                            else
                            {
                                recomend = "Hold (Buy)";
                                bank = buyBank;
                            }
                        }
                        else if( todayBuy > oldBuy)
                        if (recomend == null || bank == null) continue;
                        rowData[3] = recomend;
                        rowData[4] = bank;
                        table.Rows.Add(rowData);
                    }
                    else continue;
                }
                else continue;
            }
            table.AcceptChanges();
            return table;
        }

        

        public DataTable GetSellTableData(DateTime date)
        {
            return BSTable("Sell Table", false, date);
        }
    

        public DataTable GetTicketTableData(string bankName, DateTime date)
        {
            string tableName = $"{bankName} ticket for {date.ToShortDateString()}";
            Ticket ticket = _context.Ticket.FirstOrDefault(
                        x => x.BankShortName == bankName && x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day
                    );
            if (ticket==null)
            {
                return ErrorTable(tableName, "There is no ticket for this date");
            }
            DataTable table = new DataTable(tableName);
            table.Columns.Add(new DataColumn("Currency", typeof(string)));
            table.Columns.Add(new DataColumn("Name", typeof(string)));
            table.Columns.Add(new DataColumn("Country", typeof(string)));
            table.Columns.Add(new DataColumn("Quantity", typeof(string)));
            table.Columns.Add(new DataColumn("Buy", typeof(string)));
            table.Columns.Add(new DataColumn("Sell", typeof(string)));
            Dictionary<string, CurrencyMetadata> metadata= _context.CurrencyMetadata.ToDictionary(k=> k.IsoName,v=> v);
            List<Currency> currencies = _context.Currency.Where(x => x.TicketId == ticket.Id).ToList();
            foreach(Currency currency in currencies)
            {
                object[] rowData = new object[table.Columns.Count];
                CurrencyMetadata meta = metadata[currency.IsoName];
                rowData[0] = currency.IsoName;
                rowData[1] = meta.Name ?? "X";
                rowData[2] = meta.Country ?? "X";
                rowData[3] = meta.Quantity;
                rowData[4] = currency.Buy;
                rowData[5] = currency.Sell.HasValue ? currency.Sell.Value.ToString() : "X";

                table.Rows.Add(rowData);
            }
            table.AcceptChanges();
            return table;
        }


        public string GetIntervalBuyDateGraph(string isoName, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public string GetIntervalSellDateGraph(string isoName, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public string GetBuyDateGraph(string isoName, DateTime date)
        {
            return GetDateGraph(isoName, true, date);
        }

        public string GetSellDateGraph(string isoName, DateTime date)
        {
            return GetDateGraph(isoName, false, date);
        }
        #endregion



        private DataTable BSTable(string name,bool IsBuy,DateTime date)
        {
            string tableName = $"{name} for {date.ToShortDateString()}";
            List<Ticket> tickets = _context.Ticket.Where(
                        x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day && x.BankShortName != "CNB"
                    ).ToList();
            if (tickets.Count <= 0)
            {
                //TODO: ADD table with text that data dont exist
                return ErrorTable(tableName, "There is no ticket for this date");

            }

            Dictionary<string, Dictionary<string, float?>> bankCurrency = new Dictionary<string, Dictionary<string, float?>>();
            DataTable table = new DataTable(tableName);
            table.Columns.Add("Currency", typeof(string));
            foreach (Ticket ticket in tickets)
            {
                table.Columns.Add(ticket.BankShortName, typeof(string));
                bankCurrency.Add(ticket.BankShortName, _context.Currency.Where(x => x.TicketId == ticket.Id).ToDictionary(t => t.IsoName, t => IsBuy? t?.Buy: t?.Sell));
            }
            List<string> isoNames = _context.CurrencyMetadata.Select(x => x.IsoName).ToList();
            isoNames.Sort();

            foreach (string isoName in isoNames)
            {
                object[] rowData = new object[bankCurrency.Count + 1];
                rowData[0] = isoName;
                int i = 1;
                foreach (string bank in bankCurrency.Keys)
                {
                    Dictionary<string, float?> bankCurrencies;
                    float? price = null;
                    string priceText = null;
                    if (bankCurrency.TryGetValue(bank, out bankCurrencies))
                    {
                        if (bankCurrencies.TryGetValue(isoName, out price))
                        {
                            priceText = price.ToString();
                        }
                    }
                    if (!price.HasValue)
                    {
                        priceText = "X";
                    }
                    rowData[i] = priceText;

                    //Necessary
                    i++;
                }
                table.Rows.Add(rowData);
            }
            table.AcceptChanges();
            return table;
        }

       

        private string GetDateGraph(string isoName, bool isBuy, DateTime date)
        {
            string action = isBuy ? "Buy" : "Sell";
            string tableName = $"{isoName} {action} for {date.ToShortDateString()}";
            DataTable table = new DataTable(tableName);

            List<Ticket> tickets = _context.Ticket.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day).ToList();
            Dictionary<string, float> values = new Dictionary<string, float>();
            foreach (Ticket ticket in tickets)
            {
                Currency cur = _context.Currency.Where(x => x.TicketId == ticket.Id && x.IsoName == isoName).FirstOrDefault();
                if (cur != null)
                {
                    float value;
                    if (isBuy)
                    {
                        value = cur.Buy;
                    }
                    {
                        if (cur.Sell.HasValue) value = cur.Sell.Value;
                        else continue;
                    }
                    values.Add(ticket.BankShortName, value);
                }
            }
            dynamic jo = new ExpandoObject();
            jo.data = values;
            jo.date = date;
            string json = JsonConvert.SerializeObject(jo);
            return json;
        }


        private DataTable ErrorTable(string tableName, string message)
        {
            DataTable errorTable = new DataTable(tableName);
            errorTable.Columns.Add(new DataColumn("Reason", typeof(string)));
            errorTable.Rows.Add(new object[] { message });
            errorTable.AcceptChanges();
            return errorTable;
        }



        private DataTable CreateRecomendationTable(string tableName, DateTime date, out bool err)
        {
            err = false;
            List<Ticket> tickets = _context.Ticket.Where(
                       x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day && x.BankShortName != "CNB"
                   ).ToList();
            if (tickets.Count <= 0)
            {
                err = true;
                //TODO: ADD table with text that data dont exist
                return ErrorTable(tableName, "There is no ticket for this date");
            }

            Dictionary<string, Dictionary<string, Currency>> data = new Dictionary<string, Dictionary<string, Currency>>();
            DataTable table = new DataTable(tableName);
            table.Columns.Add("Currency", typeof(string));
            table.Columns.Add("Best Buy", typeof(string));
            table.Columns.Add("Bank Buy Name", typeof(string));
            table.Columns.Add("Best Sell", typeof(string));
            table.Columns.Add("Bank Sell Name", typeof(string));

            List<string> isoNames = _context.CurrencyMetadata.Select(x => x.IsoName).ToList();
            foreach (string isoName in isoNames)
            {
                data.Add(isoName, new Dictionary<string, Currency>());
            }

            //preprocessing
            foreach (Ticket ticket in tickets)
            {
                foreach (Currency currency in _context.Currency.Where(x => x.TicketId == ticket.Id).ToList())
                {
                    Dictionary<string, Currency> innerData = null;
                    if (data.TryGetValue(currency.IsoName, out innerData))
                    {
                        innerData.Add(ticket.BankShortName, currency);
                    }
                }
            }


            foreach (string isoName in data.Keys)
            {
                Dictionary<string, Currency> innerData = null;
                if (data.TryGetValue(isoName, out innerData))
                {

                    object[] rowData = new object[table.Columns.Count];
                    rowData[0] = isoName;
                    //max
                    string bestBuyBank = null;
                    float? bestBuy = null;

                    //min
                    string bestSellBank = null;
                    float? bestSell = null;

                    foreach (var (bankName, currency) in innerData)
                    {
                        if (currency != null)
                        {
                            if (!bestBuy.HasValue || bestBuy.HasValue && bestBuy > currency.Buy)
                            {
                                bestBuy = currency.Buy;
                                bestBuyBank = bankName;
                            }

                            if (!bestSell.HasValue || bestSell.HasValue && currency.Sell.HasValue && bestSell < currency.Sell)
                            {
                                bestSell = currency.Sell;
                                bestSellBank = bankName;
                            }
                        }
                    }
                    //Best buy
                    rowData[1] = bestBuy.HasValue ? bestBuy.ToString() : "X";
                    rowData[2] = bestBuyBank ?? "X";

                    //Best Sell
                    rowData[3] = bestSell.HasValue ? bestSell.Value.ToString() : "X";
                    rowData[4] = bestSellBank ?? "X";
                    table.Rows.Add(rowData);
                }
            }
            table.AcceptChanges();
            return table;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BCC.Model.Models;

namespace BCC.Core
{
    public class ExchangeRateManager : IExchangeRateManager, IPresentationManager
    {
       
        private readonly IPresentationManager _presentationManager;

        private readonly BCCContext _context;
         
        public ExchangeRateManager(BCCContext context, IPresentationManager pManager)
        {
            this._presentationManager = pManager;
            this._context = context;
        }
            

        #region Interface methods
       

        DataTable IPresentationManager.GetBestOfDateTableData(DateTime date)
        {
            throw new NotImplementedException();
        }

        DataTable IPresentationManager.GetBuyTableData(DateTime date)
        {
            throw new NotImplementedException();
        }

        DataTable IPresentationManager.GetCurrencyChangeTableData(string bankName, DateTime date)
        {
            throw new NotImplementedException();
        }

        DataTable IPresentationManager.GetCurrencyGraphData(string IsoName, DateTime date)
        {
            throw new NotImplementedException();
        }

        DataTable IPresentationManager.GetRecomendationTableData()
        {
            throw new NotImplementedException();
        }

        DataTable IPresentationManager.GetSellTableData(DateTime date)
        {
            throw new NotImplementedException();
        }

        DataTable IPresentationManager.GetTicketTableData(string bankName, DateTime date)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

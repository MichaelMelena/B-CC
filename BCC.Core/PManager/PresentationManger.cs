using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BCC.Model.Models;

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
            throw new NotImplementedException();
        }

        public DataTable GetBuyTableData(DateTime date)
        {
            throw new NotImplementedException();
        }

        public DataTable GetCurrencyChangeTableData(string bankName, DateTime date)
        {
            throw new NotImplementedException();
        }

        public DataTable GetCurrencyGraphData(string IsoName, DateTime date)
        {
            throw new NotImplementedException();
        }

        public DataTable GetRecomendationTableData()
        {
            throw new NotImplementedException();
        }

        public DataTable GetSellTableData(DateTime date)
        {
            throw new NotImplementedException();
        }

        public DataTable GetTicketTableData(string bankName, DateTime date)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}

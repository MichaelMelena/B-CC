using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace BCC.Core
{
    public interface IPresentationManager
    {
        DataTable GetSellTableData(DateTime date);

        DataTable GetBuyTableData(DateTime date);

        DataTable GetBestOfDateTableData(DateTime date);

        DataTable GetRecomendationTableData();

        DataTable GetTicketTableData(string bankName,DateTime date);

        DataTable GetCurrencyChangeTableData(string bankName, DateTime date);

        DataTable GetCurrencyGraphData(string IsoName, DateTime date);
    }
}

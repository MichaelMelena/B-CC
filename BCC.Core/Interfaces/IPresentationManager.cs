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

        DataTable GetRecomendationTableData(DateTime date);

        DataTable GetTicketTableData(string bankName,DateTime date);

        DataTable GetCurrencyChangeTableData(string bankName, DateTime date);

        DataTable SingleCurrencyRecommendation(string isoName);

        string GetIntervalBuyDateGraph(string isoName, DateTime start, DateTime end);

        string GetIntervalSellDateGraph(string isoName, DateTime start, DateTime end);

        string GetBuyDateGraph(string isoName, DateTime date);

        string GetSellDateGraph(string isoName, DateTime date);
    }
}

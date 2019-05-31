using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCC.Model.Models;
using BCC.Core;
using Microsoft.Extensions.Logging;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Web.Controllers
{
    public class TicketTableController : Controller
    {
        
        enum TableTypes { Buy, Sell, Ticket, Best, Recommendation, Change, TimelineBuy, TimelineSell}

        #region Dependencies
        private readonly BCCContext _context;
        private readonly IPresentationManager _presentationManager;
        private readonly ILogger<TicketTableController> _logger;
        #endregion

        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public TicketTableController(BCCContext context, IPresentationManager presentationManager, ILogger<TicketTableController> logger)
        {
            _logger = logger;
            _context = context;
            _presentationManager = presentationManager;
        }


        #region tables
        [HttpGet]
        public PartialViewResult BuyTable([FromQuery]DateTime tableDate)
        {
           
            
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;

            ViewBag.tableType = TableTypes.Buy.ToString();
            ViewBag.tableDate = tableDate.ToString("yyyy-MM-dd");
            ViewBag.bankName = null;
            ViewBag.currency = null;

            DataTable table = _presentationManager.GetBuyTableData(date: tableDate);
            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult SellTable([FromQuery]DateTime tableDate)
        {
            
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;

            ViewBag.tableType = TableTypes.Sell.ToString();
            ViewBag.tableDate = tableDate.ToString("yyyy-MM-dd");
            ViewBag.bankName = null;
            ViewBag.currency = null;

            DataTable table = _presentationManager.GetSellTableData(tableDate);
            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult BestOfDateTable([FromQuery]DateTime tableDate)
        {
            
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;
            ViewBag.tableType = TableTypes.Best.ToString();
            ViewBag.tableDate = tableDate.ToString("yyyy-MM-dd");
            ViewBag.bankName = null;
            ViewBag.currency = null;

            DataTable table = _presentationManager.GetBestOfDateTableData(tableDate);
            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult RecommendationTable([FromQuery]DateTime tableDate)
        {

            
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;
            ViewBag.tableType = TableTypes.Recommendation.ToString();
            ViewBag.tableDate = tableDate.ToString("yyyy-MM-dd");
            ViewBag.bankName = null;
            ViewBag.currency = null;

            DataTable table = _presentationManager.GetRecomendationTableData(tableDate);
            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult TicketTable([FromQuery]string bankName, [FromQuery]DateTime tableDate)
        {
            
            if (tableDate == null) tableDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(bankName)) bankName = "CNB";

            ViewBag.tableType = TableTypes.Ticket.ToString();
            ViewBag.tableDate = tableDate.ToString("yyyy-MM-dd");
            ViewBag.bankName = bankName;
            ViewBag.currency = null;
            DataTable table = _presentationManager.GetTicketTableData(bankName, tableDate);
            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult CurrencyChangeTable(string bankName,DateTime tableDate )
        {
            
            _presentationManager.GetCurrencyChangeTableData(bankName, tableDate);

            ViewBag.tableType = TableTypes.Change.ToString();
            ViewBag.tableDate = tableDate.ToString("yyyy-MM-dd");
            ViewBag.bankName = bankName;
            ViewBag.currency = null;
            return PartialView();
        }
        #endregion

        #region excels

        
        public IActionResult DownloadExcelTable(string tableType,DateTime tableDate,string bankName, string currency, int interval=7)
        {
            if(Enum.TryParse<TableTypes>(tableType,true,out TableTypes type))
            {
                var start = DateTime.Today.Subtract(TimeSpan.FromDays(interval));
                var end = DateTime.Today;
                DataTable dataTable = null;
                switch (type)
                {
                    case TableTypes.Buy:
                        dataTable = _presentationManager.GetBuyTableData(tableDate);
                        break;
                    case TableTypes.Sell:
                        dataTable = _presentationManager.GetSellTableData(tableDate);
                        break;
                    case TableTypes.Ticket:
                        dataTable = _presentationManager.GetTicketTableData(bankName, tableDate);
                        break;
                    case TableTypes.Best:
                        dataTable = _presentationManager.GetBestOfDateTableData(tableDate);
                        break;
                    case TableTypes.Recommendation:
                        dataTable = _presentationManager.GetRecomendationTableData(tableDate);
                        break;
                    case TableTypes.TimelineBuy:
                       
                        dataTable = CreateTimelineDataTable(start,end, currency, true);
                        break;
                    case TableTypes.TimelineSell:
                      
                        dataTable = CreateTimelineDataTable(start, end, currency, false);
                        break;

                }
                return CreateExcelTable(dataTable.TableName, tableType, dataTable);
            }
            return NoContent();
            
        }

        private FileContentResult  CreateExcelTable(string title, string type, DataTable dataTable)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(type);
                worksheet.Cells["A1"].LoadFromDataTable(dataTable, PrintHeaders: true);
                for (var col = 1; col < dataTable.Columns.Count + 1; col++)
                {
                    worksheet.Column(col).AutoFit();
                }
                return File(package.GetAsByteArray(), XlsxContentType, $"{title}.xlsx");
            }
        }
        #endregion

        private DataTable CreateTimelineDataTable(DateTime start, DateTime end, string currency, bool isBuy)
        {
            TimelineDatasetModel model = _presentationManager.CreateTimelineDataset(start,end,currency,isBuy);
            var table = new DataTable($"Timeline table from {start} to {end}");
            table.Columns.Add("Date", typeof(string));
            
            foreach( string bank in model.Dataset.Keys)
            {
                table.Columns.Add(bank, typeof(double));
            }

            table.AcceptChanges();

            foreach (DateTime date in model.Labels)
            {
                string stringDate = date.ToString("yyyy-MM-dd");
                DataRow row = table.NewRow();

                row[0] = date.ToShortDateString();
                foreach (string bank in model.Dataset.Keys)
                {
                    if (model.Dataset[bank].ContainsKey(stringDate))
                    {
                        row[bank] = model.Dataset[bank][stringDate];
                    }
                    else
                    {
                        row[bank] = Double.NaN;
                    }
                    
                }
                table.Rows.Add(row);
            }

            table.AcceptChanges();
            return table;
        }
    }
}
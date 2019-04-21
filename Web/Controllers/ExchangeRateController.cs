using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCC.Model.Models;
using BCC.Core;
using System.Data;
using System.Reflection;
using  Microsoft.Extensions.Logging;


namespace Web.Controllers
{
    public class ExchangeRateController : Controller
    {
        #region Dependencies
        private readonly BCCContext _context;
        private readonly IPresentationManager _presentationManager;
        private readonly ILogger<ExchangeRateController> _logger;
        #endregion

        private readonly string _version;
        public ExchangeRateController(BCCContext context, IPresentationManager presentationManager, ILogger<ExchangeRateController> logger)
        {
            _logger = logger;
            _context = context;
           _presentationManager = presentationManager;
            _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public IActionResult Index()
        {
            ViewBag.version = _version;
            ViewBag.title = "Exchange Rates";
            return View();
        }

        public PartialViewResult BuyTable([FromQuery]DateTime tableDate)
        {
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;

            DataTable table =  _presentationManager.GetBuyTableData(date: tableDate);

            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        public PartialViewResult SellTable([FromQuery]DateTime tableDate)
        {
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;
            DataTable table = _presentationManager.GetSellTableData(tableDate);

            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        public PartialViewResult BestOfDateTable([FromQuery]DateTime tableDate)
        {
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;
            DataTable table = _presentationManager.GetBestOfDateTableData(tableDate);
            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        public PartialViewResult RecomendationTable([FromQuery]DateTime tableDate)
        {
            
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;

            DataTable table = _presentationManager.GetRecomendationTableData(tableDate);
            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        
        public PartialViewResult TicketTable([FromBody]string bankName, [FromQuery]DateTime tableDate)
        {
            if (tableDate == null) tableDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(bankName)) bankName = "CNB";

            DataTable table = _presentationManager.GetTicketTableData(bankName, tableDate);
            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        public PartialViewResult CurrencyChangeTable(string bankName)
        {
            return PartialView();
        }

        public ContentResult CurrencyGraph(string currency)
        {
            string json = _presentationManager.GetBuyDateGraph("AUD", new DateTime(2019, 3, 21));
           
            return Content(content: json, contentType: "application/json");
        }
    }
}
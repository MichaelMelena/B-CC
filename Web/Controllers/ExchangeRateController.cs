using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCC.Model.Models;
using BCC.Core;
using System.Data;


namespace Web.Controllers
{
    public class ExchangeRateController : Controller
    {
        private readonly BCCContext _context;
        private readonly IPresentationManager _presentationManager;
        public ExchangeRateController(BCCContext context, IPresentationManager presentationManager)
        {
            this._context = context;
            this._presentationManager = presentationManager;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public PartialViewResult BuyTable()
        {

            DataTable table =  _presentationManager.GetBuyTableData(new DateTime(2019, 3, 21));

            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        public PartialViewResult SellTable()
        {
            DataTable table = _presentationManager.GetSellTableData(DateTime.Now);

            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        public PartialViewResult BestOfDateTable()
        {
            DataTable table = _presentationManager.GetBestOfDateTableData(new DateTime(2019, 3, 20));
            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        public PartialViewResult RecomendationTable()
        {

            DataTable table = _presentationManager.GetRecomendationTableData(new DateTime(2019, 3, 21));
            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        public PartialViewResult TicketTable(string bankName)
        {
            DataTable table = _presentationManager.GetTicketTableData(bankName,new DateTime(2019, 3, 21));
            return PartialView(viewName: "~/Views/ExchangeRate/Table.cshtml", model: table);
        }

        public PartialViewResult CurrencyChangeTable(string bankName)
        {
            return PartialView();
        }

        public PartialViewResult CurrencyGraph(string currency)
        {
            return PartialView( );
        }
    }
}
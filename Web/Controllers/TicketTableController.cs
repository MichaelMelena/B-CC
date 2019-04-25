using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCC.Model.Models;
using BCC.Core;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Web.Controllers
{
    public class TicketTableController : Controller
    {

        #region Dependencies
        private readonly BCCContext _context;
        private readonly IPresentationManager _presentationManager;
        private readonly ILogger<TicketTableController> _logger;
        #endregion

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

            DataTable table = _presentationManager.GetBuyTableData(date: tableDate);

            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult SellTable([FromQuery]DateTime tableDate)
        {
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;
            DataTable table = _presentationManager.GetSellTableData(tableDate);

            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult BestOfDateTable([FromQuery]DateTime tableDate)
        {
            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;
            DataTable table = _presentationManager.GetBestOfDateTableData(tableDate);
            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult RecommendationTable([FromQuery]DateTime tableDate)
        {

            if (tableDate == DateTime.MinValue) tableDate = DateTime.Now;

            DataTable table = _presentationManager.GetRecomendationTableData(tableDate);
            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult TicketTable([FromQuery]string bankName, [FromQuery]DateTime tableDate)
        {
            if (tableDate == null) tableDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(bankName)) bankName = "CNB";

            DataTable table = _presentationManager.GetTicketTableData(bankName, tableDate);
            return PartialView(viewName: "~/Views/TicketTable/Table.cshtml", model: table);
        }

        [HttpGet]
        public PartialViewResult CurrencyChangeTable(string bankName)
        {
            return PartialView();
        }
        #endregion
    }
}
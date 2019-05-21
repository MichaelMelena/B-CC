﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCC.Model.Models;
using BCC.Core;
using System.Reflection;
using  Microsoft.Extensions.Logging;
using Web.Models;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Web.Controllers
{
    public class ExchangeRateController : Controller
    {
        #region Dependencies
        private readonly BCCContext _context;
        private readonly IPresentationManager _presentationManager;
        private readonly ILogger<ExchangeRateController> _logger;
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
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
        public IActionResult BankTickets()
        {
            ViewBag.version = _version;
            List<BankConnector> banks = _context.BankConnector.ToList();
            return View(viewName: "~/Views/ExchangeRate/BankTickets.cshtml", model: banks);
        }

        public IActionResult BestTickets()
        {
            ViewBag.version = _version;
            return View(viewName: "~/Views/ExchangeRate/BestTickets.cshtml");
        }

        public IActionResult Recommendations()
        {
            ViewBag.version = _version;
            return View(viewName: "~/Views/ExchangeRate/Recommendations.cshtml");
        }

        public IActionResult Wholesales()
        {
            ViewBag.version = _version;
            return View(viewName: "~/Views/ExchangeRate/Wholesales.cshtml");
        }

        [HttpGet]
        public IActionResult MixTickets()
        {
            ViewBag.version = _version;
            List<CurrencyMetadata> meta = _context.CurrencyMetadata.ToList();
            List<BankConnector> banks = _context.BankConnector.ToList();
            return View(viewName: "~/Views/ExchangeRate/MixTickets.cshtml",model: Tuple.Create(banks,meta));
        }

        [HttpGet]
        public IActionResult Overview()
        {
            ViewBag.version = _version;
            List<CurrencyMetadata> meta = _context.CurrencyMetadata.ToList();
            return View(viewName: "~/Views/ExchangeRate/Overview.cshtml", model: meta);
        }



       [HttpGet]
        public IActionResult CurrencyPrice()
        {
            ViewBag.version = _version;
            
            List<CurrencyMetadata> meta = _context.CurrencyMetadata.ToList();
            return View(viewName: "~/Views/ExchangeRate/CurrencyPrice.cshtml", model: meta);
        }

        [HttpGet]
        public IActionResult CurrencyTimeline()
        {
            List<CurrencyMetadata> meta = _context.CurrencyMetadata.ToList();
            return View(viewName: "~/Views/ExchangeRate/CurrencyTimeline.cshtml", model: meta);
        }


        [HttpGet]
        public PartialViewResult OverviewControl([FromQuery]string isoName="AUD",[FromQuery] int interval=7,[FromQuery] bool isBuy=true)
        {



            ViewBag.tableType = isBuy ? "Buy" : "Sell";
           var overviewModel = new OverviewModel() { Currency = isoName, Interval = interval, IsBuy = isBuy };
            var table = _presentationManager.SingleCurrencyRecommendation(isoName);
            return PartialView(viewName: "~/Views/ExchangeRate/OverviewControl.cshtml", model: new Tuple<DataTable,OverviewModel>(table,overviewModel));
        }
       

       
        

    }
}
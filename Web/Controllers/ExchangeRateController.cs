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
        public IActionResult BankTickets()
        {
            ViewBag.version = _version;
            return View(viewName: "~/Views/ExchangeRate/BankTickets.cshtml");
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
        #region Partial views

        [HttpGet]
        public PartialViewResult TicketPanel([FromQuery] bool includeBank = false)
        {
            ViewBag.includeBank = includeBank;
            List<BankConnector> availaibleBanks = null;
            if (includeBank) {
                availaibleBanks = _context.BankConnector.Where(x => x.Enabled == true).ToList();
            }
            return PartialView(viewName: "~/Views/ExchangeRate/TicketPanel.cshtml", model: availaibleBanks);
        }
        
       [HttpGet]
       public IActionResult CurrencyDifference()
        {
            ViewBag.version = _version;
            return View(viewName: "~/Views/ExchangeRate/CurrencyDifference.cshtml");
        }
        #endregion

        
    }
}
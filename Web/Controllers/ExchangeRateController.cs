using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCC.Model.Models;
using BCC.Core;


namespace Web.Controllers
{
    public class ExchangeRateController : Controller
    {
        public readonly BCCContext _context;

        public ExchangeRateController(BCCContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public PartialViewResult BuyTable()
        {
            Ticket ticket = _context.Ticket.FirstOrDefault();
            List<Currency> currency = _context.Currency.Where(x => x.TicketId == ticket.Id).ToList();
            List<CurrencyMetadata> currencyMetadata = _context.CurrencyMetadata.ToList();
            ExchangeRateTicket erTicket = new ExchangeRateTicket(ticket.Date);

            foreach (var cur in currency)
            {
                CurrencyMetadata meta = currencyMetadata.FirstOrDefault(x => x.IsoName == cur.IsoName);
                erTicket.AddExchangeRateData(new ERDataBase(cur.IsoName, meta.Name, meta.Country, meta.Quantity, cur.Buy, cur.Sell));

            }


            return PartialView(new List<ExchangeRateTicket>() { erTicket });
        }

        public PartialViewResult SellTable()
        {
            return PartialView();
        }

        public PartialViewResult BestOfTodayTable()
        {
            return PartialView();
        }

        public PartialViewResult RecomendationTable()
        {
            return PartialView();
        }

        public PartialViewResult TicketTable(string bankName)
        {
            return PartialView();
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
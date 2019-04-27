using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BCC.Core;
using BCC.Model.Models;

namespace Web.Controllers
{
    public class TicketGraphController : Controller
    {
        #region Dependencies
        private readonly BCCContext _context;
        private readonly IPresentationManager _presentationManager;
        private readonly ILogger<TicketGraphController> _logger;
        #endregion

        public TicketGraphController(BCCContext context, IPresentationManager presentationManager, ILogger<TicketGraphController> logger)
        {
            _logger = logger;
            _context = context;
            _presentationManager = presentationManager;
        }


        [HttpGet]
        public ContentResult CurrencyGraph(string currency)
        {
            string json = _presentationManager.GetBuyDateGraph("AUD", new DateTime(2019, 3, 21));

            return Content(content: json, contentType: "application/json");
        }

        [HttpGet]
        public PartialViewResult GetGraph()
        {
            return PartialView(viewName: "~/Views/TicketGraph/Graph.cshtml");
        }

        [HttpGet]
        public JsonResult CurrencyPriceGraphData([FromQuery] string currency, [FromQuery]DateTime graphDate, [FromQuery]bool isBuy = true)
        {
            if (IsDateInValid(ref graphDate)) graphDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(currency)) currency = "AUD";
            var tickets = _context.Ticket.Where(x => x.Date.Year == graphDate.Year && x.Date.Month == graphDate.Month && x.Date.Day == graphDate.Day);
            List<string> bankNames = new List<string>();
            List<double> bankValues = new List<double>();
            foreach (var ticket in tickets)
            {

                Currency selectedCurrency = _context.Currency.Where(x => x.TicketId == ticket.Id && x.IsoName == currency.ToUpperInvariant()).FirstOrDefault();
                if (selectedCurrency == null) continue;
                if (isBuy)
                {
                    bankNames.Add(ticket.BankShortName);
                    bankValues.Add(selectedCurrency.Buy);
                }
                else if (selectedCurrency.Sell.HasValue)
                {
                    bankNames.Add(ticket.BankShortName);
                    bankValues.Add(selectedCurrency.Sell.Value);
                }
            }
            return Json(new { bankNames, bankValues, currency, date = graphDate, isBuy });
        }

        [HttpGet]
        public JsonResult CurrencyTimelineGraphData([FromQuery] string currency, [FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] bool isBuy = false)
        {
            //if (IsDateInValid(ref start)) start = DateTime.Now;
            //if (IsDateInValid(ref end)) end = DateTime.Now;
            if (start == DateTime.MinValue || end == DateTime.MinValue || start > end)
            { 
                start = DateTime.Now;
                end = DateTime.Now;
            }
            if (string.IsNullOrWhiteSpace(currency)) currency = "AUD";

            //List<Ticket> tickets = _context.Ticket.Where(x => x.Date.Year >= start.Year && x.Date.Month >= start.Month && x.Date.Day >= start.Day && x.Date.Year <= end.Year && x.Date.Month <= end.Month && x.Date.Day <= end.Day).ToList();
            
           
            var bankNames = _context.BankConnector.Select(x => x.BankShortName).ToList();
            var dataset = new Dictionary<string, Dictionary<string,double>>();
            HashSet<DateTime> labels = new HashSet<DateTime>();
            foreach (string name in bankNames)
            {
                dataset.Add(name, new Dictionary<string, double>());
                var tickets = _context.Ticket.Where(x => x.Date >= start && x.Date <= end && x.BankShortName == name).ToList();
                tickets.Sort(new TicketDateComparer());
                foreach (var ticket in tickets)
                {
                    Currency selectedCurrency = _context.Currency.FirstOrDefault(x => x.TicketId == ticket.Id && x.IsoName == currency.ToUpperInvariant());
                    if (selectedCurrency != null)
                    {
                        if (isBuy)
                        {
                            dataset[ticket.BankShortName].Add(ticket.Date.ToString("yyyy-MM-dd"),selectedCurrency.Buy);
                            
                        }
                        else
                        {
                            if (selectedCurrency.Sell.HasValue)
                            {
                                dataset[ticket.BankShortName].Add(ticket.Date.ToString("yyyy-MM-dd"), selectedCurrency.Sell.Value);
                               
                            }
                        }
                    }
                    labels.Add(ticket.Date);
                }
            }
            var labelList = labels.ToList();
            labelList.Sort();
            return new JsonResult(new { labels=labelList, dataset,currency,isBuy,start,end });
        }
        private bool IsDateInValid(ref DateTime date)
        {
            return (date == DateTime.MinValue || date > DateTime.Now);

        }

        private class TicketDateComparer : IComparer<Ticket>
        {
            public int Compare(Ticket x, Ticket y)
            {
               return x.Date.CompareTo(y.Date);
            }
        }
    }
}
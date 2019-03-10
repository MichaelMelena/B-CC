using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using BCC.Model.Models;
using BCC.Core.CNB;
namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            CNBank bank = new CNBank();
            bank.DownloadTicketForDate(DateTime.Now);
            BCCContext context = new BCCContext();
            Visit visit = context.Visit.First<Visit>();
            visit.Count += 1;
            context.SaveChanges();
            ViewData["visit"] = visit.Count;
            return View(visit);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

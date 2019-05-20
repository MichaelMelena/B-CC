using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using NLog.Common;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
namespace Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _version;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
           _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        
        public IActionResult Index()
        {

            HttpContext.Session.SetString(HttpContext.Session.Id, HttpContext.Session.Id);
            ViewBag.session = HttpContext.Session.GetString(HttpContext.Session.Id);
            ViewBag.version = _version;
            ViewBag.title = "Homepage";
            return View();
            
           
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewBag.version = _version;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

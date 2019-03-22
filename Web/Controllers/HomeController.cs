using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using BCC.Model.Models;
using BCC.Core;
using System.Reflection;
namespace Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _version;

        public HomeController()
        {
           _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        public IActionResult Index()
        {
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

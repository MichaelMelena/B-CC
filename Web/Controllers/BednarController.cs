using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.Text;
using BCC.Model.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace Web.Controllers
{
    public class BednarController : Controller
    {
        static readonly Regex trimmer = new Regex(@"\s\s+",RegexOptions.Compiled);
        private readonly BCCContext _context;
        public BednarController(BCCContext context)
        {
            _context = context;
        }
        public IActionResult Index(int number=5)
        {
            if (number < 1)
            {
                number = 1;
            }
            List<Bednar> bednars = _context.Bednar.OrderByDescending(x => x.Date).Take(number).ToList();
            return View(model: bednars);
        }

        [HttpPost]
        public ContentResult Test([FromBody] JObject content)
        {
            string data = trimmer.Replace(content.ToString(),"");
            _context.Bednar.Add(new Bednar() {
                Date = DateTime.Now,
                Json = data
            });
            _context.SaveChanges();
            Root responseObject = new Root()
            {
                date = DateTime.Now,
                message = "This is placeholder JSON",
            };

            string json = JsonConvert.SerializeObject(responseObject);
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            return Content(json, "application/json",Encoding.UTF8);
        }
        private class Root
        {
            public DateTime date { get; set; }
            public string message { get; set; }

        }
    }
}
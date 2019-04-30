using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class UtillitiesController : Controller
    {

        [HttpGet("Utillity/")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            return View(viewName: "~/Views/Utillities/DayDivision.cshtml");
        }

        [HttpGet("Utillity/DayDivisionComponent")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult DayDivisionComponent([FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery] decimal value)
        {
            if (start == null || start == DateTime.MinValue) start = DateTime.Now;
            if (end == null || end == DateTime.MinValue) end = DateTime.Now;
            int days = (end - start).Days + 1;
            var model = new DayDivisionModel()
            {
                Start = start,
                End = end,
                Value = value,
                Days = days,
            };
            if (days > 0)
            {
                decimal dailyAverage = value / days;
                model.Average = dailyAverage;
                var result = DayDivision.YearlyIntervals(start, end);
                model.Data = new List<DayDivisionData>(result.Count);
                foreach (DayDivision.DayDivisionSpan span in result)
                {
                    model.Data.Add(new DayDivisionData()
                    {
                        From = span.From,
                        To = span.To,
                        Value = (int)Math.Abs((span.From - span.To).Days + 1) * dailyAverage,

                    });
                }
            }
            else
            {
                model.Error = $"Span between the {start} and {end} is lower than a day.";
            }
            return PartialView(model: model, viewName: "~/Views/Utillities/DayDivisionComponent.cshtml");
        }

        public class DayDivision
        {

            public DayDivision(DateTime start, DateTime end, decimal value)
            {
                int days = end.Subtract(start).Days;

            }
            public static List<DayDivisionSpan> YearlyIntervals(DateTime start, DateTime end)
            {
                List<DayDivisionSpan> spans = new List<DayDivisionSpan>();
                if (end < start) throw new ArgumentException("End time can not be lower than start.");

                while (new DateTime((long)Math.Abs((end - start).Ticks)).Year - 1 >= 1)
                {

                    spans.Add(new DayDivisionSpan()
                    {
                        From = start,
                        To = new DateTime(start.Year, 12, 31)
                    });
                    start = new DateTime(start.Year + 1, 1, 1);
                }
                spans.Add(new DayDivisionSpan()
                {
                    From = start,
                    To = end,
                });

                return spans;
            }

            public class DayDivisionSpan
            {
                public DateTime From { get; set; }
                public DateTime To { get; set; }
            }

        }
        public class DayDivisionModel
        {
            public string Error { get; set; }
            public bool IsOk => Error == null;

            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public decimal Value { get; set; }
            public int Days { get; set; }
            public decimal Average { get; set; }
            public List<DayDivisionData> Data { get; set; }

        }

        public class DayDivisionData
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public decimal Value { get; set; }
        }
    }
}
using System;
using System.Text;
using System.Reflection;
using BCC.Core.CNB;
using BCC.Core;
using System.IO;
using System.Collections.Generic;

namespace CodeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt1 = new DateTime(2019, 4, 18);
            DateTime dtE1 = new DateTime(2017, 10, 23);
            DateTime dtE2 = new DateTime(2020, 12, 20);
            DateTime dt2 = new DateTime(2019, 4, 23);
            ExchangeRateTicket ticket = new CNBank().DownloadTodaysTicket();
            ExchangeRateTicket ticket1 = new CNBank().DownloadTicketForDate(dt1);
            List<ExchangeRateTicket> ticket2 = new CNBank().DownloadTicketForInterval(dt1, dt2);
            List<ExchangeRateTicket> ticket3 = new CNBank().DownloadAllTickets();
            Console.ReadKey();
        }
    }
}

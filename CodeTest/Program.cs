using System;
using System.Text;
using System.Reflection;
using BCC.Core.KB;
using BCC.Core;
using System.IO;
using System.Collections.Generic;

namespace CodeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt1 = new DateTime(2019, 01, 01);
            DateTime dtE1 = new DateTime(2017, 10, 23);
            DateTime dtE2 = new DateTime(2020, 12, 20);
            DateTime dt2 = new DateTime(2019, 02, 03);
            ExchangeRateTicket ticket = new KBank().DownloadTodaysTicket();
            ExchangeRateTicket ticket1 = new KBank().DownloadTicketForDate(dt1);
            List<ExchangeRateTicket> ticket2 = new KBank().DownloadTicketForInterval(dt1, dt2);
            List<ExchangeRateTicket> ticket3 = new KBank().DownloadAllTickets();
            Console.ReadKey();
        }
    }
}

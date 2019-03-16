using System;
using System.Text;
using System.Reflection;
using BCC.Core.CNB;
using BCC.Core;
using System.IO;

namespace CodeTest
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //new CNBank().GetCurrencyInfo();
            ExchangeRateManager.Instance.DownloadTodaysTicket();
        }
        private static int[] YearsInterval(DateTime start, DateTime end)
        {

            int difference = end.Year - start.Year;
            int[] array = new int[difference + 1];
            for (int index = 0; index < difference + 1; index++)
            {
                array[index] = start.Year + index;
            }
            return array;
        }
        private static void ReflectionExample()
        {
            var currnet = Assembly.GetExecutingAssembly();
            var dir = Directory.GetCurrentDirectory();
            Assembly assembly = Assembly.LoadFrom($"{dir}\\BCC.Core.dll");
            Type type = assembly.GetType("BCC.Core.CNB.CNBank");
            IExchangeRateBank myClass = (IExchangeRateBank)Activator.CreateInstance(type);
            ExchangeRateTicket ticket = myClass.DownloadTodaysTicket();
            Console.ReadKey();
        }
    }
    public class MyClass
    {
        public string Text { get; set; }
        public MyClass()
        {
            Text = "hello reflection";
        }
    }
}

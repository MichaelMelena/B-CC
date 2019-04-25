using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using BCC.Core;
namespace BCC.Test
{
    class MockWebClient  : IWebClient 
    {
        public  string DownloadString(string url)
        {
            return "hello";
        }
        public void Dispose()
        {

        }
    }
}

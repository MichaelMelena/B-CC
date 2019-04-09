using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public interface IWebClient :IDisposable
    {
        string DownloadString(string url);
    }
}

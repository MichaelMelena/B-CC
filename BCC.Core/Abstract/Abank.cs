using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
namespace BCC.Core.Abstract
{
    public class ABank : IDisposable
    {
        protected IWebClient _webClient;
        public IWebClient BankWebClient {
            get {
                if (_webClient == null) _webClient = new WebClientWraper();
                return _webClient;
            }
            set { _webClient = value; }
        }

        public void Dispose()
        {
            _webClient.Dispose();
        }

        protected virtual string DownloadTicketText(string url)
        {
            try
            {   
                string responseText = null;
                BankWebClient.DownloadString(url);
                if (responseText == null) throw new BCCWebclientException($"For url: {url}: response text is NULL");
                return responseText;
            }
            catch (ArgumentNullException ex)
            {
                throw new BCCWebclientException($"For url: {url}", ex);
            }
            catch (WebException ex)
            {
                throw new BCCWebclientException($"For url: {url}", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new BCCWebclientException($"For url: {url}", ex);
            }
        }
    }
}

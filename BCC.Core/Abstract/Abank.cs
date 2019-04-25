using System;

using System.Net;
using  Microsoft.Extensions.Logging;
using BCC.Core.Extensions;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace BCC.Core.Abstract
{
    
    public class ABank <T>: IDisposable
    {
        
        protected ILogger<T> Logger { get; set; }
       
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


        /// <summary>
        /// Returns not null response from requested url
        /// </summary>
        /// <param name="url">Requsted url</param>
        /// <returns>Reutrns <c>string</c> content obtained from specified url</returns>
        /// <exception cref="BCCWebclientException"></exception>
        protected virtual string DownloadTicketText(string url)
        {
            try
            {
                string responseText = BankWebClient.DownloadString(url);
                return (responseText != null)? responseText : throw new BCCWebclientException($"Request returned null for url: '{url}'");
            }
            catch (WebException ex)
            {
                using (StreamReader sr = new StreamReader(((HttpWebResponse)(ex).Response).GetResponseStream()))
                {
                    var message = sr.ReadToEnd();
                    throw new BCCWebclientException(message, ex);
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new BCCWebclientException("Url is null", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new BCCWebclientException("Invallid operation was called on web-client", ex);
            }
        }
    }
}

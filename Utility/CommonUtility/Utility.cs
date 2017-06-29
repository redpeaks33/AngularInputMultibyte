using HtmlAgilityPack;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;

namespace CommonUtility
{
    public static class Utility
    {
        public static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static Stopwatch sw = new Stopwatch();
        public static string DataBaseName = "[WordChunk]";

        public static string GetDateTimeNow()
        {
            return DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        public static string GetClientIpAddress(this System.Net.Http.HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            //else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            //{
            //    RemoteEndpointMessageProperty prop;
            //    prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}
            else
            {
                return "";
            }
        }

        public static HtmlDocument DownLoadHtml(string url)
        {
            Stream stream = null;
            StreamReader streamReader = null;
            HtmlDocument htmlDocument = null;

            sw.Restart();
            try
            {
                stream = new MyWebClient().OpenRead(url);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return htmlDocument ;
            }

            try
            {
                if (stream == null)
                {
                    return null;
                }

                streamReader = new System.IO.StreamReader(stream, System.Text.Encoding.GetEncoding(65001));
                htmlDocument = new HtmlAgilityPack.HtmlDocument();

                if (htmlDocument == null)
                {
                    return null;
                }

                htmlDocument.LoadHtml(streamReader.ReadToEnd());
                streamReader.Close();
                stream.Close();
                sw.Stop();
            }
            catch (Exception e)
            {
                sw.Stop();
                Utility.logger.Debug(sw.Elapsed);
                Utility.logger.Debug(e.Message);
                streamReader.Close();
                stream.Close();
                //If There is not html page.
                throw e;
            }

            return htmlDocument;
        }

        public class MyWebClient : System.Net.WebClient
        {
            private int timeout = 3000;

            public int Timeout
            {
                get
                {
                    return timeout;
                }
                set
                {
                    timeout = value;
                }
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var w = base.GetWebRequest(address);
                w.Timeout = timeout;
                return w;
            }
        }
    }
}
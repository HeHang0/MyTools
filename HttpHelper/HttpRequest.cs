using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpHelper
{
    class HttpRequest
    {
        public static string GetHttpResponse(string url, string method = "GET",string paramData="", string referer = "", string userAgent = "", string contentType = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (string.IsNullOrEmpty(referer))
            {
                request.Referer = "http://y.qq.com";
            }
            else
            {
                request.Referer = referer;
            }
            request.Method = method;
            if (!string.IsNullOrEmpty(contentType))
            {
                request.ContentType = contentType;
            }
            if (!string.IsNullOrEmpty(paramData))
            {
                var a = Encoding.UTF8.GetBytes(paramData);
                request.ContentLength = a.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(a, 0, a.Length);
                }
            }
            if (string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36";
            }
            else
            {
                request.UserAgent = userAgent;
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
    }
}

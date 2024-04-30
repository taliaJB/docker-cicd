using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using Eldan.TypeExtensions;
using System.Collections.Specialized;


namespace Eldan.Protocols
{
    /// <summary>
    /// Summary description for BodyParams
    /// </summary>
    public class BodyParams
    {
        JObject m_JO;

        public BodyParams(HttpRequest Request) : this(GetDocumentContents(Request))
        {
        }

        public BodyParams(string JSON)
        {
            if (string.IsNullOrWhiteSpace(JSON))
                JSON = "{}";

            m_JO = JObject.Parse(JSON);
        }

        public string GetParamVal(string key)
        {
            return (string)m_JO[key];
        }

        public override string ToString()
        {
            return m_JO.ToString();
        }

        static private string GetDocumentContents(HttpRequest Request)
        {
            string documentContents;
            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }
    }

    public static class RequestExtensions
    {
        public static void ValidateQSParameter<T>(this HttpRequest Request, string paramName)
        {
            string Res = Request.QueryString[paramName];

            if (Res == null)
                throw new Exception(string.Format("QS parameter: '{0}' is not exists", paramName));
            if (!Res.TryParseType<T>(out _))
                throw new Exception(string.Format("QS parameter: '{0}' is not type: '{1}' as required", paramName, typeof(T).ToString()));
        }

        public static string ToQueryString(this NameValueCollection nvc, string url)
        {
            return HTTP.ToQueryString(nvc, url);
        }

        public static string ToQueryString(this NameValueCollection nvc)
        {
            return HTTP.ToQueryString(nvc);
        }

        public static string ToFormString(this NameValueCollection nvc)
        {
            return HTTP.ToFormString(nvc);
        }

        public static string GetParamValueFromQS(this string url, string paramName)
        {
            Uri URI;
            try
            {
                URI = new Uri(url);
            }
            catch (Exception)
            {
                return null;
            }
            return HttpUtility.ParseQueryString(URI.Query).Get(paramName);
        }
    }
}
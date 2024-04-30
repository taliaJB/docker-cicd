using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Collections.Specialized;
using static System.Net.Mime.MediaTypeNames;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.Threading;

namespace Eldan.Protocols
{
    public class HTTP
    {
        #region Constants and Types
        public const string REQUEST_TYPE = "application/json";
        private const string USER_AGENT = "Eldan Client";

        public enum enmMethods
        {
            GET,
            HEAD,
            POST,
            PUT,
            DELETE,
            CONNECT,
            OPTIONS,
            TRACE,
            PATCH
        }

        public struct Header
        {
            public string Name;
            public string Value;
        }

        public enum enmAuthenticationType
        {
            Basic,
            NTLM,
            OAuth2
        }

        public struct Cridential
        {
            public bool UseDefaultNetworkCredentials;
            public string UserName;
            public string Password;
            public enmAuthenticationType AuthenticationType;
        }

        public struct ResponseInfo
        {
            public HttpStatusCode StatusCode;
            public string Content;

            public string GetFullHTTPCode()
            {
                return StatusCode.ToString() + " (" + (int)StatusCode + ")";
            }
        }

        private class WebClientHost : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);

                if (request is HttpWebRequest)
                {
                    var myWebRequest = request as HttpWebRequest;
                    myWebRequest.UnsafeAuthenticatedConnectionSharing = true;
                    myWebRequest.KeepAlive = true;
                }

                return request;
            }
        }
        #endregion // Constants and Types

        #region Members and properties
        public Cridential? Cridentials { get; set; } = null;
        public bool AcceptAllCertifications { get; set; } = false;
        public List<Header> Headers { get; set; } = new List<Header>();
        public string ContentType { get; set; } = REQUEST_TYPE;
        public string Accept { get; set; } = REQUEST_TYPE;
        public bool PreAuthenticate { get; set; } = true;
        public string UserAgent { get; set; } = USER_AGENT;
        public bool ThrowHTTPExeption { get; set; } = true;
        public ResponseInfo? responseInfo { get; set; } = null;
        
        public string TokenFileName { get; set; } = null;
        public string TokenEndpoint { get; set; }  = null;
        private int _TimeoutMilisec { get; set; } = 180000; // 3 minutes
        public int TimeoutMinutes
        {
            get
            {
                if (_TimeoutMilisec == System.Threading.Timeout.Infinite)
                    return System.Threading.Timeout.Infinite;
                else
                    return _TimeoutMilisec / 60000;
            }
            set
            {
                if (value != System.Threading.Timeout.Infinite && value < 0)
                    throw new Exception("HTTP.TimeoutMinutes.set - timeout is out of range");
                else
                {
                    if (value == System.Threading.Timeout.Infinite)
                        _TimeoutMilisec = System.Threading.Timeout.Infinite;
                    else
                        _TimeoutMilisec = value * 60000;
                }
            }
        }
        #endregion // Members and properties

        #region Public methods
        public string RequestPOST(string Url,
                             string Data)
        {
            return Request(enmMethods.POST, Url, Data);
        }

        public string RequestGET(string url)
        {
            return Request(enmMethods.GET, url, null);
        }

        public string Request(enmMethods method,
                                string url,
                                string data)
        {
            if (Cridentials.HasValue)
            {
                if (Cridentials.Value.AuthenticationType == enmAuthenticationType.OAuth2)
                {
                    return RequestOAuth2(method, url, data).Result;
                }
            }

            var request = GetRequestObject(url, UserAgent);
            request.ContentType = ContentType;
            request.Accept = Accept;
            request.Method = method.ToString();
            request.PreAuthenticate = PreAuthenticate;
            if (Cridentials.HasValue)
                AddAuthentication(request, url, Cridentials.Value);

            AddHeaders(request.Headers, Headers);

            if (AcceptAllCertifications)
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertificationsDo);

            request.Timeout = _TimeoutMilisec;
            request.ReadWriteTimeout = _TimeoutMilisec;
            request.ContinueTimeout = _TimeoutMilisec;

            if (data != null)
            {
                Stream stream = request.GetRequestStream();

                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            try
            {
                using (WebResponse Response = request.GetResponse())
                {
                    string result;
                    using (var streamReader = new StreamReader(Response.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }

                    responseInfo = new ResponseInfo
                    {
                        StatusCode = ((HttpWebResponse)Response).StatusCode,
                        Content = result
                    };
                    return result;
                }
            }
            catch (WebException webEx)
            {
                // Note: this function creates ResponseInfo object 
                Exception ex = GetRequestExpetion(webEx);

                if (ThrowHTTPExeption)
                    throw ex;
            }

            return null;
        }

        public async Task<string> RequestOAuth2(enmMethods method,
                                string url,
                                string data)
        {
            string accessToken = await GetAccessToken();

            using (HttpClient client = new HttpClient())
            {
                // Set the base address of the API.
                client.BaseAddress = new Uri(url);

                // Set the authorization header with the OAuth 2.0 Bearer token.
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                
                HttpResponseMessage response;
                if (method == enmMethods.GET)
                    response = await client.GetAsync(url);
                else
                {
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(url, content);
                }
                

                string result = await response.Content.ReadAsStringAsync();

                responseInfo = new ResponseInfo
                {
                    StatusCode = response.StatusCode,
                    Content = result
                };

                if (response.IsSuccessStatusCode)
                    return result;

                if (ThrowHTTPExeption)
                    throw new Exception($"HTTP Status code: ({(int)response.StatusCode}) {response.StatusCode}, HTTP error: '{result}'");
                
                return null;
            }
        }

        public DateTime? GetRefreshTokenExpirationDateTime()
        {
            Token token = Token.CreateInstance(TokenFileName);
            return token.RefreshTokenExpiresAt;
        }

        private async Task<string> GetAccessToken()
        {
            if (string.IsNullOrWhiteSpace(TokenFileName))
                throw new ArgumentNullException("TokenFileName is empty");

            Token token = Token.CreateInstance(TokenFileName);
            token.TokenEndpoint = TokenEndpoint;

            if (Cridentials.HasValue)
            {
                token.ClientId = Cridentials.Value.UserName;
                token.ClientSecret = Cridentials.Value.Password;
            }
            token.TimeoutMinutes = TimeoutMinutes;
           
            token = await token.GetAccessToken();
            return token.AccessToken;
        }

        private Exception GetRequestExpetion(WebException webEx)
        {
            string ErrorMessage;

            try
            {
                using (WebResponse Response = webEx.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)Response;

                    using (Stream ResponseData = Response.GetResponseStream())
                    using (var Reader = new StreamReader(ResponseData))
                    {
                        string Text = Reader.ReadToEnd();
                        ErrorMessage = string.Format("HTTP.HandleRequestExpetion - {0} ({1}) - Response content: '{2}' - Stack trace: {3}",
                                                            httpResponse.StatusDescription,
                                                            (int)httpResponse.StatusCode,
                                                            Text,
                                                            webEx.StackTrace);

                        responseInfo = new ResponseInfo
                        {
                            StatusCode = httpResponse.StatusCode,
                            Content = Text
                        };

                    }
                }
            }
            catch (Exception)
            {
                responseInfo = new ResponseInfo
                {
                    StatusCode = ((HttpWebResponse)webEx.Response).StatusCode,
                    Content = webEx.ToString()
                };
                return new Exception(string.Format("HTTP.HandleRequestExpetion - Request failed since: '{0}'", webEx.ToString()));
            }

            return new Exception(ErrorMessage);
        }

        public void DownloadFile(string Url, string fileName)
        {
            if (!Cridentials.HasValue)
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(Url, fileName);
                }
                return;
            }

            if (Cridentials.Value.AuthenticationType == enmAuthenticationType.Basic)
                throw new Exception("Basic authentication not supported");

            CredentialCache myCache = new CredentialCache();
            myCache.Add(new Uri(Url), "NTLM", GetNetworkCredential(Cridentials.Value));

            using (WebClientHost webClient = new WebClientHost())
            {
                webClient.Credentials = myCache;
                webClient.DownloadFile(Url, fileName);
            }
        }

        public void SetFullHTTPSSecurityProtocolType()
        {
            SetSecurityProtocolType((SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls);
        }
        #endregion // Public methods

        private static void SetSecurityProtocolType(SecurityProtocolType ProtocolType)
        {
            ServicePointManager.SecurityProtocol = ProtocolType;
        }

        private static HttpWebRequest GetRequestObject(string Url, string userAgent)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.UserAgent = userAgent;
            return request;
        }

        private static void AddHeaders(WebHeaderCollection WebHeaders, List<Header> Headers)
        {
            if (Headers == null)
                return;

            foreach (var Header in Headers)
            {
                WebHeaders.Add(Header.Name, Header.Value);
            }
        }

        private static void AddAuthentication(HttpWebRequest httpWebRequest, string Url, Cridential reqCridential)
        {
            if (reqCridential.UseDefaultNetworkCredentials)
            {
                if (reqCridential.AuthenticationType == enmAuthenticationType.Basic)
                    throw new Exception("UseDefaultNetworkCredentials can not be set when using basic authentication type");

                if (!string.IsNullOrEmpty(reqCridential.UserName) || !string.IsNullOrEmpty(reqCridential.Password))
                    throw new Exception("UserName or Password can not have value when UseDefaultNetworkCredentials is set");
            }

            switch (reqCridential.AuthenticationType)
            {
                case enmAuthenticationType.Basic:
                    string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(reqCridential.UserName + ":" + reqCridential.Password));
                    httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
                    break;
                case enmAuthenticationType.NTLM:
                    CredentialCache credentialCache = new CredentialCache();
                    credentialCache.Add(new Uri(Url), "NTLM", GetNetworkCredential(reqCridential));
                    httpWebRequest.Credentials = credentialCache;
                    break;
                default:
                    break;
            }
        }

        private static NetworkCredential GetNetworkCredential(Cridential reqCridential)
        {
            NetworkCredential credential;
            if (reqCridential.UseDefaultNetworkCredentials)
                credential = CredentialCache.DefaultNetworkCredentials;
            else
                credential = new NetworkCredential(reqCridential.UserName, reqCridential.Password);

            return credential;
        }

        private static bool AcceptAllCertificationsDo(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #region public static functionalities
        public struct Line
        {
            public string Name;
            public string Value;
        }

        public static string BuildJson(params Line[] lines)
        {
            JObject JS = new JObject();

            foreach (var line in lines)
            {
                JS.Add(line.Name, line.Value);
            }

            return JS.ToString();
        }

        public static string ToQueryString(NameValueCollection nvc, string url)
        {
            return url + ToQueryString(nvc);
        }

        public static string ToQueryString(NameValueCollection nvc)
        {
            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format(
            "{0}={1}",
            HttpUtility.UrlEncode(key),
            HttpUtility.UrlEncode(value))
                ).ToArray();
            return "?" + string.Join("&", array);
        }

        public static string ToFormString(NameValueCollection nvc)
        {
            NameValueCollection OutgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            OutgoingQueryString.Add(nvc);
            return OutgoingQueryString.ToString();
        }

        public static string GetQueryStringValue(string query, string key)
        {
            return ParseQueryString(query)[key];
        }

        public static NameValueCollection ParseQueryString(string query)
        {
            return HttpUtility.ParseQueryString(query);
        }

        public static BodyParams GetJSONBodyParams(string JSON)
        {
            return new BodyParams(JSON);
        }

        public static BodyParams GetJSONBodyParams(HttpRequest Request)
        {
            return new BodyParams(Request);
        }
        #endregion // public static functionalities
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Eldan.TypeExtensions
{
    public static class SystemWebExtensions
    {
        static public string LogIPAdress(string pageName, HttpRequest request, string RequestHeaderIPAddressKey)
        {
            string ipAddress = "[Empty]";

            if (RequestHeaderIPAddressKey != null)
            {
                ipAddress = request.Headers[RequestHeaderIPAddressKey] ?? request[RequestHeaderIPAddressKey];
            }

            return string.Format(pageName + ".Page_Load: Request came from: {0} ", string.IsNullOrWhiteSpace(ipAddress) ? "[Empty]" : ipAddress);
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Helper
{
    public static class HeaderRequestHelper
    {
        //private static string H_Sender = "X-Sender";
        //private static string H_Function_Code = "X-Function-Code";
        private static string H_Request_Id = "X-Request-Id";
        private static string H_ContentType = "Content-Type";


        public static HeaderRequestInfo GetHeaderInfo(HttpRequest request)
        {
            return new HeaderRequestInfo
            { 
                //Sender = request.Headers[H_Sender].ToString(),
                //FunctionCode = request.Headers[H_Function_Code].ToString(),
                RequestId = request.Headers[H_Request_Id].ToString(),
                ContentType = request.Headers[H_ContentType].ToString()
            };
        }

    }

    public class HeaderRequestInfo
    {
        //public string Sender { get; set; }
        public string RequestId { get; set; }
        public string ContentType { get; set; }

    }
}

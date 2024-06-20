using AXService.Dtos;
using AXService.Helper;
using AXService.Services.Interfaces;
using BGServiceAX.Attributes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;
namespace BGServiceAX
{
    public static class Util
    {
        //public static void AppendResponse(HeaderRequestInfo info)
        //{
        //    //Response.Headers.Add(H_Sender, info.Sender);
        //    //Response.Headers.Add(H_Function_Code, info.FunctionCode);
        //    Response.Headers.Add("X-Request-Id", info.RequestId);
        //}
        //public static ObjectResult ValidHeaderInfo(HeaderRequestInfo info, string contentType = "application/json")
        //{
        //    if (string.IsNullOrEmpty(info.RequestId))
        //    {
        //        return StatusCode(400, "Thiếu Header");
        //    }
        //    //if (info.FunctionCode != functionCode)
        //    //{
        //    //    return StatusCode(400, "Sai function_code");
        //    //}
        //    if (info.ContentType != contentType)
        //    {
        //        return StatusCode(400, $"Chỉ nhận Content-Type:{contentType}");
        //    }

        //    return null;
        //}
    }
}

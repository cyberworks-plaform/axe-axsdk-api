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
namespace BGServiceAX.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class RecController : Controller
    {
        //private readonly IAutoInsuranceSerivce _autoInsuranceSerivce;
        //private readonly IProcessRequestService _processRequestService;
        //private readonly string H_Request_Id = "X-Request-Id";
        //private readonly string H_ContentType = "Content-Type";

        //public RecController(IAutoInsuranceSerivce autoInsuranceSerivce, IProcessRequestService processRequestService)
        //{
        //    _autoInsuranceSerivce = autoInsuranceSerivce;
        //    _processRequestService = processRequestService;
        //}

        //[HttpPost]
        //[Route("img2text")]
        //[FilterSupportTagOnly("CyberWork")]
        //public async Task<IActionResult> TextRecognize([FromBody] BasicFileRequest request, string charType = "MIX")
        //{
        //    //Header process
        //    var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
        //    AppendResponse(headerInfo);
        //    var validInfo = ValidHeaderInfo(headerInfo);
        //    if (validInfo != null) return validInfo;
        //    try
        //    {
        //        var result = await _processRequestService.ProcessRequest(request, "img2text", headerInfo, charType);

        //        return new ContentResult
        //        {
        //            ContentType = "application/json",
        //            StatusCode = (int)HttpStatusCode.OK,
        //            Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.Message);
        //        Log.Error(ex.StackTrace);
        //        return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
        //    }
        //}

        //private void AppendResponse(HeaderRequestInfo info)
        //{
        //    //Response.Headers.Add(H_Sender, info.Sender);
        //    //Response.Headers.Add(H_Function_Code, info.FunctionCode);
        //    Response.Headers.Add(H_Request_Id, info.RequestId);
        //}
        //private ObjectResult ValidHeaderInfo(HeaderRequestInfo info, string contentType = "application/json")
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

using AXService.Dtos;
using AXService.Enums;
using AXService.Helper;
using AXService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AXAPIWrapper.Controllers
{
    [Route("[controller]")] // = axdes
    [ApiController]
    public class AxDESController : ControllerBase
    {
        private readonly IProcessRequestService _processRequestService;
        private readonly IAxdesService _axdesService;

        private readonly string H_Request_Id = "X-Request-Id";
        private readonly string H_ContentType = "Content-Type";

        public AxDESController(
            IProcessRequestService processRequestService,
            IAxdesService axdesService)
        {
            _processRequestService = processRequestService;
            _axdesService = axdesService;
        }

        [HttpPost]
        [Route("form/giay-chung-nhan-ho-kinh-doanh")]
        public async Task<IActionResult> Form_GiayChungNhanDangKyHoKinhDoanh([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCallAxDES.Form_GiayChungNhanDangKyHoKinhDoanh, headerInfo);
                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                string msg = "Error processing API: form/giay-chung-nhan-ho-kinh-doanh";
                Log.Error(ex,msg);
                return StatusCode(500,msg);
            }
        }

        #region private function

        private void AppendResponse(HeaderRequestInfo info)
        {
            Response.Headers.Add(H_Request_Id, info.RequestId);
        }
        #endregion
    }
}

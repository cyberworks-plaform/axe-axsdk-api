using AXService.Dtos;
using AXService.Enums;
using AXService.Helper;
using AXService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;
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

        /// <summary>
        /// Get root path that contain model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("RootPathOfModel")]
        public async Task<IActionResult> RootPathOfModel()
        {
            return Ok(await _axdesService.GetRooPathOfModel());
        }

        /// <summary>
        /// Get list model name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetModelName")]
        public async Task<IActionResult> GetListModelName()
        {
            var result = await _axdesService.GetListModelName();

            return new ContentResult
            {
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(result),

            };
        }

        /// <summary>
        /// List all api that can be used by client to extract form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAvailableAPI")]
        public async Task<IActionResult> GetAvailableAPIbyModelName()
        {
            var result = await _axdesService.GetListModelName();
            var controllerName = RouteData.Values["Controller"];
            var apis = result.Select(x => $"{controllerName}/form/{x.Replace(".zip", "")}").ToList();
            return new ContentResult
            {
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(apis),

            };
        }

        /// <summary>
        /// Bóc tách form biểu mẫu với modelName được truyền vào từ phía client
        /// Lấy bản ghi đầu tiên và chuyển đổi thành dạng Dictionay<string,InformationField>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("form/{modelName}")]
        public async Task<IActionResult> FormExtractByModelName([FromBody] BasicFileRequest request, string modelName)
        {
            var result = await ProcessAPIRequest(request, modelName, CommonEnum.FunctionToCallAxDES.FormExtractByModelName);
            return result;
        }
        /// <summary>
        /// Bóc tách form biểu mẫu với modelName được truyền vào từ phía client
        /// Giữ nguyên định dạng kết quả của Axdes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BocTachTheoMoHinhAsync/{modelName}")]
        public async Task<IActionResult> BocTachTheoMoHinhAsync([FromBody] BasicFileRequest request, string modelName)
        {
            var result = await ProcessAPIRequest(request, modelName, CommonEnum.FunctionToCallAxDES.FormExtractByModelNameWithRawResult);
            return result;
        }

        #region private function
        /// <summary>
        /// Thực hiện chuyển tiếp request tới AxDES
        /// </summary>
        /// <param name="request"></param>
        /// <param name="modelName"></param>
        /// <param name="functionToCall"></param>
        /// <returns></returns>
        private async Task<IActionResult> ProcessAPIRequest([FromBody] BasicFileRequest request, string modelName, string functionToCall)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            try
            {
                var result = await _processRequestService.ProcessRequest(request, functionToCall, headerInfo, modelName);
                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (AxDesNullValueException axdesNullException)
            {
                Log.Error(axdesNullException, axdesNullException.Message);
                throw ; // trả lại lỗi -> sẽ được xử lý chung ở middleware
            }
            catch (Exception ex)
            {
                string msg = $"Error calling extract form by modelName = {modelName} with message: {ex.Message}";
                Log.Error(ex, msg);
                return StatusCode(500, msg);
            }
        }
        private void AppendResponse(HeaderRequestInfo info)
        {
            Response.Headers.Add(H_Request_Id, info.RequestId);
        }
        #endregion
    }
}

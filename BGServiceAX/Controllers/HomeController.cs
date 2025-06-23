using AXService.Dtos;
using AXService.Enums;
using AXService.Helper;
using AXService.Services.Interfaces;
using BGServiceAX.Attributes;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BGServiceAX.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    [Produces("application/json")]
    public class HomeController : Controller
    {
        private readonly IAutoInsuranceSerivce _autoInsuranceSerivce;
        private readonly IProcessRequestService _processRequestService;
        private readonly IAxdesService _axdesService;
        //private readonly string H_Sender = "X-Sender";
        //private readonly string H_Function_Code = "X-Function-Code";
        private readonly string H_Request_Id = "X-Request-Id";
        private readonly string H_ContentType = "Content-Type";

        public HomeController(IAutoInsuranceSerivce autoInsuranceSerivce, IProcessRequestService processRequestService,IAxdesService axdesService)
        {
            _autoInsuranceSerivce = autoInsuranceSerivce;
            _processRequestService = processRequestService;
            _axdesService = axdesService;
        }

        [HttpPost]
        [Route("rec/img2text")]
        [FilterSupportTagOnly("CyberWork")]
        public async Task<IActionResult> TextRecognize([FromBody] BasicFileRequest request, string charType = "MIX")
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, "img2text", headerInfo, charType);
                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("rec/table2text")]
        [FilterSupportTagOnly("CyberWork")]
        public async Task<IActionResult> TextRecognizeForTable([FromBody] BasicFileRequest request, string ocr = "TRUE")
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, "table2text", headerInfo, ocr);
                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("misc/pdf-searchable")]
        [FilterSupportTagOnly("CyberWork")]
        public async Task<IActionResult> PDFSearchable([FromBody] BasicFileRequest request, string charType = "MIX")
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, "pdf-searchable", headerInfo, charType);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        //[HttpPost]
        //[Route("auto-insurance")]
        //[FilterSupportTagOnly("Insurance", "CyberWork")]
        //public async Task<IActionResult> ProcessAutoInsurance([FromBody] AutoInsuranceRequest request)
        //{
        //    var sender = Request.Headers[H_Sender].ToString();
        //    //var function_code = Request.Headers[H_Function_Code].ToString();
        //    var request_id = Request.Headers[H_Request_Id].ToString();
        //    var contentType = Request.Headers[H_ContentType].ToString();

        //    if (string.IsNullOrEmpty(sender)  || string.IsNullOrEmpty(request_id))
        //    {
        //        return StatusCode(400, "Thiếu Header");
        //    }
        //    //if (function_code != "process-auto-insurance")
        //    //{
        //    //    return StatusCode(400, "Sai function_code");
        //    //}
        //    if (contentType != "application/json")
        //    {
        //        return StatusCode(400, "Chỉ nhận Content-Type:application/json");
        //    }
        //    Response.Headers.Add(H_Sender, sender);
        //    //Response.Headers.Add(H_Function_Code, function_code);
        //    Response.Headers.Add(H_Request_Id, request_id);
        //    try
        //    {
        //        var result = await _autoInsuranceSerivce.ProcessRequest(request, "auto-insurance", request_id, sender);


        //        //Response.Headers.Add(H_ContentType, contentType);

        //        return new ContentResult
        //        {
        //            ContentType = "application/json",
        //            StatusCode = (int)HttpStatusCode.OK,
        //            Content = result
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.Message);
        //        Log.Error(ex.StackTrace);
        //        return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
        //    }
        //}

        [HttpPost]
        [Route("ocr/auto-insurance")]
        [FilterSupportTagOnly("CyberWork")]
        public async Task<IActionResult> ProcessAutoInsurance([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, "auto-insurance", headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("misc/text-compare")]
        [Produces("application/json")]
        //        [FilterSupportTagOnly("Insurance", "CyberWork")]
        public async Task<IActionResult> ProcessCompareText([FromBody] CompareTextRequest request, string output = "json")
        {
            //var sender = Request.Headers[H_Sender].ToString();
            //var function_code = Request.Headers[H_Function_Code].ToString();
            var request_id = Request.Headers[H_Request_Id].ToString();
            var contentType = Request.Headers[H_ContentType].ToString();
            if (output.ToLower() != "json" && output.ToLower() != "html")
            {
                return StatusCode(400, "param output không hợp lệ");
            }
            if (contentType != "application/json")
            {
                return StatusCode(400, "Chỉ nhận Content-Type:application/json ");
            }
            if (string.IsNullOrEmpty(request_id))
            {
                // return StatusCode(400, "Thiếu Header");
            }
            //if (function_code != "compare-texts")
            //{
            //    return StatusCode(400, "Sai function_code");
            //}
            try
            {
                //Response.Headers.Add(H_Sender, sender);
                //Response.Headers.Add(H_Function_Code, function_code);
                Response.Headers.Add(H_Request_Id, request_id);
                var result = await _autoInsuranceSerivce.ProcessCompareText(request, request_id, output);
                contentType = output.ToLower() == "json" ? "application/json" : "text/html";
                //Response.Headers.Add(H_ContentType, contentType);

                if (contentType == "application/json")
                {
                    return new ContentResult
                    {
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.OK,
                        Content = result
                    };
                }
                else
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.OK,
                        Content = result
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/vbhc")]
        [FilterSupportTagOnly("CyberWork")]
        public async Task<IActionResult> ProceesVbhc([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, "vbhc", headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        #region OCR bộ phiếu tư pháp
      
        [HttpPost]
        [Route("ocr/tuphap-khaisinh")]
        public async Task<IActionResult> ProcessTuPhapKhaiSinh([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapKhaiSinh, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/tuphap-khaitu")]
        public async Task<IActionResult> ProcessTuPhapKhaiTu([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapKhaiTu, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/tuphap-kethon")]
        public async Task<IActionResult> ProcessTuPhapKetHon([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapKetHon, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/tuphap-chamecon")]
        public async Task<IActionResult> ProcessTuPhapChaMeCon([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapChaMeCon, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/tuphap-caichinh")]
        public async Task<IActionResult> ProceesTuPhapCaiChinh([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapCaiChinh, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/tuphap-giamho")]
        public async Task<IActionResult> ProceesTuPhapGiamHo([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapGiamHo, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/tuphap-lyhon")]
        public async Task<IActionResult> ProceesTuPhapLyHo([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapLyHon, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        /// <summary>
        /// OCR mẫu phiếu Tư pháp - Nhận con nuôi
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Kết quả trả lại kiểu Dictionary<string, InformationField>
        /// </returns>
        [HttpPost]
        [Route("ocr/tuphap-nhanconnuoi")]
        public async Task<IActionResult> ProceesTuPhapNhanConNuoi([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapNhanConNuoi, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        /// <summary>
        /// OCR mẫu phiếu Tư pháp - Tình trạng hôn nhân
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Kết quả trả lại kiểu Dictionary<string, InformationField>
        /// </returns>
        [HttpPost]
        [Route("ocr/tuphap-honnhan")]
        public async Task<IActionResult> ProceesTuPhapHonNhan([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapHonNhan, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        /// <summary>
        /// OCR mẫu phiếu Tư pháp Khai sinh A3 có 3 dòng
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Kết quả trả lại kiểu List<Dictionary<string, InformationField>>
        /// </returns>
        [HttpPost]
        [Route("ocr/tuphap-a3-khaisinh-3")]
        public async Task<IActionResult> ProceesTuPhapA3KhaiSinh_3Regs([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh_Mau3Recogs, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }


        /// <summary>
        /// OCR mẫu phiếu Tư pháp Khai sinh A3 có 4 dòng
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Kết quả trả lại kiểu List<Dictionary<string, InformationField>>
        /// </returns>
        [HttpPost]
        [Route("ocr/tuphap-a3-khaisinh-4")]
        public async Task<IActionResult> ProceesTuPhapA3KhaiSinh_4Regs([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh_Mau4Recogs, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        /// <summary>
        /// OCR mẫu phiếu Tư pháp Khai tử A3
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Kết quả trả lại kiểu List<Dictionary<string, InformationField>>
        /// </returns>
        [HttpPost]
        [Route("ocr/tuphap-a3-khaitu")]
        public async Task<IActionResult> ProceesTuPhapA3KhaiTu([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                object result = "[]";
                var year = request.year;
                if (year == 1995)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiTu95, headerInfo);
                }
                if (year >= 1996 && year <=1997)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiTu96, headerInfo);
                }
                else if (year == 1998)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiTu98, headerInfo);
                }
                else if (year > 1999)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiTu, headerInfo);
                }
                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }


        /// <summary>
        /// OCR mẫu phiếu Tư pháp Kết hôn A3
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Kết quả trả lại kiểu List<Dictionary<string, InformationField>>
        /// </returns>
        [HttpPost]
        [Route("ocr/tuphap-a3-kethon")]
        public async Task<IActionResult> ProceesTuPhapA3KetHon([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                object result = "[]"; //empty list
                var year = request.year;
                if (year >= 1989 && year <= 1994)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KetHon89, headerInfo);
                }
                else if (year >= 1997 && year <= 1998)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KetHon98, headerInfo);
                }
                else if (year > 1999)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KetHon, headerInfo);
                }

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/tuphap-a3-nhanconnuoi")]
        public async Task<IActionResult> ProcessTuPhapA3NhanConNuoi([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3NhanConNuoi, headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/tuphap-a3-khaisinh")]
        public async Task<IActionResult> ProcessTuPhapA3KhaiSinh([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                object result = "[]";
                var year = request.year;
                if (year >= 1991 && year <= 1996)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh95, headerInfo);
                }
                else if (year == 1997)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh97, headerInfo);
                }
                else if (year == 1999)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh_Mau3Recogs, headerInfo);
                }
                else if (year > 1999)
                {
                    result = await _processRequestService.ProcessRequest(request, CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh_Mau4Recogs, headerInfo);
                }



                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        #endregion

        /// <summary>
        /// Hàm này không gọi AX-SDK nào, chỉ trả về rỗng []
        /// Mục đích: các mẫu phiếu chưa được xử lý bởi AX-SDK nhằm đảm bảo quy trình có thẻ hoạt động như bình thường
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Kết quả trả lại kiểu List<Dictionary<string, InformationField>>
        /// </returns>
        [HttpPost]
        [Route("ocr/no-ocr")]
        public async Task<IActionResult> NoOCR([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                object result = "[]";
                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result),
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }


        [HttpPost]
        [Route("ocr/tunguyen-insurance")]
        [FilterSupportTagOnly("Insurance", "CyberWork")]
        public async Task<IActionResult> ProceesTuNguyen([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, "tunguyen-insurance", headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }

        }

        [HttpPost]
        [Route("ocr/cmnd")]
        [FilterSupportTagOnly("Insurance", "CyberWork")]
        public async Task<IActionResult> ProceesCmnd([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, "cmnd", headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/passport")]
        [FilterSupportTagOnly("Insurance", "CyberWork")]
        public async Task<IActionResult> ProceesPassport([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, "passport", headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpPost]
        [Route("ocr/khaisinh")]
        [FilterSupportTagOnly("Insurance", "CyberWork")]
        public async Task<IActionResult> ProceesKhaiSinh([FromBody] BasicFileRequest request)
        {
            //Header process
            var headerInfo = HeaderRequestHelper.GetHeaderInfo(Request);
            AppendResponse(headerInfo);
            var validInfo = ValidHeaderInfo(headerInfo);
            if (validInfo != null) return validInfo;
            try
            {
                var result = await _processRequestService.ProcessRequest(request, "khaisinh", headerInfo);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result is string ? (string)result : JsonConvert.SerializeObject(result)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Đã có lỗi xảy ra, liên hệ quản trị viên");
            }
        }

        [HttpGet]
        [Route("_health_check")]
        [FilterSupportTagOnly("Insurance", "CyberWork")]
        public async Task<IActionResult> GetStatusSDK()
        {
            try
            {
                var result = await _processRequestService.GetSDKStatus();
                if (result)
                {
                    return new ContentResult
                    {
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.OK,
                        Content = "OK"
                    };
                }
                else
                {
                    return StatusCode(400, $"SDK không hoạt động");
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Có lỗi xảy ra");
            }

        }

        /// <summary>
        /// Lấy thông tin license của AX-SDK
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("_get_license")]

        public async Task<IActionResult> GetLicenseInfo()
        {
            try
            {
                var result = await _processRequestService.GetLicenseInfo();

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result
                };



            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return StatusCode(400, $"Có lỗi xảy ra");
            }

        }


        [HttpGet]
        [Route("_ping")]
        [FilterSupportTagOnly("CyberWork", "Insurance")]
        //[AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        /// <summary>
        /// Hàm này dùng để test tính năng Rate Limiting của API.
        /// Hàm này sẽ đợi ngâu nhiên từ 0 đến 5 giây trước khi trả về kết quả là: "Hello from TestRateLimit"
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> TestRateLimit()
        {
            await Task.Delay(new Random().Next(5000));
            return Ok($"Hello from TestRateLimit");
        }



        private void AppendResponse(HeaderRequestInfo info)
        {
            //Response.Headers.Add(H_Sender, info.Sender);
            //Response.Headers.Add(H_Function_Code, info.FunctionCode);
            Response.Headers.Add(H_Request_Id, info.RequestId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private ObjectResult ValidHeaderInfo(HeaderRequestInfo info, string contentType = "application/json")
        {
            //if (string.IsNullOrEmpty(info.RequestId))
            //{
            //    return StatusCode(400, "Thiếu Header");
            //}
            //if (info.FunctionCode != functionCode)
            //{
            //    return StatusCode(400, "Sai function_code");
            //}
            //if (info.ContentType != contentType)
            //{
            //    return StatusCode(400, $"Chỉ nhận Content-Type:{contentType}");
            //}

            return null;
        }

    }
}

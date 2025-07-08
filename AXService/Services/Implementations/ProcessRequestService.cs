using AX.AXSDK;
using AXService.Dtos;
using AXService.Enums;
using AXService.Helper;
using AXService.Services.Interfaces;
using Azure.Core;
using Microsoft.Extensions.Configuration;
using OneAPI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AXService.Services.Implementations
{
    public class ProcessRequestService : IProcessRequestService
    {
        #region Init
        private readonly IInternalOcrSerivce _ocrService;
        private readonly IAxdesService _axdesService;
        private readonly ITableSegmentationService _tableSegmentService;
        private readonly IAmzFileClientFactory _amzFileClientFactory;
        private readonly string TempSavePath;
        private readonly bool isSaveFile;


        public ProcessRequestService(IConfiguration configuration,
            IInternalOcrSerivce internalOcrSerivce,
            IAxdesService axdesService,
            ITableSegmentationService tableSegmentService,
            IAmzFileClientFactory amzFileClientFactory)
        {
            TempSavePath = configuration["StorageTempFile"];
            isSaveFile = configuration["IsSaveFile"] == "true";
            Directory.CreateDirectory(TempSavePath);
            _ocrService = internalOcrSerivce;
            _tableSegmentService = tableSegmentService;
            _amzFileClientFactory = amzFileClientFactory;
            _axdesService = axdesService;
        }
        #endregion
        public async Task<object> ProcessRequest(BasicFileRequest request, string endpoint, HeaderRequestInfo headerInfo, params string[] args)
        {
            string requestId = string.Empty;
            try
            {
                if (request.fileId == null)
                    request.fileId = Guid.NewGuid().ToString();

                requestId = $"{headerInfo.RequestId}";

                var sw = new Stopwatch();
                sw.Start();
                var logArgs = args != null ? string.Join(",", args) : "None";
                Log.Warning($"Start handle request : {requestId} - Request.FileId= {request.fileId} - Request EndPoint: {endpoint} - Request Args: {logArgs} ");
                var filePath = request.filePath;
                var isCreateStempFile = false;
                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = await SaveFileOfRequest(request, headerInfo);
                    isCreateStempFile = true;
                }
                var funcToCall = GetFuncToCall(endpoint, "", request.createNormalizedImage, args);

                if (request.field != null)
                {
                    string tableJson = request.field.ToString();
                    funcToCall = GetFuncToCall(endpoint, tableJson, request.createNormalizedImage, args);
                }

                var result = await GetOcrResult(filePath, headerInfo, funcToCall);

                //Hiện tại xóa file vì chạy thử
                if (isCreateStempFile && File.Exists(filePath))
                {
                    if (!isSaveFile)
                    {
                        File.Delete(filePath);
                    }
                }
                sw.Stop();
                Log.Warning($"End handle request : {requestId} - Duration: {sw.ElapsedMilliseconds} ms ");
                return result;


            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khi xử lý request: {requestId} => Message: {ex.Message} => Stack strace: {ex.StackTrace} ");
                throw ex;
            }
        }

        public async Task<bool> GetSDKStatus()
        {
            return await _ocrService.IsActive();
        }

        public async Task<string> GetLicenseInfo()
        {
            return await _ocrService.GetLicenseInfo();
        }


        #region Private
        private async Task<object> GetOcrResult(string filePath, HeaderRequestInfo headerInfo, Func<string, Task<object>> func)
        {
            var requestId = $"{headerInfo.RequestId}";
            var sw = new Stopwatch();
            Log.Warning($"Ocr Start - Request: {requestId} - File: {filePath}!!!");
            sw.Start();

            var resultOcr = await func.Invoke(filePath);
            Log.Warning($"Ocr End - Request: {requestId} - File {filePath} -  Duration: {sw.ElapsedMilliseconds} ms : Đã lấy xong kết quả Orc  : {resultOcr}");

            sw.Stop();
            return resultOcr;
        }

        private Func<string, Task<object>> GetFuncToCall(string endpoint, string tableJson , bool createNormalizedImage, params string[] args)
        {
            switch (endpoint)
            {
                case "table2text":
                    if (args == null || args[0].ToUpper() == "TRUE")
                    {
                        return async (string path) => await _ocrService.NhanDangTextChoBang(path, tableJson);
                    }
                    else
                    {
                        return async (string path) =>
                        {
                            return tableJson;
                        };
                    }

                case "cmnd":
                    return async (string path) => await _ocrService.BocTach_Cmnd(path);
                case "auto-insurance":
                    return async (string path) => await _ocrService.BocTachBaoHiemXeOTo(path);
                case "khaisinh":
                    return async (string path) => await _ocrService.BocTach_KhaiSinh(path);
                case "passport":
                    return async (string path) => await _ocrService.BocTach_Passport(path);
                case "vbhc":
                    return async (string path) => await _ocrService.BocTach_VBHC(path);
                case "tunguyen":
                    return async (string path) => await _ocrService.BocTach_TuNguyen(path);
                case CommonEnum.FunctionToCall.ExtractTuPhapKhaiSinh:
                    return async (string path) => await _ocrService.ExtractTuPhapKhaiSinh(path,createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapKhaiTu:
                    return async (string path) => await _ocrService.ExtractTuPhapKhaiTu(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapKetHon:
                    return async (string path) => await _ocrService.ExtractTuPhapKetHon(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapChaMeCon:
                    return async (string path) => await _ocrService.ExtractTuPhapChaMeCon(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapCaiChinh:
                    return async (string path) => await _ocrService.ExtractTuPhapCaiChinh(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapGiamHo:
                    return async (string path) => await _ocrService.ExtractTuPhapGiamHo(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapLyHon:
                    return async (string path) => await _ocrService.ExtractTuPhapLyHon(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapNhanConNuoi:
                    return async (string path) => await _ocrService.ExtractTuPhapNhanConNuoi(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapHonNhan:
                    return async (string path) => await _ocrService.ExtractTuPhapHonNhan(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh_Mau3Recogs:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiSinh_Mau3Recogs(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh_Mau4Recogs:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiSinh_Mau4Recogs(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiTu:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiTu(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KetHon:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KetHon(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3NhanConNuoi:
                    return async (string path) => await _ocrService.ExtractTuPhapA3NhanConNuoi(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh95:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiSinh95(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh97:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiSinh97(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KetHon89:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KetHon89(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KetHon98:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KetHon98(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiTu95:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiTu95(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiTu96:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiTu96(path, createNormalizedImage);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiTu98:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiTu98(path, createNormalizedImage);

                case CommonEnum.FunctionToCall.SegmentTuPhapA3KetHon:
                    return async (string path) => await _tableSegmentService.Segment_TuPhapA3KetHon(path, 0);
                case CommonEnum.FunctionToCall.SegmentTuPhapA3KhaiSinh:
                    return async (string path) => await _tableSegmentService.Segment_TuPhapA3KhaiSinh(path, 0);
                case CommonEnum.FunctionToCall.SegmentTuPhapA3KhaiTu:
                    return async (string path) => await _tableSegmentService.Segment_TuPhapA3KhaiTu(path, 0);

                #region axdes function mapping
                case CommonEnum.FunctionToCallAxDES.FormExtractByModelName:
                    if (args == null || args.Length < 1)
                    {
                        throw new ArgumentException("Missing Param: ModelName");
                    }
                    string modelName = args[0];
                    return async (string path) => await _axdesService.FormExtractByModelName(path, modelName,isUseAxdesRawResult:false);
                
                case CommonEnum.FunctionToCallAxDES.FormExtractByModelNameWithRawResult:
                    if (args == null || args.Length < 1)
                    {
                        throw new ArgumentException("Missing Param: ModelName");
                    }
                    modelName = args[0];
                    return async (string path) => await _axdesService.FormExtractByModelName(path, modelName, isUseAxdesRawResult: true);

                #endregion

                case "img2text":
                    if (args == null || args.Length < 1)
                    {
                        throw new ArgumentException("Missing Param");
                    }
                    if (!Enum.IsDefined(typeof(CommonEnum.CharType), args[0].ToUpper()))
                    {
                        throw new ArgumentException($"Parman charType must be: {string.Join(',', Enum.GetValues(typeof(CommonEnum.CharType)))}");
                    }
                    return async (string path) => await _ocrService.NhanDienVungOptional(path, (CommonEnum.CharType)Enum.Parse(typeof(CommonEnum.CharType), args[0].ToUpper()));
                case "pdf-searchable":
                    if (args == null || args.Length < 1)
                    {
                        throw new ArgumentException("Missing Param");
                    }
                    if (!Enum.IsDefined(typeof(CommonEnum.CharType), args[0].ToUpper()))
                    {
                        throw new ArgumentException($"Parman charType must be: {string.Join(',', Enum.GetValues(typeof(CommonEnum.CharType)))}");
                    }
                    return async (string path) => await _ocrService.TaoFilePDFSearch(path, (CommonEnum.CharType)Enum.Parse(typeof(CommonEnum.CharType), args[0].ToUpper()));
                default:
                    return async (string path) => await _ocrService.NhanDienVung(path);
            }
        }

        private async Task<string> SaveFileOfRequest(BasicFileRequest request, HeaderRequestInfo headerInfo)
        {
            var fileNameWithOutExt = $"{request.fileId}_{headerInfo.RequestId}";
            if (!string.IsNullOrEmpty(request.fileBase64)) //=> đọc file Base64
            {
                var fileExtension = GetFileExtensionBase64(request.fileBase64);
                var fileName = string.IsNullOrEmpty(fileExtension) ? fileNameWithOutExt : $"{fileNameWithOutExt}.{fileExtension}";
                var filepath = Path.Combine(TempSavePath, fileName);
                if (File.Exists(filepath))
                {
                    throw new Exception("Trùng request");
                }
                File.WriteAllBytes(filepath, Convert.FromBase64String(request.fileBase64));
                return filepath;
            }
            else //DownloadFile
            {
                var amzS3client = _amzFileClientFactory.Create();
                var filePath = await amzS3client.DownloadFileFromS3Url(fileNameWithOutExt, request.fileUrl);
                return filePath;
            }

        }

        private string GetFileExtensionBase64(string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}

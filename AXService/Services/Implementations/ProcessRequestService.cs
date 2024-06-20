using AXService.Dtos;
using AXService.Enums;
using AXService.Helper;
using AXService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AXService.Services.Implementations
{
    public class ProcessRequestService : IProcessRequestService
    {
        #region Init
        private readonly IInternalOcrSerivce _ocrService;
        private readonly IAmzFileClientFactory _amzFileClientFactory;
        private readonly string TempSavePath;
        private readonly bool isSaveFile;


        public ProcessRequestService(IConfiguration configuration, IInternalOcrSerivce internalOcrSerivce, IAmzFileClientFactory amzFileClientFactory)
        {
            TempSavePath = configuration["StorageTempFile"];
            isSaveFile = configuration["IsSaveFile"] == "true";
            Directory.CreateDirectory(TempSavePath);
            _ocrService = internalOcrSerivce;
            _amzFileClientFactory = amzFileClientFactory;
        }
        #endregion
        public async Task<object> ProcessRequest(BasicFileRequest request, string endpoint, HeaderRequestInfo headerInfo, params string[] args)
        {
            try
            {
                if (request.fileId == null)
                    request.fileId = Guid.NewGuid().ToString();
                Log.Warning($"Xử lý request :  {headerInfo.RequestId}_{request.fileId}");
                var filePath = await SaveFileOfRequest(request, headerInfo);
                var funcToCall = GetFuncToCall(endpoint, "", args);

                if (request.field != null)
                {
                    string tableJson = request.field.ToString();
                    funcToCall = GetFuncToCall(endpoint, tableJson, args);
                }

                var result = await GetOcrResult(filePath, headerInfo, funcToCall);

                //Hiện tại xóa file vì chạy thử
                if (File.Exists(filePath))
                {
                    if (!isSaveFile)
                    {
                        File.Delete(filePath);
                    }
                }
                return result;


            }
            catch (Exception ex)
            {

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
            var sw = new Stopwatch();
            Log.Warning($"Ocr Start !!!");
            sw.Start();

            var resultOcr = await func.Invoke(filePath);
            Log.Warning($"Duration: {sw.ElapsedMilliseconds} ms : Đã lấy xong kết quả Orc  : {resultOcr}");

            sw.Stop();
            return resultOcr;
        }

        private Func<string, Task<object>> GetFuncToCall(string endpoint, string tableJson = "", params string[] args)
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
                case "hotich":
                    if (args == null || args.Length < 3)
                    {
                        throw new ArgumentException("Missing Param");
                    }
                    if (!Enum.IsDefined(typeof(CommonEnum.DocType), args[0].ToUpper()))
                    {
                        throw new ArgumentException($"Parman docType must be: {string.Join(',', Enum.GetValues(typeof(CommonEnum.DocType)))}");
                    }
                    if (!Enum.IsDefined(typeof(CommonEnum.FormType_HT), args[1].ToUpper()))
                    {
                        throw new ArgumentException($"Parman formType must be: {string.Join(',', Enum.GetValues(typeof(CommonEnum.FormType_HT)))}");
                    }
                    if (!Enum.IsDefined(typeof(CommonEnum.CharType), args[2].ToUpper()))
                    {
                        throw new ArgumentException($"Parman charType must be: {string.Join(',', Enum.GetValues(typeof(CommonEnum.CharType)))}");
                    }
                    return async (string path) => await _ocrService.BocTach_HoTich(path, (CommonEnum.DocType)Enum.Parse(typeof(CommonEnum.DocType), args[0].ToUpper()), (CommonEnum.FormType_HT)Enum.Parse(typeof(CommonEnum.FormType_HT), args[1].ToUpper()), (CommonEnum.CharType)Enum.Parse(typeof(CommonEnum.CharType), args[2].ToUpper()));
                case CommonEnum.FunctionToCall.ExtractTuPhapCaiChinh:
                    return async (string path) => await _ocrService.ExtractTuPhapCaiChinh(path);
                case CommonEnum.FunctionToCall.ExtractTuPhapGiamHo:
                    return async (string path) => await _ocrService.ExtractTuPhapGiamHo(path);
                case CommonEnum.FunctionToCall.ExtractTuPhapLyHon:
                    return async (string path) => await _ocrService.ExtractTuPhapLyHon(path);
                case CommonEnum.FunctionToCall.ExtractTuPhapNhanConNuoi:
                    return async (string path) => await _ocrService.ExtractTuPhapNhanConNuoi(path);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh_Mau3Recogs:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiSinh_Mau3Recogs(path);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiSinh_Mau4Recogs:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiSinh_Mau4Recogs(path);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KhaiTu:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KhaiTu(path);
                case CommonEnum.FunctionToCall.ExtractTuPhapA3KetHon:
                    return async (string path) => await _ocrService.ExtractTuPhapA3KetHon(path);
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

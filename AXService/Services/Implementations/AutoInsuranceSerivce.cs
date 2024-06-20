using AXService.Dtos;
using AXService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AXService.Services.Implementations
{
    public class AutoInsuranceSerivce : IAutoInsuranceSerivce
    {
        private readonly IBlobService _blobService;
        private readonly IInternalOcrSerivce _ocrService;
        private readonly string TempSavePath;

        public AutoInsuranceSerivce(IBlobService blobService, IConfiguration configuration, IInternalOcrSerivce internalOcrSerivce)
        {
            _blobService = blobService;
            TempSavePath = configuration["StorageTempFile"];
            _ocrService = internalOcrSerivce;
        }
        public async Task<string> ProcessRequest(AutoInsuranceRequest request, string endpoint, string requestId)
        {
            try
            {
                Log.Warning($"Xử lý request :  {requestId}");
                var filePath = SaveRequest(request, requestId);
                Log.Warning($"Upload File");
                await _blobService.UploadFileBlobAsync(filePath, Path.GetFileName(filePath));
                var result = await GetOcrResult(filePath);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi xử lý request: {requestId} ; ex: {ex.Message} ; trace :{ex.StackTrace}");
                return $"Lỗi xử lý request: {requestId} ; ex: {ex.Message}";
            }
        }

        public async Task<string> ProcessCompareText(CompareTextRequest request, string requestId, string output)
        {
            try
            {
                Log.Warning($"Xử lý request :  {requestId}");
                var resultOcr = await _ocrService.CompareText(request.textOne, request.textTwo, output);
                return resultOcr;
            }
            catch (Exception ex)
            {

                Log.Error($"Lỗi xử lý request: {requestId} ; ex: {ex.Message} ; trace :{ex.StackTrace}");
                return $"Lỗi xử lý request: {requestId} ; ex: {ex.Message}";
            }
        }

        private string SaveFileFromBase64(string fileName, string base64)
        {
            if (!Directory.Exists(TempSavePath))
            {
                Directory.CreateDirectory(TempSavePath);
            }
            var filepath = Path.Combine(TempSavePath, fileName);
            File.WriteAllBytes(filepath, Convert.FromBase64String(base64));
            return filepath;
        }

        private string SaveRequestV0(AutoInsuranceRequest request, string requestId)
        {
            var fileName = $"{request.fileId}_{requestId}";//.{request.fileExtension}";
            fileName = string.IsNullOrEmpty(request.fileExtension) ? fileName : $"{fileName}.{request.fileExtension}";
            return SaveFileFromBase64(fileName, request.fileBase64);
        }
        private string SaveRequest(AutoInsuranceRequest request, string requestId)
        {
            var fileNameWithOutExt = $"{request.fileId}_{requestId}";
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

        private async Task<string> GetOcrResult(string filePath)
        {
            var sw = new Stopwatch();
            Log.Warning($"Ocr !!!");
            sw.Start();
            var resultOcr = await _ocrService.BocTachBaoHiemXeOTo(filePath);
            Log.Warning($"Duration: {sw.ElapsedMilliseconds} ms : Đã lấy xong kết quả Orc  : {resultOcr}");

            sw.Stop();
            //if (File.Exists(filePath))
            //{
            //    File.Delete(filePath);
            //}
            return resultOcr;
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
    }
}

using AXService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Implementations
{
    public class CyberWorkService : ICyberWorkService
    {
        private readonly string TempSavePath;
        private readonly IInternalOcrSerivce _ocrService;

        public CyberWorkService(IConfiguration configuration, IInternalOcrSerivce internalOcrSerivce)
        {
            TempSavePath = configuration["StorageTempFile"];
            _ocrService = internalOcrSerivce;
        }


        public async Task<string> GetOcrRegion(string base64String)
        {
            var fileExtension = GetFileExtension(base64String);
            var fileName = Guid.NewGuid().ToString();
            fileName = string.IsNullOrEmpty(fileExtension) ? fileName : $"{fileName}.{fileExtension}";
            var filepath = Path.Combine(TempSavePath, fileName);
            File.WriteAllBytes(filepath, Convert.FromBase64String(base64String));

            var result =  await _ocrService.NhanDienVung(filepath);

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            return result;
        }

        private string GetFileExtension(string base64String)
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

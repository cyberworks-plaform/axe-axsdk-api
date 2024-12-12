using AXService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using AxDesApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Newtonsoft;
using Newtonsoft.Json;
using AX.AXSDK;
using System.Linq;
namespace AXService.Services.Implementations
{
    /// <summary>
    /// Lớp này cung cấp dịch vụ gọi các hàm xử lý bóc tách của AxDSES và trả lại kết quả cho lớp trên API-Controller
    /// Đi kèm với mỗi hàm bóc tách của AxDES sẽ là 1 model => Model này được lưu trữ ở share storage 
    /// Các kết quả trả lại từ AxDES sẽ được chuyển đổi sang dạng Dictionary<string,InformationField> 
    ///     => mục đích đồng dạng với các result trả về từ AXSDK 
    /// </summary>
    public class AxdesService : IAxdesService
    {
        private readonly IConfiguration _configuration;
        private readonly string _axSvAddress;
        private readonly string _axdesModelPath; //root path contain model for runing ax-des form extraction

        public AxdesService(IConfiguration configuration)
        {
            _configuration = configuration;
            _axSvAddress = _configuration["AxConfigs:Address"] ?? "localhost";
            _axdesModelPath = _configuration["AxConfigs:AxdesModelPath"] ?? $"{Path.Combine(Environment.CurrentDirectory, "\\lib\\ax-des\\model")}";

            AxDesApiManager.SetHost(_axSvAddress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<object> Form_GiayChungNhanDangKyHoKinhDoanh(string filePath)
        {
            var modelName = "cw.giaychungnhanhokinhdoanh.zip";
            try
            {
                var modelPath = Path.Combine(_axdesModelPath, modelName);
                if (false == File.Exists(modelPath))
                {
                    throw new FileNotFoundException(modelPath);
                }
                var axdesResponse = await AxDesApi.Extraction.BocTachTheoMoHinhAsync(modelPath, filePath, string.Empty, isKeyByNameField: true);

                var res = ConvertResultToAxSDKFormat(axdesResponse.FirstOrDefault());
                
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error calling {nameof(AxDesApi.Extraction.BocTachTheoMoHinhAsync)}  with model: {modelName} ");
                throw;
            }
        }

        #region private function
        /// <summary>
        /// Convert result 
        ///     from format AXDES->ExtractModels.ExtractResult 
        ///     to format AXSDK->Dictionary<string,InformationField>
        /// </summary>
        /// <param name="axdesResult"></param>
        /// <returns></returns>
        private Dictionary<string,InformationField> ConvertResultToAxSDKFormat(ExtractModels.ExtractResult axdesResult)
        {
            var desResult = new Dictionary<string,InformationField>();
            foreach (var item in axdesResult.Result)
            {
                desResult.Add(item.Key, new InformationField
                {
                     Area = new System.Drawing.Rectangle(item.Value.Area.x, item.Value.Area.y, item.Value.Area.width, item.Value.Area.height),
                     PageSize = new System.Drawing.Size(item.Value.PageSize.width,item.Value.PageSize.height),
                     Confidence = item.Value.Confidence,
                     Page = item.Value.Page,
                     Value = item.Value.Value,
                     Name = item.Key, // trong AXDES chưa có name => Tạm lấy Key làm Name
                });
            }
            return desResult;
        }
        #endregion


    }
}

using AX.AXSDK;
using AXService.Enums;
using AXService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OneAPI;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static AXService.Enums.CommonEnum;

namespace AXService.Services.Implementations
{
    public class InternalOcrSerivce : IInternalOcrSerivce
    {
        private readonly IConfiguration _configuration;
        private readonly string _axSvAddress;

        public InternalOcrSerivce(IConfiguration configuration)
        {
            _configuration = configuration;
            _axSvAddress = _configuration["AxConfigs:Address"] ?? "localhost";
            APIs.SetServerAddress(_axSvAddress);
        }

        public async Task<bool> IsActive()
        {
            var timeout = TimeSpan.FromSeconds(1); //5s
            try
            {
                return await CancelAfterAsync(ct => GetStatusSDK(ct), timeout, new CancellationToken());
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Log.Error("TimeOut when get Sdk Alive");
                return false;

            }
        }
        public async Task<string> GetLicenseInfo()
        {
            var timeout = TimeSpan.FromSeconds(1); //5s
            try
            {
                var licenseinfo = APIs.GetLicenseInfo();
                return await Task.FromResult(licenseinfo);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Log.Error("TimeOut when get Sdk Alive");
                return ex.Message;

            }
        }

        #region Helper
        private async Task<bool> GetStatusSDK(CancellationToken cancellationToken)
        {
            try
            {
                var result = APIs.IsSDKAlive();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }

        }
        async Task<TResult> CancelAfterAsync<TResult>(Func<CancellationToken, Task<TResult>> startTask, TimeSpan timeout, CancellationToken cancellationToken)
        {
            using (var timeoutCancellation = new CancellationTokenSource())
            using (var combinedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellation.Token))
            {
                var originalTask = startTask(combinedCancellation.Token);
                var delayTask = Task.Delay(timeout, timeoutCancellation.Token);
                var completeTask = await Task.WhenAny(originalTask, delayTask);

                timeoutCancellation.Cancel();
                if (completeTask == originalTask)
                {
                    return await originalTask;
                }
                else
                {
                    throw new TimeoutException();
                }
            }

        }
        #endregion


        public async Task<string> DemoBocTachVBHC(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ExtractInfoAPI.SmartExtractVBHCInfo(filePath, out string vbhcjson));
                //return JsonConvert.SerializeObject(rs);
                return vbhcjson;
            }
            catch (Exception ex)
            {
                throw ex;
                //return ex.Message;
            }
        }

        public async Task<string> BocTachChuIn(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ExtractInfoAPI.ExtractHoaDonChuIn(filePath));
                //return JsonConvert.SerializeObject(rs);
                return rs;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public async Task<string> BocTachChuVietTay(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ExtractInfoAPI.ExtractHoaDonVietTay(filePath));
                //return JsonConvert.SerializeObject(rs);
                return rs;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }




        public async Task<string> BocTachHoaDon(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ExtractInfoAPI.ExtractHoaDon(filePath));
                //return JsonConvert.SerializeObject(rs);
                return rs;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //public async Task<string> BocTachBangLaiXe(string filePath)
        //{
        //    try
        //    {
        //        var rs = await Task.FromResult(APIs.ExtractInfoAPI.ExtractBangLaiXe(filePath, out string vbhcjson));
        //        //return JsonConvert.SerializeObject(rs);
        //        return vbhcjson;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //        //return ex.Message;
        //    }
        //}


        public async Task<string> BocTachBaoHiemXeOTo(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ExtractInfoAPI.ExtractBaoHiemOto(filePath));
                //return JsonConvert.SerializeObject(rs);
                return rs;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> CompareText(string textOne, string textTwo, string output)
        {
            try
            {
                if (output.ToLower() == "json")
                {
                    var rs = await Task.FromResult(APIs.SpellAPI.CompareText2Json(textOne, textTwo));
                    return rs;
                }
                else if (output.ToLower() == "html")
                {
                    var rs = await Task.FromResult(APIs.SpellAPI.CompareText2HTML(textOne, textTwo));
                    return rs;
                }
                else
                {
                    return string.Empty;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<object> TaoFilePDFSearch(string filePath, CommonEnum.CharType charType = CharType.MIX)
        {
            try
            {
                if (charType == CharType.MIX)
                {
                    var rs = await Task.FromResult(APIs.PDFAPI.CreatePDFA4VBHC_CVT(filePath, filePath + "_out.pdf"));
                    Byte[] pdfSearch = File.ReadAllBytes(filePath + "_out.pdf");
                    return Convert.ToBase64String(pdfSearch);
                }
                else if (charType == CharType.TEXT)
                {
                    var rs = await Task.FromResult(APIs.PDFAPI.CreatePDFA4VBHC(filePath, filePath + "_out.pdf"));
                    Byte[] pdfSearch = File.ReadAllBytes(filePath + "_out.pdf");
                    return Convert.ToBase64String(pdfSearch);
                }
                else
                {
                    var rs = await Task.FromResult(APIs.PDFAPI.CreatePDFA4CVT(filePath, filePath + "_out.pdf"));
                    Byte[] pdfSearch = File.ReadAllBytes(filePath + "_out.pdf");
                    return Convert.ToBase64String(pdfSearch);
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return ex.Message;
            }
        }
        public async Task<string> NhanDangTextChoBang(string filePath, string tableJson)
        {
            try
            {
                string outTableJson = APIs.API.RecognizeTableJSON(filePath, tableJson);
                return outTableJson;
            }
            catch (Exception ex)
            {
                throw ex;
                //return ex.Message;
            }
        }


        public async Task<string> NhanDienVung(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ICRecognizer.RecognizeVBHC_CVT(filePath, out string outputJson));
                return outputJson;
            }
            catch (Exception ex)
            {
                throw ex;
                //return ex.Message;
            }
        }
        /// <summary>
        /// Hàm này sẽ OCR 1 ảnh đầu vào và trả lại kết quả là text tương ứng
        /// Hàm này thường dùng để OCR một mẩu ảnh (vùng ảnh) được cắt ra từ ảnh lớn
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="charType"></param>
        /// <returns></returns>
        public async Task<object> NhanDienVungOptional(string filePath, CharType charType = CharType.MIX)
        {
            try
            {
                var rs = new List<OCRResult>();
                if (charType == CharType.MIX)
                {
                  rs  = await Task.FromResult(APIs.ICRecognizer.RecognizeVBHC_CVT(filePath, out string outputJson));
                    
                }
                else if (charType == CharType.TEXT)
                {
                    rs= await Task.FromResult(APIs.API.RecognizeVBHC(filePath));
                }
                else
                {
                    rs = await Task.FromResult(APIs.ICRecognizer.RecognizeCVT(filePath));
                    //Dictionary<string, string> output = new Dictionary<string, string>();
                    //output.Add("text", rs.ToString());
                    
                }

                return rs;
            }
            catch (Exception ex)
            {
                throw ex;
                //return ex.Message;
            }
        }


        //AxCloud

        public async Task<object> BocTach_VBHC(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ExtractInfoAPI.SmartExtractVBHCInfo(filePath, out string vbhcjson));
                return vbhcjson;
                //return rs;
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        public async Task<object> BocTach_Cmnd(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ExtractInfoAPI.ExtractCMNDByDL(filePath));
                return JsonConvert.SerializeObject(rs);
                //return rs;
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        public async Task<object> BocTach_KhaiSinh(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractGiayKhaiSinh(filePath, out string jsoninfo));
                //return JsonConvert.SerializeObject(rs);
                return jsoninfo;
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }
        public async Task<object> BocTach_Passport(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ExtractInfoAPI.ExtractPassport(filePath, out string jsonInfo));
                return jsonInfo;
                //return rs;
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        public async Task<object> BocTach_TuNguyen(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.ExtractInfoAPI.ExtractBaoHiemTuNguyen(filePath, out string jsonInfo));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        public async Task<object> BocTach_HoTich(string filePath, DocType docType, FormType_HT formType, CharType charType)
        {
            try
            {
                if (docType == DocType.A3)
                {
                    return await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3_KS_DEV(filePath, out List<List<Dictionary<string, string>>> sub));
                }
                else //A4
                {
                    switch ((formType, charType))
                    {
                        case (FormType_HT.KS, CharType.HANDWRI): //Khai sinh, viết tay
                            APIs.FormAPI.ExtractTuPhapKS(filePath, out string jsonInfoTuPhapKS);
                            return await Task.FromResult(jsonInfoTuPhapKS);
                        case (FormType_HT.KS, CharType.TEXT): //Khai sinh chữ in
                            APIs.FormAPI.ExtractTuPhapKSChuIn(filePath, out string jsonInfoTuPhapKSChuIn);
                            return await Task.FromResult(jsonInfoTuPhapKSChuIn);
                        case (FormType_HT.KH, CharType.HANDWRI): // kết hôn viết tay
                            APIs.FormAPI.ExtractTuPhapKH(filePath, out string jsonInfoTuPhapKH);
                            return await Task.FromResult(jsonInfoTuPhapKH);
                        case (FormType_HT.CMC, CharType.HANDWRI): // Nhận cha mẹ con viết tay
                            APIs.FormAPI.ExtractTuPhapCMC(filePath, out string tuPhapCMCJson);
                            return await Task.FromResult(tuPhapCMCJson);
                        case (FormType_HT.KT, CharType.HANDWRI): //Khai tử viết tay
                            APIs.FormAPI.ExtractTuPhapKT(filePath, out string jsonInfoTuPhapKT);
                            return await Task.FromResult(jsonInfoTuPhapKT);
                        case (FormType_HT.KT, CharType.TEXT): //Khai tử chữ in
                            APIs.FormAPI.ExtractTuPhapKTChuIn(filePath, out string jsonInfoTuPhapKTChuIn);
                            return await Task.FromResult(jsonInfoTuPhapKTChuIn);
                        case (FormType_HT.HN, CharType.HANDWRI): //Hôn nhân viết tay
                            APIs.FormAPI.ExtractTuPhapHN(filePath, out string jsonInfoTuPhapHN);
                            return await Task.FromResult(jsonInfoTuPhapHN);
                        default:
                            throw new Exception("Does not support");
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Boc tách phiếu khai sinh A4
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<object> ExtractTuPhapKhaiSinh(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapKhaiSinh(filePath));
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Bóc tách phiếu tư pháp khai tử A4
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<object> ExtractTuPhapKhaiTu(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapKhaiTu(filePath));
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Bóc tách phiếu kết hôn A4
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<object> ExtractTuPhapKetHon(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapKetHon(filePath));
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Bóc tách phiếu tư pháp Cha Mẹ Con A4
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<object> ExtractTuPhapChaMeCon(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapChaMeCon(filePath));
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }
        /// <summary>
        /// Bóc tách phiếu Tư pháp - Cải chính
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Dictionary<string, InformationField></returns>
        public async Task<object> ExtractTuPhapCaiChinh(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapCaiChinh(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Bóc tách phiếu Tư pháp - Giám hộ
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// Dictionary<string, InformationField>
        /// </returns>
        public async Task<object> ExtractTuPhapGiamHo(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapGiamHo(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Bóc tách phiếu Tư pháp - Ly Hôn
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// Dictionary<string, InformationField>
        /// </returns>
        public async Task<object> ExtractTuPhapLyHon(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapLyHon(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Bóc tách phiếu Tư pháp - Nhận con nuôi
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// Dictionary<string, InformationField>
        /// </returns>
        public async Task<object> ExtractTuPhapNhanConNuoi(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapNhanConNuoi(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }


        /// <summary>
        /// Bóc tách phiếu  A3 Khai sinh 3 recogs
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// List<Dictionary<string, InformationField>>
        /// </returns>
        public async Task<object> ExtractTuPhapA3KhaiSinh_Mau3Recogs(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KhaiSinh_Mau3Recogs(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Bóc tách phiếu  A3 Khai sinh 4 recogs
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// List<Dictionary<string, InformationField>>
        /// </returns>
        public async Task<object> ExtractTuPhapA3KhaiSinh_Mau4Recogs(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KhaiSinh_Mau4Recogs(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Bóc tách phiếu  A3 Khai tử
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// List<Dictionary<string, InformationField>>
        /// </returns>
        public async Task<object> ExtractTuPhapA3KhaiTu(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KhaiTu(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        /// <summary>
        /// Bóc tách phiếu  A3 Kết hôn
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// List<Dictionary<string, InformationField>>
        /// </returns>
        public async Task<object> ExtractTuPhapA3KetHon(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KetHon(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        public async Task<object> ExtractTuPhapA3NhanConNuoi(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3NhanConNuoi(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        public async Task<object> ExtractTuPhapA3KhaiSinh95(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KhaiSinh95(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        public async Task<object> ExtractTuPhapA3KhaiSinh97(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KhaiSinh97(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        public async Task<object> ExtractTuPhapA3KetHon89(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KetHon89(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }
        public async Task<object> ExtractTuPhapA3KetHon98(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KetHon98(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

        public async Task<object> ExtractTuPhapA3KhaiTu95(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KhaiTu95(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }
        public async Task<object> ExtractTuPhapA3KhaiTu96(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KhaiTu96(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }
        public async Task<object> ExtractTuPhapA3KhaiTu98(string filePath)
        {
            try
            {
                var rs = await Task.FromResult(APIs.FormAPI.ExtractTuPhapA3KhaiTu98(filePath));
                //return jsonInfo;
                return JsonConvert.SerializeObject(rs);
            }
            catch (Exception ex)
            {

                throw ex;
                //return ex.Message;
            }
        }

    
    }
}

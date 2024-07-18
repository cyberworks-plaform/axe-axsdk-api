using AXService.Enums;
using System.Threading.Tasks;
using static AXService.Enums.CommonEnum;

namespace AXService.Services.Interfaces
{
    public interface IInternalOcrSerivce
    {
        Task<string> DemoBocTachVBHC(string filePath);
        Task<string> BocTachChuIn(string filePath);
        Task<string> BocTachChuVietTay(string filePath);
        Task<string> BocTachHoaDon(string filePath);
        Task<string> BocTachBaoHiemXeOTo(string filePath);
        Task<string> CompareText(string textOne, string textTwo, string output);
        Task<object> NhanDienVungOptional(string filePath, CharType charType = CharType.MIX);
        Task<string> NhanDienVung(string filePath);
        Task<string> NhanDangTextChoBang(string filePath, string tableJson);
        Task<object> BocTach_VBHC(string filePath);
        Task<object> BocTach_HoTich(string filePath, DocType docType, FormType_HT formType, CharType charType);
        Task<object> BocTach_TuNguyen(string filePath);
        Task<object> BocTach_Cmnd(string filePath);
        Task<object> BocTach_Passport(string filePath);
        Task<object> BocTach_KhaiSinh(string filePath);
        //Task<string> BocTachBangLaiXe(string filePath);
        Task<object> TaoFilePDFSearch(string filePath, CommonEnum.CharType charType = CharType.MIX);
        Task<bool> IsActive();
        Task<string> GetLicenseInfo();
        Task<object> ExtractTuPhapCaiChinh(string filePath);
        Task<object> ExtractTuPhapGiamHo(string filePath);
        Task<object> ExtractTuPhapLyHon(string filePath);
        Task<object> ExtractTuPhapNhanConNuoi(string filePath);
        Task<object> ExtractTuPhapA3KhaiSinh_Mau3Recogs(string filePath);
        Task<object> ExtractTuPhapA3KhaiSinh_Mau4Recogs(string filePath);
        Task<object> ExtractTuPhapA3KhaiTu(string filePath);
        Task<object> ExtractTuPhapA3KetHon(string filePath);
        Task<object> ExtractTuPhapA3NhanConNuoi(string filePath);
        Task<object> ExtractTuPhapA3KhaiSinh95(string filePath);
        Task<object> ExtractTuPhapA3KetHon89(string filePath);
        Task<object> ExtractTuPhapA3KhaiTu95(string filePath);
        Task<object> ExtractTuPhapA3KhaiTu98(string filePath);
        Task<object> ExtractTuPhapKhaiSinh(string filePath);
        Task<object> ExtractTuPhapKhaiTu(string filePath);
        Task<object> ExtractTuPhapKetHon(string filePath);
        Task<object> ExtractTuPhapChaMeCon(string filePath);
        Task<object> ExtractTuPhapA3KhaiSinh97(string filePath);
        Task<object> ExtractTuPhapA3KetHon98(string filePath);
        Task<object> ExtractTuPhapA3KhaiTu96(string filePath);
    }
}

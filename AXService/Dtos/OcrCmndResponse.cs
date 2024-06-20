using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class OcrCmndResponse : OcrResponse
    {
        public OcrCmndData Data { get; set; }
    }

    public class OcrCmndData
    {
        public string AnhChanDung { get; set; }

        public string CmndImage { get; set; }

        public string DanToc { get; set; }

        public double DanTocConfidence { get; set; }

        public string DauVetRiengVaDiHinh { get; set; }

        public double DauVetRiengVaDiHinhConfidence { get; set; }

        public string DoBackFace { get; set; }

        public string DoFrontFace { get; set; }

        public string GioiTinh { get; set; }

        public double GioiTinhConfidence { get; set; }

        public string HoKhauThuongTru { get; set; }

        public double HoKhauThuongTruConfidence { get; set; }

        public string HoTen { get; set; }

        public double HoTenConfidence { get; set; }

        public string HoTenKhac { get; set; }

        public double HoTenKhacConfidence { get; set; }

        public string NgayCap { get; set; }

        public double NgayCapConfidence { get; set; }

        public string NgayHetHan { get; set; }

        public double NgayHetHanConfidence { get; set; }

        public string NgaySinh { get; set; }

        public double NgaySinhConfidence { get; set; }

        public string NguyenQuan { get; set; }

        public double NguyenQuanConfidence { get; set; }

        public string NoiCap { get; set; }

        public string QuocTich { get; set; }

        public double QuocTichConfidence { get; set; }

        public string SoCmnd { get; set; }

        public double SoCmndConfidence { get; set; }

        public string TonGiao { get; set; }

        public double TonGiaoConfidence { get; set; }

        public string Type { get; set; }

        public string VanTayPhai { get; set; }

        public string VanTayTrai { get; set; }
    }
}

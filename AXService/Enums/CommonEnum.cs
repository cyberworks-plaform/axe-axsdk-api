using AX.AXSDK;
using OneAPI;
using System;
using System.Collections.Generic;

namespace AXService.Enums
{
    public static class CommonEnum
    {
        public enum FormType_HT
        {
            KS, //Khai sinh
            KT, //Khai tử
            KH, //Kết hôn
            HN, //Hôn nhân
            CMC, //Nhận cha mẹ con
        }
        public enum DocType
        {
            A3,
            A4,
        }

        public enum CharType
        {
            TEXT,
            HANDWRI,
            MIX
        }

        /// <summary>
        /// Danh sách các hàm để mapping từ ProcessRequest -> InternalOcr
        /// </summary>
        public struct FunctionToCall
        {
            //Tư pháp A4
            public const string ExtractTuPhapKhaiSinh = "ExtractTuPhapKhaiSinh";
            public const string ExtractTuPhapKhaiTu = "ExtractTuPhapKhaiTu";
            public const string ExtractTuPhapKetHon = "ExtractTuPhapKetHon";
            public const string ExtractTuPhapChaMeCon = "ExtractTuPhapChaMeCon";
            public const string ExtractTuPhapCaiChinh = "ExtractTuPhapCaiChinh";
            public const string ExtractTuPhapGiamHo = "ExtractTuPhapGiamHo";
            public const string ExtractTuPhapLyHon = "ExtractTuPhapLyHon";
            public const string ExtractTuPhapNhanConNuoi = "ExtractTuPhapNhanConNuoi";
            public const string ExtractTuPhapHonNhan = "ExtractTuPhapHonNhan";

            //Tư pháp A3
            public const string ExtractTuPhapA3KhaiSinh_Mau3Recogs = "ExtractTuPhapA3KhaiSinh_Mau3Recogs";
            public const string ExtractTuPhapA3KhaiSinh_Mau4Recogs = "ExtractTuPhapA3KhaiSinh_Mau4Recogs";
            public const string ExtractTuPhapA3KhaiTu = "ExtractTuPhapA3KhaiTu";
            public const string ExtractTuPhapA3KetHon = "ExtractTuPhapA3KetHon";

            public const string ExtractTuPhapA3NhanConNuoi = "ExtractTuPhapA3NhanConNuoi";
            public const string ExtractTuPhapA3KhaiSinh95 = "ExtractTuPhapA3KhaiSinh95";
            public const string ExtractTuPhapA3KhaiSinh97 = "ExtractTuPhapA3KhaiSinh97";
            public const string ExtractTuPhapA3KetHon89 = "ExtractTuPhapA3KetHon89";
            public const string ExtractTuPhapA3KetHon98 = "ExtractTuPhapA3KetHon98";
            public const string ExtractTuPhapA3KhaiTu95 = "ExtractTuPhapA3KhaiTu95";
            public const string ExtractTuPhapA3KhaiTu96 = "ExtractTuPhapA3KhaiTu96";
            public const string ExtractTuPhapA3KhaiTu98 = "ExtractTuPhapA3KhaiTu98";

            public const string SegmentTuPhapA3KetHon = "SegmentTuPhapA3KetHon";
            public const string SegmentTuPhapA3KhaiSinh = "SegmentTuPhapA3KhaiSinh";
            public const string SegmentTuPhapA3KhaiTu = "SegmentTuPhapA3KhaiTu";



        }

        public struct FunctionToCallAxDES
        {
            public const string FormExtractByModelName = "FormExtractByModelName";
        }
    }
}

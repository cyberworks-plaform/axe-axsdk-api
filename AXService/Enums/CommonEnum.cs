using AX.AXSDK;
using OneAPI;
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
            public const string ExtractTuPhapCaiChinh = "ExtractTuPhapCaiChinh";
            public const string ExtractTuPhapGiamHo = "ExtractTuPhapGiamHo";
            public const string ExtractTuPhapLyHon = "ExtractTuPhapLyHon";
            public const string ExtractTuPhapNhanConNuoi = "ExtractTuPhapNhanConNuoi";
            public const string ExtractTuPhapA3KhaiSinh_Mau3Recogs = "ExtractTuPhapA3KhaiSinh_Mau3Recogs";
            public const string ExtractTuPhapA3KhaiSinh_Mau4Recogs = "ExtractTuPhapA3KhaiSinh_Mau4Recogs";
            public const string ExtractTuPhapA3KhaiTu = "ExtractTuPhapA3KhaiTu";
            public const string ExtractTuPhapA3KetHon = "ExtractTuPhapA3KetHon";

        }
    }
}

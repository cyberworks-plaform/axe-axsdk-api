using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class OcrVbhcResponse : OcrResponse
    {
        public OcrVbhcData Data { get; set; }
    }

    public class OcrVbhcData
    {
        public string DieuKhoan { get; set; }

        public string KinhGui { get; set; }

        public string KyHieu { get; set; }

        public string LoaiVB { get; set; }

        public string NgayKy { get; set; }

        public string NgayNhanMoc { get; set; }

        public string NguoiKy { get; set; }

        public string NoiBanHanh { get; set; }

        public string NoiDung { get; set; }

        public string NoiNhan { get; set; }

        public string NoiNhanMoc { get; set; }

        public string SoKyHieu { get; set; }

        public string SoQuyetDinh { get; set; }

        public string ThoiHanMoc { get; set; }

        public string VeViec { get; set; }
    }
}

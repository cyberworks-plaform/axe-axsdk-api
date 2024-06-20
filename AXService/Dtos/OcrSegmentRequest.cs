using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class OcrSegmentRequest : OcrRequest
    {
        public string CoordinateArea { get; set; }

        public string FilePath { get; set; }    // Đường dẫn tương đối file gốc
    }
}

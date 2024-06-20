using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class OcrSegmentRequestV2 : OcrRequest
    {
        public string CoordinateArea { get; set; }

        public Guid FileInstanceId { get; set; }    // InstanceId của file gốc
    }
}

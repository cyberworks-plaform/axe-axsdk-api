using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class OcrResponse
    {
        public bool Success { get; set; }
        public int? Status { get; set; }
        public int Time { get; set; }
        public string Message { get; set; }
    }
}

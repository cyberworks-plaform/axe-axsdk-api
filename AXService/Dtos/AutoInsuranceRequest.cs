using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class AutoInsuranceRequest
    {
        public string fileId { get; set; }
        public string fileExtension { get; set; } = "pdf";
        public string fileBase64 { get; set; }
    }
}

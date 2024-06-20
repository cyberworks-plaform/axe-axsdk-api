using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class OcrRequest
    {
        public string ApiKey { get; set; }

        public IFormFile FileDocument { get; set; }

        public string Type { get; set; }

        public int Page { get; set; }

        public string Field { get; set; }

        public int FieldType { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int PageWidth { get; set; }

        public int PageHeight { get; set; }
    }

    public class OcrRequestType
    {
        public const string VBHC = "VBHC";
        public const string vbhc_tm = "vbhc_tm";
        public const string CMND = "CMND";
        public const string form = "form";
        public const string FULLTEXT = "FULLTEXT";

    }
}

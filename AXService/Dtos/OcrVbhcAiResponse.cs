using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class OcrVbhcAiResponse : OcrResponse
    {
        public List<OcrVbhcAiData> Data { get; set; }
    }

    public class OcrVbhcAiData
    {
        public string Type { get; set; }

        public string Value { get; set; }

        public int Page { get; set; }

        public string Field { get; set; }

        public int FieldType { get; set; } = 1;

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int PageWidth { get; set; }

        public int PageHeight { get; set; }

        public bool IsVBHCField { get; set; }

        public List<LineData> Lines { get; set; }
    }

    public class LineData
    {
        public string Text { get; set; }

        public int Line { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int PageWidth { get; set; }

        public int PageHeight { get; set; }

        public List<double> CharConfidences { get; set; }

        public List<double> WordConfidences { get; set; }

        //public List<double> WordCharConfidences { get; set; }

        public double LineConfidence { get; set; }
    }
}

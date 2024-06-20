using AX.AXSDK;
using AXService.Dtos;
using Ce.Common.Lib.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AXService.Helper
{
    public static class SegmentHelper
    {
        public static OcrRequest ParseCoordinateArea(string coordinateArea)
        {
            var coord = JsonConvert.DeserializeObject<Hashtable>(coordinateArea);
            var page = coord["Page"].ToString();
            var w = coord["Width"].ToString();
            var h = coord["Height"].ToString();
            var x = coord["X"].ToString();
            var y = coord["Y"].ToString();
            var pw = coord["PageWidth"].ToString();
            var ph = coord["PageHeight"].ToString();
            var wid = 0;
            var hei = 0;
            int pwid = 0;
            int phei = 0;
            var pointX = 0;
            var pointY = 0;
            var pageInt = 0;
            if (StringHelper.IsNumeric(w))
                wid = int.Parse(w);
            if (StringHelper.IsNumeric(h))
                hei = int.Parse(h);
            if (StringHelper.IsNumeric(pw))
                pwid = (int)(float.Parse(pw));
            if (StringHelper.IsNumeric(ph))
                phei = (int)(float.Parse(ph));
            if (StringHelper.IsNumeric(x))
                pointX = int.Parse(x);
            if (StringHelper.IsNumeric(y))
                pointY = int.Parse(y);
            if (StringHelper.IsNumeric(page))
                pageInt = int.Parse(page);
            if (wid <= 0 || hei <= 0)
            {
                return null;
            }

            return new OcrRequest
            {
                Page = pageInt,
                PageWidth = pwid,
                PageHeight = phei,
                X = pointX,
                Y = pointY,
                Width = wid,
                Height = hei,
                FieldType = (int)FieldType.TEXT
            };
        }
    }
}

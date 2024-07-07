using AX.AXSDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface ITableSegmentationService
    {
        Task<List<Dictionary<string, InformationField>>> Segment_TuPhapA3KetHon(string filePath, int year);
        Task<List<Dictionary<string, InformationField>>> Segment_TuPhapA3KhaiSinh(string filePath, int year);
        Task<List<Dictionary<string, InformationField>>> Segment_TuPhapA3KhaiTu(string filePath, int year);
    }
}

using AXService.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface IExternalOcrService
    {
        Task<OcrVbhcResponse> BmBocTachVbhc(OcrRequest model);

        Task<OcrVbhcAiResponse> BmBocTachVbhcAi(OcrRequest model);

        Task<OcrCmndResponse> BmBocTachCmnd(OcrRequest model);

        Task<OcrSegmentResponse> OcrWithSegment(OcrSegmentRequest model);

        Task<OcrSegmentResponse> OcrSegmentFull(OcrRequest model);
    }
}

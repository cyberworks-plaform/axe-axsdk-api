using AXService.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface IAutoInsuranceSerivce
    {
        Task<string> ProcessRequest(AutoInsuranceRequest request,string endpoint, string requestId);
        Task<string> ProcessCompareText(CompareTextRequest request, string requestId, string output);
    }
}

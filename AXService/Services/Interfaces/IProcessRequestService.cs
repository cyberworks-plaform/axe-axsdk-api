using AXService.Dtos;
using AXService.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface IProcessRequestService
    {
        Task<object> ProcessRequest(BasicFileRequest request, string endpoint,HeaderRequestInfo headerInfo, params string[] args);
        Task<bool> GetSDKStatus();
        Task<string> GetLicenseInfo();
    }
}

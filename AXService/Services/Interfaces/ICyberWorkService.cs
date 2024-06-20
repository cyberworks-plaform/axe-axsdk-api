using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface ICyberWorkService
    {
        Task<string> GetOcrRegion(string base64String);
    }
}

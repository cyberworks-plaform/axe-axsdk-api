using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface IAxdesService
    {
        Task<object> Form_GiayChungNhanDangKyHoKinhDoanh(string filePath);
    }
}

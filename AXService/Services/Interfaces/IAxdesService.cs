using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface IAxdesService
    {
        Task<object> FormExtractByModelName(string filePath, string modelName,bool isUseAxdesRawResult);
        Task<List<string>> GetListModelName();
        Task<string> GetRooPathOfModel();
    }
}

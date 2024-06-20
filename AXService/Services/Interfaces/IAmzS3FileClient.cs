using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface IAmzS3FileClient
    {
        Task<string> DownloadFileFromS3Url(string prefix, string url);
        Task<string> DownloadFileFromUrl(string fileNamePrefix, string url);
    }
}

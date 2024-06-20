using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface IBlobService
    {
        Task<BlobDownloadResult> GetBlobAsync(string name);
        Task<IEnumerable<string>> GetListBlobAsync();
        Task UploadFileBlobAsync(string filePath, string fileName);
        Task UploadContentBlobAsync(string content, string fileName);
        Task UploadContentBlobAsync(byte[] bytes, string fileName);
        Task DeleteBlobAsync(string blobName);
    }
}

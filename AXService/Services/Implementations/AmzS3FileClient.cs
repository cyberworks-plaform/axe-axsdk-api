using AXService.Helper;
using AXService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Implementations
{
    public class AmzS3FileClient : IAmzS3FileClient
    {
        private readonly HttpClient _client;
        private readonly string _savePath;
        public AmzS3FileClient(HttpClient httpClient,IConfiguration configuration)
        {
            _client = httpClient;
            _savePath = configuration["StorageTempFile"];
        }


        public async Task<string> DownloadFileFromS3Url(string prefix,string url)
        {
            try
            {
                Uri uri = new Uri(url);
                var fileNameUri = Path.GetFileName(uri.LocalPath);
                var fileName = $"{prefix}_{fileNameUri}";
                var filePath = Path.Combine(_savePath, fileName);
                if (File.Exists(filePath))
                {
                    throw new Exception("Request trùng");
                }
                var response = await _client.GetAsync(url);
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var fileInfo = new FileInfo(filePath);
                    using (var fileStream = fileInfo.OpenWrite())
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }

                return filePath;

            }
            catch (Exception ex)
            {

                throw new Exception($"Lỗi download file với url: {url} : {ex.Message}");

            }

        }

        public async Task<string> DownloadFileFromUrl(string fileNamePrefix, string url)
        {
            try
            {
                Uri uri = new Uri(url);
                var fileNameUri = Path.GetFileName(uri.LocalPath);
                var extension = Path.GetExtension(fileNameUri);
                var fileName = $"{fileNamePrefix}{extension}";
                var filePath = Path.Combine(_savePath, fileName);

                var response = await _client.GetAsync(url);
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var fileInfo = new FileInfo(filePath);
                    using (var fileStream = fileInfo.OpenWrite())
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }

                return filePath;

            }
            catch (Exception ex)
            {

                throw new Exception($"Lỗi download file với url: {url}");

            }
        }
    }
}

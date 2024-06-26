using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class BasicFileRequest
    {
        /// <summary>
        /// fileId: được sử dụng để làm định danh cho mỗi request
        /// default value: Guid.NewGuid()
        /// </summary>
        public string fileId { get; set; } //

        /// <summary>
        /// Đường dẫn file dạng smb hoặc local storage
        /// </summary>
        public string filePath { get; set; }
        /// <summary>
        /// Nếu fileBase64=empty => tự dộng tử file từ amzone: var filePath = await amzS3client.DownloadFileFromS3Url(fileNameWithOutExt, request.fileUrl);
        /// </summary>
        public string fileUrl { get; set; }

        /// <summary>
        /// file extention; default = pdf
        /// </summary>
        public string fileExtension { get; set; } = "pdf";

        /// <summary>
        /// Dữ liệu file dưới dạng Base64
        /// </summary>
        public string fileBase64 { get; set; }
        public object field { get; set; } //=> ocr for table
    }
}

using AXService.Services.Interfaces;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AXService.Dtos;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using AXService.Enums;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace AXService.Services.Implementations
{
    public class Worker : IWorker
    {
        private bool IsEnqueue = true;
        private readonly int TimeDelayWhenFree = 5;
        private readonly string QueueTaskConnectionString;
        private readonly string QueueTaskName;
        private readonly string QueueResultConnectionString;
        private readonly string QueueResultName;
        private readonly string TempSavePath;
        private readonly bool UseCloudBlob = true;
        private readonly bool IsMessEncode = false;

        private readonly QueueClient _taskClient;
        private readonly QueueClient _resultClient;


        private readonly IBlobService _blobService;
        private readonly IInternalOcrSerivce _ocrService;
        private readonly IExternalOcrService _ocrExternalService;
        private readonly IAmzFileClientFactory _amzFileClientFactory;


        public Worker(IConfiguration configuration, IBlobService blobService, IInternalOcrSerivce internalOcrSerivce, IExternalOcrService externalOcrService, IAmzFileClientFactory amzFileClientFactory)
        {
            _blobService = blobService;
            _ocrService = internalOcrSerivce;
            _ocrExternalService = externalOcrService;
            _amzFileClientFactory = amzFileClientFactory;

            UseCloudBlob = (configuration["UseBlob"] ?? "true") == "true";
            IsMessEncode = (configuration["IsEncode"] ?? "true") == "true";

            TempSavePath = configuration["StorageTempFile"];
            var timeDelayFromConfig = configuration["RunConfig:DelayWhenFree"] ?? "";
            if (Int32.TryParse(timeDelayFromConfig, out int configdelay))
            {
                TimeDelayWhenFree = configdelay;
            }
            QueueTaskConnectionString = configuration["TaskQueue:ConnectionString"] ?? "";
            QueueTaskName = configuration["TaskQueue:Name"] ?? "";
            QueueResultConnectionString = configuration["ResultQueue:ConnectionString"] ?? "";
            QueueResultName = configuration["ResultQueue:Name"] ?? "";
            if (string.IsNullOrEmpty(QueueTaskConnectionString) || string.IsNullOrEmpty(QueueResultConnectionString) || string.IsNullOrEmpty(QueueTaskName) || string.IsNullOrEmpty(QueueResultName))
            {
                throw new Exception("Can't not read connection string");
            }

            //Init connect
            _taskClient = new QueueClient(QueueTaskConnectionString, QueueTaskName); //todo: chang type
            _resultClient = new QueueClient(QueueResultConnectionString, QueueResultName); //todo: chang type
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (IsEnqueue)
                {
                    await GetMessageOnTaskQueue();
                    //await Task.Delay(1000 * 60);
                }
                else
                {
                    await RelaxGetACoffee();
                }

            }
        }
        #region Read Message
        private async Task GetMessageOnTaskQueue()
        {
            try
            {
                if (await _taskClient.ExistsAsync())
                {
                    QueueProperties properties = await _taskClient.GetPropertiesAsync();
                    if (properties.ApproximateMessagesCount > 0)
                    {
                        Log.Warning($"Nhận message mới");
                        QueueMessage[] retrievedMessage = await _taskClient.ReceiveMessagesAsync(1);
                        string theMessage = retrievedMessage[0].Body.ToString();
                        string theMessageId = retrievedMessage[0].MessageId.ToString();
                        Log.Warning($"Message Receive: {theMessageId}");
                        await _taskClient.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);


                        var rs = await ReadMessage(theMessage, theMessageId);

                        //Gửi kết quả
                        await SendResultMessage(rs);
                    }
                    else
                    {
                        IsEnqueue = false;
                    }
                }
                else
                {
                    Log.Error("Task Queue not Exist");
                }
            }
            catch (Exception ex)
            {

                Log.Error($"Lỗi khi nhận message: {ex.Message}");
            }

        }
        private async Task<string> ReadMessage(string messageRaw, string messageId)
        {
            try
            {
                
                var messageText = string.Empty;
                if (IsMessEncode)
                {
                    messageText = Base64Decode(messageRaw);
                }
                else
                {
                    messageText = messageRaw;
                }
                Log.Warning($"Bắt đầu lấy kết quả Ocr");
                var messageRequest = JsonConvert.DeserializeObject<TaskQueueRequest>(messageText);
                if (messageRequest == null)
                {
                    throw new Exception($"Không thể parse tin nhắn {messageId} với nội dung: {messageRaw}");
                }

                Log.Warning($"Tải file");
                var amzS3client = _amzFileClientFactory.Create();
                var filePath = await amzS3client.DownloadFileFromS3Url(messageRequest.requestId,messageRequest.fileUrl);

                if (!File.Exists(filePath))
                {
                    throw new Exception($"Không thể download file với tin nhắn {messageId} có nội dung: {messageRaw}");
                }


                Log.Warning($"Upload File");
                await _blobService.UploadFileBlobAsync(filePath, Path.GetFileName(filePath));

                //var resultOcr = string.Empty;
                //var sw = new Stopwatch();
                //Log.Warning($"Ocr !!!");
                //sw.Start();
                //switch (messageRequest.function)
                //{
                //    case nameof(EnumTaskQueue.Function.processTaxInvoice):
                //        resultOcr = await _ocrService.BocTachHoaDon(filePath);
                //        break;
                //    default:
                //        Log.Error($"Không xác định được function với message: {messageId}");
                //        throw new Exception($"Không xác định được function với message: {messageId}");
                //}
                ////Log.Warning($"Lấy được kết quả {resultOcr} ");
                //var result = JsonConvert.DeserializeObject(resultOcr);
                
                //JObject jO = JObject.FromObject(result);
                //if (jO.ContainsKey("fileId"))
                //{
                //    jO["fileId"] = messageRequest.fileId;
                //}
                //else
                //{
                //    jO.Add("fileId", messageRequest.fileId);
                //}
                //if (jO.ContainsKey("requestId"))
                //{
                //    jO["requestId"] = messageRequest.requestId;
                //}
                //else
                //{
                //    jO.Add("requestId", messageRequest.requestId);
                //}
                ////jO.Add("requestId", messageRequest.requestId);

                //var resultMessage = jO.ToString();
                //Log.Warning($"Duration: {sw.ElapsedMilliseconds} ms : Đã lấy xong kết quả Orc với message {messageId} : {resultMessage}");
                
                //sw.Stop();

                //if (File.Exists(filePath))
                //{
                //    File.Delete(filePath);
                //}

                return await GetOcrResult(messageRequest, messageId, filePath);// resultMessage;
            }
            catch (Exception ex)
            {
                Log.Error($"Lôi khi lấy kết quả Ocr của gói tin {messageId} nội dung {messageRaw} : {ex.Message}");
                return $"Lỗi với tin nhắn {messageId} :{ex.Message}";
            }            
        }


        private async Task<string> GetOcrResult(TaskQueueRequest messageRequest, string messId, string path)
        {
            if (!File.Exists(path))
            {
                var objectResult = new {
                    fileId = messageRequest.fileId,
                    requestId = messageRequest.requestId,
                    message = "Không thể lưu được file vào hệ thống"
                };

                return JsonConvert.SerializeObject(objectResult);
            }
            try
            {
                var resultOcr = string.Empty;
                var sw = new Stopwatch();
                Log.Warning($"Ocr !!!");
                sw.Start();
                switch (messageRequest.function)
                {
                    case nameof(EnumTaskQueue.Function.processTaxInvoice):
                        resultOcr = await _ocrService.BocTachHoaDon(path);
                        break;
                    case nameof(EnumTaskQueue.Function.processAutoInsurance):
                        resultOcr = await _ocrService.BocTachBaoHiemXeOTo(path);
                        break;
                    default:
                        Log.Error($"Không xác định được function với message: {messId}");
                        throw new Exception($"Không xác định được function với request: {messageRequest.requestId}");
                }
                //Log.Warning($"Lấy được kết quả {resultOcr} ");
                var result = JsonConvert.DeserializeObject(resultOcr);

                JObject jO = JObject.FromObject(result);
                //fileId
                if (jO.ContainsKey(nameof(TaskQueueRequest.fileId)))
                {
                    jO[nameof(TaskQueueRequest.fileId)] = messageRequest.fileId;
                }
                else
                {
                    jO.Add(nameof(TaskQueueRequest.fileId), messageRequest.fileId);
                }

                //requestId
                if (jO.ContainsKey(nameof(TaskQueueRequest.requestId)))
                {
                    jO[nameof(TaskQueueRequest.requestId)] = messageRequest.requestId;
                }
                else
                {
                    jO.Add(nameof(TaskQueueRequest.requestId), messageRequest.requestId);
                }

                //function
                //if (jO.ContainsKey(nameof(TaskQueueRequest.function)))
                //{
                //    jO[nameof(TaskQueueRequest.function)] = messageRequest.function;
                //}
                //else
                //{
                //    jO.Add(nameof(TaskQueueRequest.function), messageRequest.function);
                //}

                ////sender
                //if (jO.ContainsKey(nameof(TaskQueueRequest.sender)))
                //{
                //    jO[nameof(TaskQueueRequest.sender)] = messageRequest.sender;
                //}
                //else
                //{
                //    jO.Add(nameof(TaskQueueRequest.sender), messageRequest.sender);
                //}


                var resultMessage = jO.ToString();
                Log.Warning($"Duration: {sw.ElapsedMilliseconds} ms : Đã lấy xong kết quả Orc với message {messId} : {resultMessage}");

                sw.Stop();

                File.Delete(path);
                return resultMessage;
            }
            catch (Exception ex)
            {
                var objectResult = new
                {
                    fileId = messageRequest.fileId,
                    requestId = messageRequest.requestId,
                    message = ex.Message
                };
                Log.Error($"Xảy ra lỗi khi OCR :  message {messId} : {JsonConvert.SerializeObject(objectResult)}");
                return JsonConvert.SerializeObject(objectResult);
            }
        }    

        #endregion

        #region Send Message
        private async Task SendResultMessage(string mess)
        {
            try
            {
                //mess = Base64Encode(mess);
                await _resultClient.SendMessageAsync(mess);
                Log.Warning($"Đã gửi kết quả lên queue");
            }
            catch (Exception ex)
            {

                Log.Error($"Lỗi khi gửi message: {ex.Message}");
            }
        }
        #endregion

        #region Private 
        private async Task RelaxGetACoffee()
        {
            Log.Information($"Relax time");
            await Task.Delay(1000 * TimeDelayWhenFree);
            IsEnqueue = true;
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private string GetTempPath(string fileName)
        {
            if (Directory.Exists(TempSavePath))
            {
                var temp = Guid.NewGuid();
                var newPath = Path.Combine(TempSavePath, $"{temp}_{fileName}");
                return newPath;
            }
            else
            {
                throw new Exception("Path save temp file not exist");
            }

        }
        #endregion
    }
}

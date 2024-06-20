using AXService.Dtos;
using AXService.Helper;
using AXService.Services.Interfaces;
using Ce.Interaction.Lib.HttpClientAccessors.Interfaces;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Implementations
{
    public class ExternalOcrService : IExternalOcrService
    {
        private readonly IBaseHttpClientFactory _clientFatory;
        private readonly string _apiExternalOcrEndpoint;
        private readonly string _apiKey;

        public ExternalOcrService(IConfiguration configuration, IBaseHttpClientFactory clientFatory)
        {
            _clientFatory = clientFatory;
            _apiExternalOcrEndpoint = configuration["AxConfigs:ApiExternalOcrEndpoint"] ?? "localhost";
            _apiKey = configuration["AxConfigs:ApiKey"] ?? "";
        }

        public async Task<OcrVbhcResponse> BmBocTachVbhc(OcrRequest model)
        {
            model.Type = OcrRequestType.VBHC;
            model.ApiKey = _apiKey;

            OcrVbhcResponse response;
            try
            {
                var client = _clientFatory.Create();
                var rs = await client.PostWithFilePublicAsync<Hashtable>(_apiExternalOcrEndpoint, null, nameof(model.FileDocument), model.FileDocument, model);
                response = new OcrVbhcResponse
                {
                    Data = rs["data"] != null ? JsonConvert.DeserializeObject<OcrVbhcData>(rs["data"].ToString()) : null,
                    Message = rs["messages"]?.ToString(),
                    Status = rs["status"] != null ? Int32.Parse(rs["status"].ToString()) : -1,
                    Time = rs["time"] != null ? Int32.Parse(rs["time"].ToString()) : -1,
                    Success = rs["status"] != null && rs["status"].ToString() == "0"
                };
                if (!response.Success)
                {
                    Log.Error(response.Message);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is TimeoutException)
                {
                    ex = ex.InnerException;
                }
                else if (ex is TaskCanceledException)
                {
                    if ((ex as TaskCanceledException).CancellationToken == null || (ex as TaskCanceledException).CancellationToken.IsCancellationRequested == false)
                    {
                        ex = new TimeoutException("Timeout occurred");
                    }
                }

                response = new OcrVbhcResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Success = false
                };
                Log.Error(ex, ex.Message);
            }

            return response;
        }

        public async Task<OcrVbhcAiResponse> BmBocTachVbhcAi(OcrRequest model)
        {
            model.Type = OcrRequestType.vbhc_tm;
            model.ApiKey = _apiKey;

            OcrVbhcAiResponse response;
            try
            {
                var client = _clientFatory.Create();
                var rs = await client.PostWithFilePublicAsync<Hashtable>(_apiExternalOcrEndpoint, null, nameof(model.FileDocument), model.FileDocument, model);
                var data = rs["data"] != null
                    ? JsonConvert.DeserializeObject<List<OcrVbhcAiData>>(rs["data"].ToString())
                    : null;
                if (data != null && EnumerableExtensions.Any(data))
                {
                    // Chỉ lấy các dữ liệu IsVBHCField = true
                    data = data.Where(x => x.IsVBHCField).ToList();
                }
                response = new OcrVbhcAiResponse
                {
                    Data = data,
                    Message = rs["messages"]?.ToString(),
                    Status = rs["status"] != null ? Int32.Parse(rs["status"].ToString()) : -1,
                    Time = rs["time"] != null ? Int32.Parse(rs["time"].ToString()) : -1,
                    Success = rs["status"] != null && rs["status"].ToString() == "0"
                };
                if (!response.Success)
                {
                    Log.Error(response.Message);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is TimeoutException)
                {
                    ex = ex.InnerException;
                }
                else if (ex is TaskCanceledException)
                {
                    if ((ex as TaskCanceledException).CancellationToken == null || (ex as TaskCanceledException).CancellationToken.IsCancellationRequested == false)
                    {
                        ex = new TimeoutException("Timeout occurred");
                    }
                }

                response = new OcrVbhcAiResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Success = false
                };
                Log.Error(ex, ex.Message);
            }

            return response;
        }

        public async Task<OcrCmndResponse> BmBocTachCmnd(OcrRequest model)
        {
            model.Type = OcrRequestType.CMND;
            model.ApiKey = _apiKey;

            OcrCmndResponse response;
            try
            {
                var client = _clientFatory.Create();
                var rs = await client.PostWithFilePublicAsync<Hashtable>(_apiExternalOcrEndpoint, null, nameof(model.FileDocument), model.FileDocument, model);
                response = new OcrCmndResponse
                {
                    Data = rs["data"] != null ? JsonConvert.DeserializeObject<OcrCmndData>(rs["data"].ToString()) : null,
                    Message = rs["messages"]?.ToString(),
                    Status = rs["status"] != null ? Int32.Parse(rs["status"].ToString()) : -1,
                    Time = rs["time"] != null ? Int32.Parse(rs["time"].ToString()) : -1,
                    Success = rs["status"] != null && rs["status"].ToString() == "0"
                };
                if (!response.Success)
                {
                    Log.Error(response.Message);
                }
            }
            catch (Exception ex)
            {
                response = new OcrCmndResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Success = false
                };
                Log.Error(ex, ex.Message);
            }

            return response;
        }

        public async Task<OcrSegmentResponse> OcrWithSegment(OcrSegmentRequest model)
        {
            // tranform model OcrSegmentRequest to OcrRequest
            var request = SegmentHelper.ParseCoordinateArea(model.CoordinateArea);
            request.FileDocument = model.FileDocument;
            request.Type = OcrRequestType.form;
            request.ApiKey = _apiKey;
            request.Field = "ocr";
            request.FieldType = 1;    // hoặc 2 cho chữ in v2

            OcrSegmentResponse response;
            try
            {
                var client = _clientFatory.Create();
                var rs = await client.PostWithFilePublicAsync<Hashtable>(_apiExternalOcrEndpoint, null, nameof(request.FileDocument), request.FileDocument, request);
                response = new OcrSegmentResponse
                {
                    Data = rs["data"] != null ? rs["data"].ToString() : null,
                    Message = rs["messages"]?.ToString(),
                    Status = rs["status"] != null ? Int32.Parse(rs["status"].ToString()) : -1,
                    Time = rs["time"] != null ? Int32.Parse(rs["time"].ToString()) : -1,
                    Success = rs["status"] != null && rs["status"].ToString() == "0"
                };
                if (!response.Success)
                {
                    Log.Error(response.Message);
                }
            }
            catch (Exception ex)
            {
                response = new OcrSegmentResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Success = false
                };
                Log.Error(ex, ex.Message);
            }

            return response;
        }

        public async Task<OcrSegmentResponse> OcrSegmentFull(OcrRequest model)
        {
            model.Type = OcrRequestType.FULLTEXT;
            model.ApiKey = _apiKey;

            OcrSegmentResponse response;
            try
            {
                var client = _clientFatory.Create();
                var rs = await client.PostWithFilePublicAsync<Hashtable>(_apiExternalOcrEndpoint, null, nameof(model.FileDocument), model.FileDocument, model);
                var data = rs["data"] != null ? JsonConvert.DeserializeObject<Hashtable>(rs["data"].ToString()) : null;
                response = new OcrSegmentResponse
                {
                    Data = data != null ? data["Text"].ToString() : null,
                    Message = rs["messages"]?.ToString(),
                    Status = rs["status"] != null ? Int32.Parse(rs["status"].ToString()) : -1,
                    Time = rs["time"] != null ? Int32.Parse(rs["time"].ToString()) : -1,
                    Success = rs["status"] != null && rs["status"].ToString() == "0"
                };
                if (!response.Success)
                {
                    Log.Error(response.Message);
                }
            }
            catch (Exception ex)
            {
                response = new OcrSegmentResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Success = false
                };
                Log.Error(ex, ex.Message);
            }

            return response;
        }
    }
}

using Microsoft.Extensions.Configuration;
using OneAPI;
using System.Diagnostics;
using System.IO;
using System;
using System.Net.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using AxDesApi;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;


public class Program
{
    private static IConfiguration _configuration { get; set; }
    private static IHttpClientFactory _httpClientFactory { get; set; }
    private static HttpClient _httpClient { get; set; }
    private static void Init(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        _configuration = builder.Build();

        _httpClient = new HttpClient();

        Log.Logger = new LoggerConfiguration()
                  .ReadFrom.Configuration(_configuration)
                  .CreateLogger();

    }
    private static object CallAXDirect(string filePath)
    {
        var result = APIs.FormAPI.ExtractTuPhapKhaiSinh(filePath);
        return result;
    }
    private static HttpResponseMessage CallAXOverAPIWrapper(string address, string filePath)
    {
        var apiEndPoint = "home/ocr/tuphap-khaisinh";
        var requestUri = new Uri(new Uri(address), apiEndPoint);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
        var requestBody = new Dictionary<string, string>();
        requestBody.Add("filePath", filePath);
        var httpContentStr = JsonConvert.SerializeObject(requestBody);

        requestMessage.Content = new StringContent(httpContentStr, Encoding.UTF8, "application/json");

        var response = _httpClient.SendAsync(requestMessage).Result;
        return response;
    }
    private static bool CheckAPIAddress(string address)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        var response = _httpClient.SendAsync(request).Result;
        return response.IsSuccessStatusCode;
    }

    public static void RunAx()
    {
        Console.WriteLine("Start Run AXSDK...");
        try
        {
            var axsdk_address = _configuration["AxSDKAddress"];
            var dataFolder = _configuration["DataFolder:Path"];
            var dataFolder_Filter = _configuration["DataFolder:Filter"];
            var numOfFileSimulation = long.Parse(_configuration["NumOfFileSimulation"]);
            var maxConcurrency = int.Parse(_configuration["MaxConcurrency"] ?? "5"); // Thêm tham số trong appsettings nếu muốn

            if (string.IsNullOrEmpty(dataFolder_Filter))
            {
                dataFolder_Filter = "*.*";
            }

            var apiAddress = _configuration["APIAddress"];
            string axServer = string.Empty;
            bool isUsingAPI = string.IsNullOrEmpty(_configuration["IsUseAPI"]) ? false : bool.Parse(_configuration["IsUseAPI"]);
            if (isUsingAPI)
            {
                axServer = apiAddress;
                Log.Information($"Connecting API at address: {apiAddress}... ");
                var checkconnect = CheckAPIAddress(apiAddress);
                Log.Information($"Connected!");
            }
            else
            {
                axServer = axsdk_address;
                Log.Information($"Connecting AX at address: {axsdk_address}... ");
                APIs.SetServerAddress(axsdk_address);
                var checkLicnese = APIs.CheckLicense();
                Log.Information($"Connected! CheckLicense status: {checkLicnese}");
            }

            var directoryInfo = new DirectoryInfo(dataFolder);
            var allFiles = directoryInfo.GetFiles(dataFolder_Filter, SearchOption.AllDirectories).Take((int)numOfFileSimulation).ToList();

            Log.Information($"Total file to process: {allFiles.Count} in {dataFolder}");
            Log.Information("--------------------------------------------");

            var totalTime = 0L;
            var totalSuccess = 0;
            var lockObj = new object();

            Parallel.ForEachAsync(allFiles, new ParallelOptions { MaxDegreeOfParallelism = maxConcurrency }, async (file, _) =>
            {
                var sw = Stopwatch.StartNew();
                bool isSuccess = false;
                string message = string.Empty;
                try
                {
                    Log.Information($"--> [{DateTime.Now:HH:mm:ss}] Start ocr: {file.Name}");
                    object result;
                    if (isUsingAPI)
                    {
                        result = await Task.Run(() => CallAXOverAPIWrapper(apiAddress, file.FullName));
                        if (result is HttpResponseMessage httpResponseMessage)
                        {
                            var status = httpResponseMessage.StatusCode;
                            message = httpResponseMessage.Content.ReadAsStringAsync().Result;
                            if (status != HttpStatusCode.OK)
                            {

                                isSuccess = false;
                            }
                            else
                            {
                                isSuccess = true;
                            }

                        }
                    }
                    else
                    {
                        result = await Task.Run(() => CallAXDirect(file.FullName));

                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"ERROR processing {file.Name}: {ex.Message}");
                }
                finally
                {
                    sw.Stop();
                    lock (lockObj)
                    {
                        totalTime += sw.ElapsedMilliseconds;
                        if (isSuccess) totalSuccess++;
                    }
                    string decorateMesssage = string.Empty;
                    string msg = $"--> [{DateTime.Now:HH:mm:ss}] End ocr: {file.Name} - Message: {message} - Success: {isSuccess} - Duration: {sw.ElapsedMilliseconds} ms";
                    if (isSuccess)
                    {
                        Log.Information(msg);
                    }
                    else
                    {
                        Log.Warning(msg);
                    }

                }
            }).Wait();

            Log.Information("--------------------------------------------");
            var avgTime = totalSuccess > 0 ? totalTime / totalSuccess : 0;
            Log.Information($"Processed {totalSuccess}/{allFiles.Count} files - Total time: {totalTime / 1000}s - Avg: {avgTime} ms/file");
            Log.Information("Done.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Exception occurred: {ex.Message} ");
        }
    }


    public static void TestAxDes()
    {
        try
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Start Run TestAxDes...");
            var axsdk_address = "172.16.15.131";
            var modelFolder = @"\\172.16.15.131\filedata\axdes-model\cw.giaychungnhanhokinhdoanh.zip";


            string filePath = @"\\172.16.15.131\filedata\FileTemp\giay-cn-ho-kd\10.pdf";

            AxDesApiManager.SetHost(axsdk_address);
            var sw = new Stopwatch();
            sw.Restart();
            Console.WriteLine("Start calling  BocTachTheoMoHinh() ...");
            var result = AxDesApi.Extraction.BocTachTheoMoHinh(modelFolder, filePath, string.Empty, isKeyByNameField: true);
            sw.Stop();
            Console.WriteLine($"End calling  BocTachTheoMoHinh() \t Duration: {sw.ElapsedMilliseconds} ms \t Result below:");
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    public static void TestRateLimit()
    {
        var url = "https://localhost:44357/Home/TestRateLimit";
        int totalRequests = 20;
        var httpClientHandler = new HttpClientHandler
        {
            // Bỏ qua SSL self-signed (nếu dùng localhost dev)
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        var httpClient = new HttpClient(httpClientHandler);

        var tasks = Enumerable.Range(1, totalRequests);

        Console.WriteLine($"Sending {totalRequests} requests to {url}...\n");
        try
        {


            var response = httpClient.GetAsync(url).Result;
            var result = response.Content.ReadAsStringAsync();
            var status = response.IsSuccessStatusCode ? "✅" : "❌";
            Console.WriteLine($"Initial request status: {status} - {result}\n");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Initial request failed: {ex.Message}");
            return;
        }
        Parallel.ForEachAsync(tasks, new ParallelOptions { MaxDegreeOfParallelism = totalRequests }, async (i, _) =>
        {
            try
            {
                var response = httpClient.GetAsync(url).Result;
                var result = await response.Content.ReadAsStringAsync();

                var status = response.IsSuccessStatusCode ? "✅" : "❌";
                Console.WriteLine($"[{i:00}] {status} Status: {response.StatusCode} - {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{i:00}] ❌ Exception: {ex.Message}");
            }
        });
    }
    public static void Main(string[] args)
    {
        Init(args);
        RunAx();
        //TestAxDes();
        //TestRateLimit();
    }


}
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
        
    }
    private static object CallAXDirect(string filePath)
    {
        var result = APIs.FormAPI.ExtractTuPhapKhaiSinh(filePath);
        return result;
    }
    private static object CallAXOverAPIWrapper(string address, string filePath)
    {
        var apiEndPoint = "home/ocr/tuphap-khaisinh";
        var requestUri = new Uri(new Uri(address), apiEndPoint);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
        var requestBody = new Dictionary<string, string>();
        requestBody.Add("filePath",filePath);
        var httpContentStr = JsonConvert.SerializeObject(requestBody);

        requestMessage.Content = new StringContent(httpContentStr, Encoding.UTF8, "application/json");

        var response = _httpClient.SendAsync(requestMessage).Result;
        return response.Content;
    }
    private static bool CheckAPIAddress(string address)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        var response = _httpClient.SendAsync(request).Result;
        return response.IsSuccessStatusCode;
    }

    public static void RunAx()
    {
        try
        {
            var axsdk_address = _configuration["AxSDKAddress"];
            var dataFolder = _configuration["DataFolder:Path"];
            var dataFolder_Filter = _configuration["DataFolder:Filter"];
            if (string.IsNullOrEmpty(dataFolder_Filter))
            {
                dataFolder_Filter = "*.*";
            }

            var apiAddress = _configuration["APIAddress"];
            bool isUsingAPI = string.IsNullOrEmpty(_configuration["IsUseAPI"]) ? false : bool.Parse(_configuration["IsUseAPI"]);
            if (isUsingAPI)
            {
                Console.Write($"Connecting API at address: {apiAddress}... ");
                var checkconnect = CheckAPIAddress(apiAddress);
                Console.WriteLine($"Connected!");
            }
            else
            {

                Console.Write($"Connecting AX at address: {axsdk_address}... ");
                APIs.SetServerAddress(axsdk_address);
                var checkLicnese = APIs.CheckLicense();
                Console.WriteLine($"Connected! CheckLicense status: {checkLicnese}");
            }
            var directoryInfo = new DirectoryInfo(dataFolder);

            var totalFile = directoryInfo.GetFiles(dataFolder_Filter, SearchOption.AllDirectories).Length;
            Console.WriteLine($"Total file: {totalFile} in {dataFolder}");
            Console.WriteLine("--------------------------------------------");
            var sw = new Stopwatch();
            long totalTime = 0;
            var fileIndx = 0;
            foreach (var file in directoryInfo.GetFiles(dataFolder_Filter, SearchOption.AllDirectories))
            {
                try
                {
                    fileIndx++;
                    Console.WriteLine($"--> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Start ocr file {file.Name} - File {fileIndx}/{totalFile}");
                    sw.Restart();
                    object result;
                    if (isUsingAPI)
                    {
                        result = CallAXOverAPIWrapper(apiAddress, file.FullName);
                    }
                    else
                    {
                        result = CallAXDirect(file.FullName);
                    }
                    sw.Stop();
                    totalTime += sw.ElapsedMilliseconds;
                    Console.WriteLine($"--> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] End ocr file {file.Name} - Duration: {sw.ElapsedMilliseconds} ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }

            Console.WriteLine("--------------------------------------------");
            var totalSecond = totalTime / 1000;
            var everageTime = totalTime / totalFile;
            Console.WriteLine($"Process total file {totalFile} in {totalSecond} s - Everage: {everageTime} ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message} ");
            Console.WriteLine(ex.StackTrace);
        }
    }

    public static void Main(string[] args)
    {
        Init(args);
        RunAx();
    }


}
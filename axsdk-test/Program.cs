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
        requestBody.Add("filePath", filePath);
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
        Console.WriteLine("Start Run AXSDK...");
        try
        {
            var axsdk_address = _configuration["AxSDKAddress"];
            var dataFolder = _configuration["DataFolder:Path"];
            var dataFolder_Filter = _configuration["DataFolder:Filter"];
            var numOfFileSimulation = long.Parse(_configuration["NumOfFileSimulation"]);
           
            if (string.IsNullOrEmpty(dataFolder_Filter))
            {
                dataFolder_Filter = "*.*";
            }

            var apiAddress = _configuration["APIAddress"];
            string axServer=string.Empty;
            bool isUsingAPI = string.IsNullOrEmpty(_configuration["IsUseAPI"]) ? false : bool.Parse(_configuration["IsUseAPI"]);
            if (isUsingAPI)
            {
                axServer = apiAddress;
                Console.Write($"Connecting API at address: {apiAddress}... ");
                var checkconnect = CheckAPIAddress(apiAddress);
                Console.WriteLine($"Connected!");
            }
            else
            {
                axServer=axsdk_address;
                Console.Write($"Connecting AX at address: {axsdk_address}... ");
                APIs.SetServerAddress(axsdk_address);
                var checkLicnese = APIs.CheckLicense();
                Console.WriteLine($"Connected! CheckLicense status: {checkLicnese}");
            }
            var directoryInfo = new DirectoryInfo(dataFolder);
            
            var totalFile = directoryInfo.GetFiles(dataFolder_Filter, SearchOption.AllDirectories).Length;
            if (numOfFileSimulation < totalFile)
            {
                numOfFileSimulation = totalFile;
            }
            Console.WriteLine($"Total file: {totalFile} in {dataFolder}");
            Console.WriteLine($"Number of file simulation: {numOfFileSimulation}");
            Console.WriteLine("--------------------------------------------");
            var sw = new Stopwatch();
            long totalTime = 0;
            var fileIndx = 0;
            var isStop = false;
            var isOCRSuccess = false;
            while (!isStop)
            {
                foreach (var file in directoryInfo.GetFiles(dataFolder_Filter, SearchOption.AllDirectories))
                {

                    try
                    {
                        sw.Restart();
                        
                        fileIndx++;
                        Console.WriteLine($"--> \t[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] \tStart ocr file \t{file.Name} \tServer: \t{axServer} \tFile \t{fileIndx}/{numOfFileSimulation}");
                        object result;
                        if (isUsingAPI)
                        {
                            result = CallAXOverAPIWrapper(apiAddress, file.FullName);
                        }
                        else
                        {
                            result = CallAXDirect(file.FullName);
                        }
                        isOCRSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        isOCRSuccess = false;
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        sw.Stop();
                        totalTime += sw.ElapsedMilliseconds;
                        Console.WriteLine($"--> \t[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] \tEnd ocr file \t{file.Name} \tSuccess: {isOCRSuccess} \tDuration: \t{sw.ElapsedMilliseconds} ms");
                        
                    }
                }
                if (fileIndx >= numOfFileSimulation)
                {
                    isStop = true;
                }

                Console.WriteLine("--------------------------------------------");
                var totalSecond = totalTime / 1000;
                var everageTime = totalTime / fileIndx;
                Console.WriteLine($"Process total file {fileIndx} in {totalSecond} s - Everage: {everageTime} ms");
                Console.WriteLine("--------------------------------------------");

            }

            Console.WriteLine("Done");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message} ");
            Console.WriteLine(ex.StackTrace);
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
            Console.WriteLine(JsonConvert.SerializeObject(result,Formatting.Indented));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    public static void Main(string[] args)
    {
        Init(args);
        //RunAx();
        TestAxDes();
    }


}
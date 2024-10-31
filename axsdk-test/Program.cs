using Microsoft.Extensions.Configuration;
using AX.AXSDK;
using OneAPI;
using System.Diagnostics;
using System.IO;
using System;

public class Program
{
    public static void RunAx()
    {
        try
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            var axsdk_address = config["AxSDKAddress"];
            var dataFolder = config["DataFolder"];
            Console.Write($"Connecting AX at address: {axsdk_address}... ");
            APIs.SetServerAddress(axsdk_address);
            var checkLicnese = APIs.CheckLicense();
            Console.WriteLine($"Connected! CheckLicense status: {checkLicnese}");
            
            var directoryInfo = new DirectoryInfo(dataFolder);
            
            var totalFile = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).Length;
            Console.WriteLine($"Total file: {totalFile} in {dataFolder}"); ;
            Console.WriteLine("--------------------------------------------");
            var sw = new Stopwatch();
            long totalTime = 0;
            var fileIndx = 0;
            foreach (var file in directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
            {
                try
                {
                    fileIndx++;
                    Console.WriteLine($"--> [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Start ocr file {file.Name} - File {fileIndx}/{totalFile}");
                    sw.Restart();
                    var result = APIs.FormAPI.ExtractTuPhapKhaiSinh(file.FullName);
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
            var everageTime = totalTime/totalFile;
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
        RunAx();
    }

    
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using AnomalyDetectionWebService.Controllers;
using System.Threading.Tasks;

namespace AnomalyDetectionWebService
{
    public class Program
    {
        public static readonly string NormalModelCSVFolder = "NormalModelsDB" + System.IO.Path.DirectorySeparatorChar;
        public static void Main(string[] args)
        {
            if(!System.IO.Directory.Exists(NormalModelCSVFolder))
            {
                Console.WriteLine("Error: Unable to find folder "+NormalModelCSVFolder);
                Console.WriteLine("       Which in use for database/IO for models storage");
                return;
            }

            Console.WriteLine("Do you want to restore prev Normal Models?");
            Console.WriteLine("y         yes");
            Console.WriteLine("n         no, but they will stay in folder" + NormalModelCSVFolder);
            Console.WriteLine("remove    no, and ALL csv file in " + NormalModelCSVFolder + " will be removed!");
            Console.Write("Enter option: ");

            string input = Console.ReadLine().ToLower().Trim();
            while (input != "y" && input != "n" && input != "remove")
            {
                Console.Write("Enter option: ");
                input = Console.ReadLine().ToLower().Trim();
            }

            List<MODEL> initList = new List<MODEL>();
            if (input == "y") {
                try
                {
                    foreach (string file in System.IO.Directory.GetFiles(NormalModelCSVFolder, "*.csv"))
                        initList.Add(IO_Util.RestoreExtendedModelInfo(file).info);
                } catch
                {
                    Console.WriteLine("Error: unable to load csv files.");
                    initList = new List<MODEL>();
                }
            }
            if (input == "remove")
            {
                try
                {
                    foreach (string file in System.IO.Directory.GetFiles(NormalModelCSVFolder, "*.csv"))
                        System.IO.File.Delete(file);
                }
                catch
                {
                    Console.WriteLine("Error: unable to delete csv files.");
                }
            }
            Console.WriteLine();
            AnomalyDetectionController.InitADM(new AnomalyDetectorsManager(initList));

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

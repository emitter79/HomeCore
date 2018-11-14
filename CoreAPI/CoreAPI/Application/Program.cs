using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreHardware.Interfaces;
using CoreHardware.Models;
using CoreHardware.Enumerations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CoreAPI
{
    public class Program
    {
        public static ISystemNode systemNode { get; set; }

        public static void Main(string[] args)
        {
            string port = string.Empty;
            var platform = System.Environment.OSVersion.Platform.ToString().ToUpper();
            if (platform == "UNIX") port = "/dev/ttyUSB0";
            else port = "COM7";

            systemNode = new SystemNode(port, PortSpeeds._115200, ProcessorTypes.ATMEGA_328);
            if (systemNode.IsConnected)
            {
                Console.WriteLine("System node connected on: {0}", systemNode.PortName);
            }
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseWebRoot("wwwroot")
                .UseUrls("http://*:5000")
                .Build();
    }
}
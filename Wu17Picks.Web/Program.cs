﻿using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Wu17Picks.Web
{
    public class Program
    {

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();


        //TODO Nlog Config not working with entity framework

        //public static void Main(string[] args)
        //{
        //    var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        //    try
        //    {
        //        logger.Debug("init main");
        //        BuildWebHost(args).Build().Run();
        //    }
        //    catch (Exception exception)
        //    {
        //        //NLog: catch setup errors
        //        logger.Error(exception, "Stopped program because of exception");
        //        throw;
        //    }
        //    finally
        //    {
        //        // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
        //        NLog.LogManager.Shutdown();
        //    }
        //}

        //public static IWebHostBuilder BuildWebHost(string[] args) =>
        //     WebHost.CreateDefaultBuilder(args)
        //         .UseStartup<Startup>()
        //         .ConfigureLogging(logging =>
        //         {
        //             logging.ClearProviders();
        //             logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        //         })
        //         .UseNLog();  // NLog: Setup NLog for Dependency injection
    }
}

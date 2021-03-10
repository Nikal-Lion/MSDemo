using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MS.DbContexts;
using MS.UnitOfWork;
using MS.WebCore.Logger;
using Serilog;
using System;
#if !DEBUG
using System.Diagnostics;
#endif
using System.IO;

namespace MS.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
             .AddEnvironmentVariables()
             .Build();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
#if !DEBUG
            Serilog.Debugging.SelfLog.Enable(msg =>
                Debug.WriteLine(msg)
            );
#endif
            try
            {
                var host = CreateHostBuilder(args).Build();
                Log.Logger.Information("网站启动中...");
                using (IServiceScope scope = host.Services.CreateScope())
                {
                    Log.Logger.Information("初始化NLog");
                    //确保NLog.config中连接字符串与appsettings.json中同步
                    NLogExtensions.EnsureNlogConfig("NLog.config", "MySQL", Configuration.GetConnectionString("MSDbContext"));

                    Log.Logger.Information("初始化数据库");
                    //初始化数据库
                    DBSeed.Initialize(scope.ServiceProvider.GetRequiredService<IUnitOfWork<MSDbContext>>());
                }
                Log.Logger.Information("网站启动完成");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "网站启动失败");
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())//替换autofac作为DI容器
            .UseSerilog((context, logger) =>
            {
                //read serilog configuration from current context configurations
                logger.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
#if DEBUG
                .UseEnvironment("Development")
#endif
                .UseStartup<Startup>();
            })
            //.AddNlogService()
            ;
    }
}

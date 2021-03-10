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
                Log.Logger.Information("��վ������...");
                using (IServiceScope scope = host.Services.CreateScope())
                {
                    Log.Logger.Information("��ʼ��NLog");
                    //ȷ��NLog.config�������ַ�����appsettings.json��ͬ��
                    NLogExtensions.EnsureNlogConfig("NLog.config", "MySQL", Configuration.GetConnectionString("MSDbContext"));

                    Log.Logger.Information("��ʼ�����ݿ�");
                    //��ʼ�����ݿ�
                    DBSeed.Initialize(scope.ServiceProvider.GetRequiredService<IUnitOfWork<MSDbContext>>());
                }
                Log.Logger.Information("��վ�������");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "��վ����ʧ��");
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
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())//�滻autofac��ΪDI����
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

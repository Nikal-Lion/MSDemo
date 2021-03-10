using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MS.Common.IDCode;
using MS.Entities.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MS.WebCore
{
    public static class WebCoreExtensions
    {
        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        /// <summary>
        /// 添加跨域策略，从appsetting中读取配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    string[] allowOrigins = configuration.GetSection("Startup:Cors:AllowOrigins").Get<string[]>();
                    builder
                    .WithOrigins(allowOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    ;
                });
            });
            return services;
        }

        /// <summary>
        /// 注册WebCore服务，配置网站
        /// do other things
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebCoreService(this IServiceCollection services, IConfiguration configuration)
        {
            //绑定appsetting中的SiteSetting
            services.Configure<SiteSetting>(configuration.GetSection(nameof(SiteSetting)));

            #region 单例化雪花算法
            string workIdStr = configuration.GetSection("SiteSetting:WorkerId").Value;
            string datacenterIdStr = configuration.GetSection("SiteSetting:DataCenterId").Value;
            long workId;
            long datacenterId;
            try
            {
                workId = long.Parse(workIdStr);
                datacenterId = long.Parse(datacenterIdStr);
            }
            catch (Exception)
            {
                throw;
            }
            IdWorker idWorker = new IdWorker(workId, datacenterId);
            services.AddSingleton(idWorker);

            #endregion
            return services;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddJsonDateTimeConverter(this IMvcBuilder builder)
        {
            builder.AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:MM:ss";
                // 设置自定义时间戳格式
                opt.SerializerSettings.Converters = new List<JsonConverter>
                 {
                     new Converters.TimeConverter()
                 };
                // 设置下划线方式，首字母是小写
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
            return builder;
        }
        /// <summary>
        /// swagger extension
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                //设置swagger文档相关信息
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ms demo 文档",
                    Version = "v1"
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                        },
                        new List<string> ()
                    }
                });
                options.AddServer(new OpenApiServer()
                {
                    Url = "http://localhost:5000",
                    Description = "本地"
                });

                //实体层的xml文件名
                string xmlEntityPath = GetXmlAbsolutePath(typeof(IEntity).Assembly.GetName().Name);
                if (File.Exists(xmlEntityPath))
                {
                    options.IncludeXmlComments(xmlEntityPath);
                }
                //var xmlPath = GetXmlAbsolutePath(Assembly.GetExecutingAssembly().GetName().Name); 因当前文件不在WebApi中，所以获取当前执行assembly name不是WebApi，胆汁异常
                var xmlPath = GetXmlAbsolutePath("MS.WebApi");
                if (!File.Exists(xmlPath))
                    throw new InvalidOperationException("The XML file does not exist for Swagger - see link above for more info.");
                options.IncludeXmlComments(xmlPath);

                options.CustomOperationIds(apiDesc =>
                {
                    var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                    return $"{controllerAction.ControllerName}-{controllerAction.ActionName}-{ controllerAction.GetHashCode()}";
                });
            });
            return services;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        public static string GetXmlAbsolutePath(string xmlFileName)
        {
            string xmlFile = $"{xmlFileName}.xml";
            return Path.Combine(AppContext.BaseDirectory, xmlFile);
        }
    }
}

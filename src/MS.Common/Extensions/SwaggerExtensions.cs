using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MS.Common.Extensions
{
    public static class SwaggerExtensions
    {

        public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                //设置swagger文档相关信息
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Lssc WebApi 文档",
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
                    Url = "http://localhost:5006",
                    Description = "本地"
                });
                options.AddServer(new OpenApiServer()
                {
                    Url = "http://core.api.localhost.com",
                    Description = "测试环境-本地服务端"
                });
                options.AddServer(new OpenApiServer()
                {
                    Url = "http://content.test.lssc.yssc-cloud.com",
                    Description = "测试环境-测试服务端"
                });
                options.AddServer(new OpenApiServer()
                {
                    Url = "http://core.api.lssc-cloud.com",
                    Description = "正式环境服务端"
                });
                //add VerifyKey Header for LoginRequiredAttribute
                //options.OperationFilter<AddVerifyKeyHeaderParameter>();
                //set enum description 
                options.DocumentFilter<EnumDocumentFilter>();

                //实体层的xml文件名
                string xmlEntityPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(IEntity).Assembly.GetName().Name}.xml");
                if (File.Exists(xmlEntityPath))
                {
                    options.IncludeXmlComments(xmlEntityPath);
                }
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
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

    }
}

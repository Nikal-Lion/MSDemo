using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MS.Component.Aop;
using MS.Component.Jwt;
using MS.DbContexts;
using MS.Middlewares;
using MS.Models.Automapper;
using MS.Services;
using MS.UnitOfWork;
using MS.WebApi.Filters;
using MS.WebCore;
using MS.WebCore.MultiLanguages;

namespace MS.WebApi
{
    public class Startup
    {
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}
        /// <summary>
        /// 
        /// </summary>
        public ILifetimeScope AutofacContainer { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        public Startup(IWebHostEnvironment env)
        {
            // In ASP.NET Core 3.0 `env` will be an IWebHostingEnvironment, not IHostingEnvironment.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        //���autofac��DI��������
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //ע��IBaseService��IRoleService�ӿڼ���Ӧ��ʵ����
            //builder.RegisterType<BaseService>().As<IBaseService>().InstancePerLifetimeScope();
            //builder.RegisterType<RoleService>().As<IRoleService>().InstancePerLifetimeScope();

            //ע��aop������ 
            //��ҵ���������ƴ��˽�ȥ����ҵ���ӿں�ʵ������ע�ᣬҲ��ҵ�������������˴���
            builder.AddAopService(ServiceExtensions.GetAssemblyName());
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //��Ӷ����Ա��ػ�֧��
            services.AddMultiLanguages();
            services.AddSwaggerGen();

            services
                .AddControllers(options =>
                {
                    options.Filters.Add<ApiResultFilter>();
                    options.Filters.Add<ApiExceptionFilter>();
                })
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));//��ע����ӱ��ػ���Դ�ṩ��Localizerprovider
                })
                .AddJsonDateTimeConverter();

            //ע��������
            services.AddCorsPolicy(Configuration);
            //ע��webcore������վ��Ҫ���ã�
            services.AddWebCoreService(Configuration);

            //ע�Ṥ����Ԫ��ͬʱע����DBContext��
            services.AddUnitOfWorkService<MSDbContext>(options =>
            {
                string connectionString = Configuration.GetConnectionString("MSDbContext");
                ServerVersion serverVersion = ServerVersion.AutoDetect(connectionString);
                options.UseMySql(connectionString, serverVersion);
            });

            //ע��automapper����
            services.AddAutomapperService();

            //ע��jwt����
            services.AddJwtService(Configuration);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
#if NETCOREAPP3_0_OR_GREATER
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#else
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#endif
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                //Enable middleware to serve swagger - ui(HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ms demo V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseMultiLanguage(Configuration);//��Ӷ����Ա��ػ�֧��

            app.UseOptionsRequestHandler();

            app.UseCors(WebCoreExtensions.MyAllowSpecificOrigins);//��ӿ���

            app.UseAuthentication();//�����֤�м��

            app.UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}

using Microsoft.AspNetCore.Builder;
using MS.Middlewares.Components;

namespace MS.Middlewares
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// 请求日志记录中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggerMiddleware>();
        }
        /// <summary>
        /// 处理Get的Options请求
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOptionsRequestHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<OptionsMiddleware>();
        }
    }
}

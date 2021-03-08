using MS.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Ms.Middleware
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
    }
}

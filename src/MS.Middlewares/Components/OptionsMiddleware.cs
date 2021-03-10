using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MS.Middlewares.Components
{
    public class OptionsMiddleware
    {
        #region Private fields

        private readonly RequestDelegate _next;

        #endregion
        public OptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            AppendCorsResponseHeaders(context);
            await (IsOptionsRequest(context.Request.Method) ? context.Response.WriteAsync(string.Empty) : _next.Invoke(context));
        }
        /// <summary>
        /// append cors response headers
        /// </summary>
        /// <param name="context"></param>
        private void AppendCorsResponseHeaders(HttpContext context)
        {
            string origin = context.Request.Headers["Origin"].ToString();
            context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { origin });
            context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "*" });
            context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "*" });
            context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private static bool IsOptionsRequest(string httpMethod)
        {
            return httpMethod == HttpMethods.Options.ToString();
        }
    }
}
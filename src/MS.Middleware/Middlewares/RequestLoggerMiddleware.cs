using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using MS.Middlewares.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MS.Middlewares
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggerMiddleware> _logger;
        private readonly Dictionary<string, string> fileExts;
        private readonly List<string> urlFilters;
        private readonly List<string> headerKeys;

        public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;

            fileExts ??= new Dictionary<string, string>
            {
                { ".css", "text/css" },
                { ".js", "application/x-javascript" },
                { ".htm", "text/html" },
                { ".html", "text/html" },
                { ".manifest", "text/cache-manifest" },

                { ".bmp", "image/bmp" },
                { ".gif", "image/gif" },
                { ".jpeg", "image/jpeg" },
                { ".jpg", "image/jpg" },
                { ".png", "image/png" },
                { ".svg", "image/svg+xml" },

                { ".avi", "video/avi" },
                { ".ico", "image/x-icon" },
                { ".doc", "application/msword" },
                { ".pdf", "application/pdf" },
                { ".rmvb", "application/vnd.rn-realmedia-vbr" },
                { ".swf", "application/x-shockwave-flash" },
                { ".xls", "application/-excel" },
                { ".xml", "text/xml" },

                { ".apk", "application/octet-stream" },
                { ".zip", "application/x-zip-compressed" },
                { ".rar", "application/x-rar-compressed" },
                { ".txt", "text/plain" },

                { ".woff", " font/x-font-woff" }
            };

            urlFilters ??= new List<string>()
            {
                "/appapis/homeget/gethomepagedata",
                "/appapis/homeget/gethomepagedata_v308",
                "/appapis/homeget/guessyoulike",
                "/appapis/homeget/getheadcommoditys",

                "/apis/home/guessyoulike",
                "/apis/home/gethomepagedata",

                "/apis/commodity/getmallcategory",

                "/lsapis/user/getpersonaldatanew",

                "/files/files/",
                "/files/image/swipingimage",

                "/account/lslogincoderesult"
            };

            headerKeys ??= new List<string>
            {
                "host",
                "x-real-ip",
                "x-forwarded-for",
                "user-agent",
                "content-length",
                "ls-platform",
                "vercode",
                "ls-apiversion",
                "verifykey",
            };
        }
        public async Task Invoke(HttpContext context)
        {
            var logDto = new RequestResponseLog(_logger);
            await HandleRequestLogAsync(context.Request, logDto);
            logDto.PrintRequest();

            var originalBody = context.Response.Body;
            using var fakeBody = EnableReadAsync(context.Response);

            await _next.Invoke(context);

            await HandleResponseLogAsync(logDto, originalBody, fakeBody);
            logDto.PrintResponse();
        }

        /// <summary>
        /// handle response 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="logDto"></param>
        /// <returns></returns>
        private async Task HandleResponseLogAsync(RequestResponseLog logDto, Stream originalBody, Stream fakeBody)
        {
            logDto.ExcuteEndTime = DateTime.Now;
            fakeBody.Position = 0;
            var responseBody = new StreamReader(fakeBody).ReadToEnd();
            logDto.ResponseBody = responseBody;

            fakeBody.Position = 0;
            await fakeBody.CopyToAsync(originalBody);

            await Task.CompletedTask;
        }
        /// <summary>
        /// handle request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        private async Task HandleRequestLogAsync(HttpRequest request, RequestResponseLog log)
        {
            log.Url = $"{request.Host}{request.Path}";
            log.ExcuteStartTime = DateTime.Now;
            log.Method = request.Method;

            var headerDictionary = new Dictionary<string, string>();
            foreach (var key in headerKeys)
            {
                if (request.Headers.ContainsKey(key))
                {
                    headerDictionary[key] = string.Join(";", request.Headers[key].ToArray());
                }
            }
            log.Headers = headerDictionary;

            request.EnableBuffering();
            var body = await ReadBodyAsync(request.ContentLength, request.ContentType, request.Body);
            log.RequestBody = body;

            await Task.CompletedTask;
        }


        private Stream EnableReadAsync(HttpResponse response)
        {
            if (!response.Body.CanRead || !response.Body.CanSeek)
            {
                var fakeResponseStream = new MemoryStream();
                response.Body = fakeResponseStream;
                return fakeResponseStream;
            }
            return response.Body;
        }
        /// <summary>
        /// get request body 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<string> ReadBodyAsync(long? contentLength, string contentType, Stream body)
        {
            if (contentLength.GetValueOrDefault(-1) > 0)
            {
                var encoding = GetEncoding(contentType);
                return await this.ReadStreamAsync(body, encoding).ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// get content type's encoding type
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private Encoding GetEncoding(string contentType)
        {
            var mediaType = string.IsNullOrWhiteSpace(contentType) ? default : new MediaType(contentType);
            var encoding = mediaType.Encoding;
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding;
        }

        /// <summary>
        /// read stream by specified encoding type
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private async Task<string> ReadStreamAsync(Stream stream, Encoding encoding)
        {
            //这里注意Body部分不能随StreamReader一起释放
            stream.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(stream, encoding, true, 1024, leaveOpen: true);
            var str = await sr.ReadToEndAsync();
            stream.Seek(0, SeekOrigin.Begin); //内容读取完成后需要将当前位置初始化，否则后面的InputFormatter会无法读取
            return str;
        }
    }
}
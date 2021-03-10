using Microsoft.Extensions.Logging;
using MS.Common.Extensions;
using MS.Common.IDCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Middlewares.Components.Dto
{
    public class RequestResponseLog
    {
        private readonly ILogger<RequestLoggerMiddleware> logger;
        public RequestResponseLog()
        {

        }
        public RequestResponseLog(ILogger<RequestLoggerMiddleware> logger)
        : this()
        {
            this.logger = logger;
        }
        public string Url { get; set; }
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string Method { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public DateTime ExcuteStartTime { get; set; }
        public DateTime ExcuteEndTime { get; set; }

        #region Private fields
        private long requestTimestamp;
        private long responseTimestamp;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var headerJson = this.Headers.ToJsonString();
            StringBuilder contentBuilder = new StringBuilder("-----------------------------------------\r\n");
            contentBuilder.AppendLine($" request header:{headerJson}");

            if (!string.IsNullOrWhiteSpace(this.RequestBody))
            {
                contentBuilder.AppendLine($" request body:{this.RequestBody}");
            }
            if (!string.IsNullOrWhiteSpace(this.ResponseBody))
            {
                contentBuilder.AppendLine($" response body  :{this.ResponseBody}");
            }

            return contentBuilder.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CreatePrintLog()
        {
            requestTimestamp = this.ExcuteStartTime.ToUnixTimestamp();
            responseTimestamp = this.ExcuteEndTime.ToUnixTimestamp();
            yield return $"[{requestTimestamp}]request headers:{this.Headers.ToJsonString()}";
            if (!string.IsNullOrWhiteSpace(this.RequestBody))
            {
                yield return $"[{requestTimestamp}]request body:{this.RequestBody}";
            }
            if (!string.IsNullOrWhiteSpace(this.ResponseBody))
            {
                yield return $"[{responseTimestamp}]response body:{this.ResponseBody}";
            }
        }
        public void PrintRequest()
        {
            requestTimestamp = this.ExcuteStartTime.ToUnixTimestamp();
            logger.LogInformation($"[{requestTimestamp}]request headers:{this.Headers.ToJsonString()}");
            if (!string.IsNullOrWhiteSpace(this.RequestBody))
            {
                logger.LogInformation($"[{requestTimestamp}]request body:{this.RequestBody}");
            }
        }
        public void PrintResponse()
        {
            if (!string.IsNullOrWhiteSpace(this.ResponseBody))
            {
                responseTimestamp = this.ExcuteEndTime.ToUnixTimestamp();
                logger.LogInformation($"response body:{this.ResponseBody}");
            }
        }
    }
}
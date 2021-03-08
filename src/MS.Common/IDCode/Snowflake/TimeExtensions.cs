using System;

namespace MS.Common.IDCode
{
    public static class TimeExtensions
    {
        public static Func<long> currentTimeFunc = InternalCurrentTimeMillis;

        public static long CurrentTimeMillis()
        {
            return currentTimeFunc();
        }

        public static IDisposable StubCurrentTime(Func<long> func)
        {
            currentTimeFunc = func;
            return new DisposableAction(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

        public static IDisposable StubCurrentTime(long millis)
        {
            currentTimeFunc = () => millis;
            return new DisposableAction(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static long ToLong(this DateTime time)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(Jan1st1970, TimeZoneInfo.Local);
            return (long)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 转为js可用的
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToUnixTimestamp(this DateTime time)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(Jan1st1970, TimeZoneInfo.Local);
            return (long)(time - startTime).TotalMilliseconds;
        }

        private static readonly DateTime Jan1st1970 = new DateTime
           (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static long InternalCurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }
}

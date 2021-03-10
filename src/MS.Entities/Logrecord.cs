using MS.Entities.Core;
using System;

namespace MS.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Logrecord : IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LogDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LogLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Logger { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Exception { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MachineName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MachineIp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NetRequestMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NetRequestUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NetUserIsauthenticated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NetUserAuthtype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NetUserIdentity { get; set; }
    }
}

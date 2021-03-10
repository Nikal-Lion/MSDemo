using MS.Entities.Core;
using System;

namespace MS.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class UserLogin : IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HashedPassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AccessFailedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LockedTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public User User { get; set; }
    }
}

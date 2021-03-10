using System;

namespace MS.Entities.Core
{
    /// <summary>
    /// 没有Id主键的实体继承这个
    /// </summary>
    public interface IEntity
    {
    }
    /// <summary>
    /// 有Id主键的实体继承这个
    /// </summary>
    public abstract class BaseEntity : IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public StatusCode StatusCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? Creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? Modifier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModifyTime { get; set; }
    }
}

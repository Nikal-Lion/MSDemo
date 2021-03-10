using MS.Entities.Core;

namespace MS.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Role : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }
    }
}

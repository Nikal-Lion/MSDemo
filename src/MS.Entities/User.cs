using MS.Entities.Core;

namespace MS.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Role Role { get; set; }
    }
}

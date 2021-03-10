using System.ComponentModel;

namespace MS.Entities.Core
{
    /// <summary>
    /// 
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// 软删除，已删除的无法恢复，无法看见，暂未使用
        /// </summary>
        [Description("已删除")]
        Deleted = -1,
        /// <summary>
        /// 生效
        /// </summary>
        [Description("生效")]
        Enable = 0,
        /// <summary>
        /// 失效的还可以改为生效
        /// </summary>
        [Description("失效")]
        Disable = 1
    }
}

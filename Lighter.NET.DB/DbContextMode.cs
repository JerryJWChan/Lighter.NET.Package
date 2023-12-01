using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lighter.NET.DB
{
    /// <summary>
    /// DbContext模式
    /// </summary>
    [Flags]
    public enum DbContextMode
    {
        /// <summary>
        /// 預設
        /// </summary>
        Default = 0,
        /// <summary>
        /// 不用變更追蹤
        /// </summary>
        NoDetectChange = 1,
        /// <summary>
        /// 不用緩載入
        /// </summary>
        NoLazyLoading = 2
    }
}

namespace Lighter.NET.Common
{

    /// <summary>
    /// 自訂Link引用元素
    /// </summary>
    public class HtmlLinkElement
    {
        /// <summary>
        /// url(例如：CSS檔所在相對路徑)
        /// </summary>
        public string href { get; set; } = "";
        /// <summary>
        /// 引用資源與現有頁面之關係(例如：stylesheet)
        /// </summary>
        public string rel { get; set; } = "stylesheet";
        /// <summary>
        /// 跨域存取原則(例如：anonymous)
        /// </summary>
        public string crossorigin { get; set; } = "";
        /// <summary>
        /// media (mime) type
        /// </summary>
        public string type { get; set; } = "";
        /// <summary>
        /// media query
        /// </summary>
        public string media { get; set; } = "";
        /// <summary>
        /// 完整性驗證碼
        /// </summary>
        public string integrity { get; set; } = "";
        /// <summary>
        /// 轉介站原則
        /// </summary>
        public string referrerpolicy { get; set; } = "";
    }
}

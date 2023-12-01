namespace Lighter.NET.Common
{
    /// <summary>
    /// 自訂Script引用元素
    /// </summary>
    public class HtmlScriptElement
    {
        /// <summary>
        /// url(例如：JS檔所在相對路徑)
        /// </summary>
        public string src { get; set; } = "";
        /// <summary>
        /// 非同步
        /// </summary>
        public string asycn { get; set; } = "";
        /// <summary>
        /// 延遲載入
        /// </summary>
        public string defer { get; set; } = "";
        /// <summary>
        /// 跨域存取原則(例如：anonymous)
        /// </summary>
        public string crossorigin { get; set; } = "";
        /// <summary>
        /// 完整性驗證碼
        /// </summary>
        public string integrity { get; set; } = "";
        /// <summary>
        /// script種類(html5之後不需指定)
        /// </summary>
        public string type { get; set; } = "";
    }
}

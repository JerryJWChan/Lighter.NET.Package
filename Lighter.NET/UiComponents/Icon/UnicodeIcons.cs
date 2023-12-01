using Microsoft.AspNetCore.Html;
namespace Lighter.NET.UiComponents.Icon
{
    /// <summary>
    /// Unicode圖示代碼
    /// </summary>
    public static class UnicodeIcons
    {
        /// <summary>
        /// 新增/加入(加號：+)
        /// </summary>
        public static IHtmlContent Add(string cssClass = "") => UnicodeIcon("&plus;", cssClass);
        /// <summary>
        /// 編輯(一支鉛筆)
        /// </summary>
        public static IHtmlContent Edit(string cssClass = "") => UnicodeIcon("&#9998;", cssClass);
        /// <summary>
        /// 關閉(打叉：x)
        /// </summary>
        public static IHtmlContent Close(string cssClass = "") => UnicodeIcon("&times;", cssClass);
        /// <summary>
        /// 取消(打叉：x)
        /// </summary>
        public static IHtmlContent Cancel(string cssClass = "") => UnicodeIcon("&times;", cssClass);
        /// <summary>
        /// 刪除(打叉：x)
        /// </summary>
        public static IHtmlContent Delete(string cssClass = "") => UnicodeIcon("&times;", cssClass);
        /// <summary>
        /// 刷新/重載(圓形箭頭：順時針)
        /// </summary>
        public static IHtmlContent Reload(string cssClass = "") => UnicodeIcon("&#10227;", cssClass);
        /// <summary>
        /// 方格打勾
        /// </summary>
        public static IHtmlContent SquareBoxChecked(string cssClass = "") => UnicodeIcon("&#128505;",cssClass);
        /// <summary>
        /// 方格空白
        /// </summary>
        public static IHtmlContent SquareBoxEmpty(string cssClass = "") => UnicodeIcon("&#8414;", cssClass); 
        /// <summary>
        /// 儲存(磁碟片)
        /// </summary>
        public static IHtmlContent FloppyDisk(string cssClass = "") => UnicodeIcon("&#128427;", cssClass);
        /// <summary>
        /// Email
        /// </summary>
        public static IHtmlContent Email(string cssClass = "") => UnicodeIcon("&#9989;", cssClass);
        /// <summary>
        /// 列印(Printer)
        /// </summary>
        public static IHtmlContent Print(string cssClass = "") => UnicodeIcon("&#128438;", cssClass);
        /// <summary>
        /// 前往(箭頭向右)
        /// </summary>
        public static IHtmlContent Goto(string cssClass = "") => UnicodeIcon("&#10140;", cssClass);

        /// <summary>
        /// Unicode icon
        /// </summary>
        /// <param name="unicodeEntity">unicode代碼(格式：&#[代碼];)</param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public static IHtmlContent UnicodeIcon(string unicodeEntity, string cssClass = "")
        {
            string html = $"<span class=\"{cssClass}\">{unicodeEntity}</span>";
            return new HtmlString(html);
        }
    }

}

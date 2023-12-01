namespace Lighter.NET.Common
{
    /// <summary>
    /// 連結定義
    /// </summary>
    public class HyperLinkDefinition
    {
        /// <summary>
        /// 連結名稱(識別用)(會轉成：html tag的 data-name屬性)
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 連結文字
        /// </summary>
        public string Text { get; set; } = "";
        /// <summary>
        /// 網址
        /// </summary>
        public string Url { get; set; } = "";
        /// <summary>
        /// css class (多項時以空白分隔)
        /// </summary>
        public string CssClass { get; set; } = "";
        /// <summary>
        /// 開啟目標視窗
        /// </summary>
        public HyperLinkTargetType Target { get; set; } = HyperLinkTargetType.Self;

    }
}

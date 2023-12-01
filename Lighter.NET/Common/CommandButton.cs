namespace Lighter.NET.Common
{
    /// <summary>
    /// 命令按鈕
    /// </summary>
    public class CommandButton
    {
        /// <summary>
        /// 按鈕種類
        /// </summary>
        public ButtonType ButtonType { get; set; } = ButtonType.Button;
        /// <summary>
        /// 命令名稱
        /// </summary>
        public string CommandName { get; set; } = "";
        /// <summary>
        /// 對應的Controller Action
        /// </summary>
        public string ActionName { get; set; } = "";
        /// <summary>
        /// 按鈕是否可視
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// 按鈕文字
        /// </summary>
        public string Text { get; set; } = "";
        /// <summary>
        /// Client端Click事件處理js函式
        /// </summary>
        public string ClickEventHandler { get; set; } = "";
        /// <summary>
        /// Css Class
        /// </summary>
        public string CssClass { get; set; } = "button";
        /// <summary>
        /// 可視性的Css Class
        /// </summary>
        public string VisibleCssClass
        {
            get
            {
                return (Visible) ? "show visible" : "hide hidden";
            }
        }
    }

}

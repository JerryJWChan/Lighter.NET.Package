namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 列命令(例如：編輯、刪除…)
    /// </summary>
    public class RowCommand
    {
        /// <summary>
        /// 命令名稱
        /// </summary>
        public string CommandName { get; set; } = "";
        /// <summary>
        /// 命令按鈕文字
        /// </summary>
        public string CommandText { get; set; } = "";
        /// <summary>
        /// Css class
        /// </summary>
        public string CssClass { get; set; } = "";
        public RowCommand() { }
        public RowCommand(string commandName,string commandText) 
        { 
            CommandName= commandName;
            CommandText= commandText;
        }
        public RowCommand(string commandName, string commandText, string cssClass)
        {
            CommandName = commandName;
            CommandText = commandText;
            CssClass = cssClass;
        }
    }
}

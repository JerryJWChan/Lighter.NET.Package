using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成Html button tag
    /// </summary>
    public class Button : UiElementBase
    {
        public override UiElementType ElementType => UiElementType.Button;
        /// <summary>
        /// 按鈕種類(button / submit / link button / reset)
        /// </summary>
        public ButtonType ButtonType { get; set; } = ButtonType.Button;
        /// <summary>
        /// 命令名稱
        /// </summary>
        public string CommandName { get; set; } = "";
        /// <summary>
        /// 點擊後要開啟的Controller Action url或連結網址
        /// </summary>
        public string ActionUrl { get; set; } = "";
        /// <summary>
        /// 按鈕是否可視(預設:true)
        /// </summary>
        public bool Visible { get; set; } = true;
        /// <summary>
        /// 按鈕文字
        /// </summary>
        public string Text { get; set; } = "";
        /// <summary>
        /// onclick屬性所要執行的javascript
        /// </summary>
        public string InlineScript { get; set; } = "";
        /// <summary>
        /// Client端Click事件處理js函式
        /// </summary>
        public string ClickEventHandler { get; set; } = "";
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
        public override string Render(IUiElement? setting = null)
        {
            string btnType = (this.ButtonType == ButtonType.LinkButton)? "button" : this.ButtonType.ToString().ToLower();
            string typeAttr = $"type=\"{btnType}\"";
            string commandAttr = string.IsNullOrEmpty(CommandName)?"": $"data-command=\"{CommandName}\"";
            string classAttr = $"class=\"{CssClass??""} {VisibleCssClass}\"";
            string inlineScriptAttr = string.IsNullOrEmpty(InlineScript) ? "" : $"onclick=\"{InlineScript}\"";
            //若有指定ClickEventHandler，但未設定Id時，則自動產生Id
            if(string.IsNullOrEmpty(Id))
            {
                Id = $"btn_{Guid.NewGuid().ToString().Replace("-","")}";
            }
            string html = $"<button {typeAttr} {IDAttribute} {NameAttribute} {commandAttr} {inlineScriptAttr} {ValueAttribute} {classAttr} >{GetText()}</button>";
            string script = BuildScript();
            return html + script;
        }

        /// <summary>
        /// 取得按鈕文字
        /// </summary>
        /// <returns></returns>
        protected virtual string GetText()
        {
            return this.Text;
        }

        /// <summary>
        /// 建立按鈕所需的javascript
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildScript()
        {
            string script = "";
            if (!string.IsNullOrEmpty(ClickEventHandler))
            {
                script += "<script>";
                script += $"document.getElementById('{Id}').addEventListener('click',(e)=>{{{ClickEventHandler}(e); }});";
                script += "</script>";
            }
            return script;
        }
    }
}

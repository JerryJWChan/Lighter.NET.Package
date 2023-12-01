using Lighter.NET.UiComponents.Icon;

namespace Lighter.NET.UiComponents
{
    public class BackToTopButton:Button
    {
        /// <summary>
        /// 自動加入Icon(圓圈向上箭頭)
        /// </summary>
        public bool AutoIcon { get; set; } = true;
        /// <summary>
        /// 自動加入Icon色彩
        /// </summary>
        public string AutoIconColor { get; set; } = "";
        /// <summary>
        /// 自動加入Icon尺寸縮放(預設：3倍)
        /// </summary>
        public double AutoIconScale { get; set; } = 3.0;
        public BackToTopButton() {
            InlineScript = "$ScrollTop()";
            CssClass= "hide font-xl";
            if (string.IsNullOrEmpty(AutoIconColor)) AutoIconColor = "#2f85ff";
        }

        public override string Render(IUiElement? setting = null)
        {
            string btnHtml =  base.Render(setting);
            string containerHtml = $"<div class=\"back-to-top\">{btnHtml}</div>";
            return containerHtml;
        }

        protected override string GetText()
        {
            if (string.IsNullOrEmpty(Text)) Text = "Back To Top";
            if (AutoIcon)
            {
                var tagMaker = new TagMaker();
                Text = @tagMaker.Icon(SvgIcons.GoTop, fillColor: "#2f85ff", scale: 3) + Text;
            }
            return Text;    
        }

        protected override string BuildScript()
        {
            string script = "";
            script += "<script>";
            script += $"RegisterBackToTopButton(\"#{Id}\");";
            script += "window.addEventListener('scroll', __backToTopSwitch);";
            script += "</script>";
            return script;
        }
    }
}

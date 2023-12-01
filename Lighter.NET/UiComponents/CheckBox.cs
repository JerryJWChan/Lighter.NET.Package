using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成input type=checkbox的html tag
    /// </summary>
    public class CheckBox : UiElementBase
    {
        public override UiElementType ElementType =>  UiElementType.CheckBox;
        public override bool IsFormFieldElement { get; set; } = true;
        public CheckBox()
        {
            UiElementBase.Default.ApplyDefault(this);
        }

        /// <summary>
        /// 是(勾選)的值，預設為1
        /// </summary>
        public string TrueValue { get; set; } = "true";
        /// <summary>
        /// 否(取消勾選)的值，預設為0
        /// </summary>
        public string FalseValue { get; set; } = "0";

        public override string Render( IUiElement? setting = null)
        {
            ApplySetting(setting);

            var tagHtml = $"<input type=\"checkbox\" {NameAttribute} {IDAttribute} value=\"{HtmlEncode(TrueValue)}\" class=\"{CssClass}\" {RequiredDataAttribute()} {TemplateDataAttribute()} {GetCheckedAttribute()} {DisabledAttribute} />";
            
            return tagHtml;
        }

        public override string RenderInnerHTML(IUiElement? setting = null)
        {
            return "";
        }

        private string GetCheckedAttribute()
        {
            CharBool cbool = new CharBool(Value);
            return (cbool.IsChecked) ? " checked" : "";
        }
    }
}

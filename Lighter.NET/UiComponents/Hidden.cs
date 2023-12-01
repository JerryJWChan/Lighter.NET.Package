using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成input type=hidden的html tag
    /// </summary>
    public class Hidden : UiElementBase
    {
        public override UiElementType ElementType =>  UiElementType.HiddenField;
        public override bool IsFormFieldElement { get; set; } = true;
        public override string Render( IUiElement? setting = null)
        {
            ApplySetting(setting);
            string html = $"<input type=\"hidden\" {IDAttribute} {NameAttribute} value=\"{HtmlEncode(Value)}\" {TemplateDataAttribute()} {MakeAllAttributesExpression()}/>";
            return html ;
        }

        public override string RenderInnerHTML(IUiElement? setting = null)
        {
            return "";
        }
    }
}

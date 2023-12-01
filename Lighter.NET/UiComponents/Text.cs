
using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成input type=text的html tag
    /// </summary>
    public class Text : UiElementBase
    {
        public override UiElementType ElementType => UiElementType.TextBox;
        public override bool IsFormFieldElement { get; set; } = true;
        /// <summary>
        /// 長度下限
        /// </summary>
        public int MinLength { get; set; }
        /// <summary>
        /// 長度上限
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// 文字格式字串(套用於日期或數字顯示格式)
        /// </summary>
        [SettingProperty]
        public string? Format { get; set; }
        /// <summary>
        /// 是否顯示空白欄位(若需避免顯示value-type型別預設值時，可將此屬性設成true)
        /// </summary>
        [SettingProperty]
        public bool Blank { get; set; } = false;

        /// <summary>
        /// 對應html的placeholder屬性文字
        /// </summary>
        [SettingProperty]
        public string PlaceholderText { get; set; } = "";
        /// <summary>
        /// 經過語系轉換的Placeholder文字
        /// </summary>
        [SettingProperty]
        public string LocalizedPlaceholderText { get; set; } = "";
        public Text() {
            UiElementBase.Default.ApplyDefault(this);
        }
        public override string Render(IUiElement? setting = null)
        {
            ApplySetting(setting);
            string isReadOnly = this.IsReadOnly ? $"tabindex=\"-1\" readonly" : "";
            string hintText = GetPropertyDescription();
            string placeHolderAttr = hintText == "" ? "" : $" placeholder=\"{hintText}\"";
            string value = Blank ? "" : HtmlEncode(Value);
            string html = $"<input type=\"text\" {NameAttribute} {IDAttribute} {MinLengthAttr()} {MaxLengthAttr()} value=\"{value}\" class=\"{CssClass}\" {placeHolderAttr} {isReadOnly} {RequiredDataAttribute()} {TemplateDataAttribute()} {MakeAllAttributesExpression()} {DisabledAttribute}>";
            return html ;
        }

        public override string RenderInnerHTML(IUiElement? setting = null)
        {
            ApplySetting(setting);
            return this.Value;
        }

        /// <summary>
        /// 取得欄位提示文字
        /// </summary>
        /// <param name="metaProp"></param>
        /// <returns></returns>
        public virtual string GetPropertyDescription()
        {
            if (!string.IsNullOrEmpty(LocalizedPlaceholderText)) return LocalizedPlaceholderText;
            if (ModelMetaProperty == null || ModelMetaProperty.Display ==null) 
            { 
                return this.PlaceholderText; 
            }
            string desc = ModelMetaProperty.Display.Description ?? "";
            return (desc == "") ? this.PlaceholderText : desc;
        }

        private string MinLengthAttr()
        {
            return (MinLength > 0) ? $"minlength=\"{MinLength}\" " : "";
        }

        private string MaxLengthAttr()
        {
            return (MaxLength > 0) ? $"maxlength=\"{MaxLength}\" " : "";
        }
    }
}

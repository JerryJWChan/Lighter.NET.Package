using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成textarea的html tag
    /// </summary>
    public class TextArea : UiElementBase
    {
        public override UiElementType ElementType => UiElementType.TextArea;
        public override bool IsFormFieldElement { get; set; } = true;
        /// <summary>
        /// 顯示行數
        /// </summary>
        [SettingProperty]
        public int Cols { get; set; } = 0;
        /// <summary>
        /// 顯示列數
        /// </summary>
        [SettingProperty]
        public int Rows { get; set; } = 0;
        /// <summary>
        /// 文字長度上限
        /// </summary>
        [SettingProperty]
        public int MaxLength { get; set; } = 0;
        /// <summary>
        /// 對應html的placeholder屬性文字
        /// </summary>
        [SettingProperty]
        public string PlaceHolderText { get; set; } = "";
        /// <summary>
        /// 經過語系轉換的Placeholder文字
        /// </summary>
        [SettingProperty]
        public string LocalizedPlaceholderText { get; set; } = "";
        /// <summary>
        /// 是否顯示空白欄位(若需避免顯示value-type型別預設值時，可將此屬性設成true)
        /// </summary>
        [SettingProperty]
        public bool Blank { get; set; } = false;
        /// <summary>
        /// 內容超過範圍時的捲軸方向(預設：無)
        /// </summary>
        [SettingProperty]
        public ScrollDirection Scrollbar { get; set; } = ScrollDirection.Undefined;
        public TextArea() 
        {
            UiElementBase.Default.ApplyDefault(this);
        }
        public override string Render(IUiElement? setting = null)
        {
            ApplySetting(setting);
            string cols_rowsAttr = "";
            if (Cols > 0) cols_rowsAttr += $" cols=\"{Cols}\"";
            if (Rows > 0) cols_rowsAttr += $" rows=\"{Rows}\"";
            string maxlengthAttr = (MaxLength > 0) ? $" maxlength=\"{MaxLength}\"" : "";
            string isReadOnly = this.IsReadOnly ? $"tabindex=\"-1\" readonly" : "";
            string hintText = GetPropertyDescription();
            string placeHolderAttr = hintText == "" ? "" : $" placeholder=\"{hintText}\"";
            string cssClass = this.CssClass;
            if (Scrollbar == ScrollDirection.None) cssClass += " scroll-none";
            if (Scrollbar == ScrollDirection.Vertical) cssClass += " scroll-v";
            if (Scrollbar == ScrollDirection.Horizontal) cssClass += " scroll-h";
            if (Scrollbar == ScrollDirection.Both) cssClass += " scroll-both";
            string html = $"<textarea  {NameAttribute} {IDAttribute} {cols_rowsAttr} {maxlengthAttr} class=\"{cssClass}\" {placeHolderAttr} {isReadOnly} {RequiredDataAttribute()} {TemplateDataAttribute()} {DisabledAttribute}>{HtmlEncode(Value)}</textarea>";
            return html;
        }

        public override string RenderInnerHTML(IUiElement? setting = null)
        {
            ApplySetting(setting);
            return this.Value;
        }

        public string GetPropertyDescription()
        {
            if (!string.IsNullOrEmpty(LocalizedPlaceholderText)) return LocalizedPlaceholderText;
            if (ModelMetaProperty == null || ModelMetaProperty.Display == null) { return this.PlaceHolderText; }
            string desc = ModelMetaProperty.Display.Description ?? "";
            return (desc == "") ? this.PlaceHolderText : desc;
        }
    }
}

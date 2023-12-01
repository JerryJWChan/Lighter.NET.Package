using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成input type=number的html tag
    /// </summary>
    public class Number : UiElementBase
    {
        public override UiElementType ElementType => UiElementType.Number;
        public override bool IsFormFieldElement { get; set; } = true;
        /// <summary>
        /// 是否顯示空白欄位(若需避免顯示value-type型別預設值時，可將此屬性設成true)
        /// </summary>
        [SettingProperty]
        public bool Blank { get; set; } = false;

        private int? _min;
        /// <summary>
        /// 下限值
        /// </summary>
        public int? Min 
        {
            get { return _min; }
            set { _min = value; }
        }

        private int? _max;
        /// <summary>
        /// 上限值
        /// </summary>
        public int? Max 
        {
            get { return _max; }
            set { _max = value; }
        }

        private int _step = 1;
        /// <summary>
        /// 數值間距
        /// </summary>
        public int Step 
        {
            get { return _step; }
            set
            {
                if (value < 0) value = 1;
                _step = value;
            }
        }

        public Number()
        {
            UiElementBase.Default.ApplyDefault(this);
        }

        public override string Render(IUiElement? setting = null)
        {
            ApplySetting(setting);
            string isReadOnly = this.IsReadOnly ? $"tabindex=\"-1\" readonly" : "";
            string minAttr = Min.HasValue ? $"min=\"{Min}\"" : "";
            string maxAttr = Max.HasValue ? $"min=\"{Max}\"" : "";
            string stepAttr = $"step=\"{Step}\"";
            string value = Blank ? "" : HtmlEncode(Value);
            string html = $"<input type=\"number\" {NameAttribute} {IDAttribute} {minAttr} {maxAttr} {stepAttr} value=\"{value}\" class=\"{CssClass}\" {isReadOnly} {RequiredDataAttribute()} {TemplateDataAttribute()} {DisabledAttribute}>";
            return html;
        }
    }
}

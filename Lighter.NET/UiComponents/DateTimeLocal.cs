using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成input type=datetime-local的html tag
    /// </summary>
    public class DateTimeLocal:UiElementBase
    {
        public override UiElementType ElementType => UiElementType.DateTime;
        public override bool IsFormFieldElement { get; set; } = true;

        /// <summary>
        /// 設定值
        /// </summary>
        [SettingProperty]
        public DateTime? SetValue { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        [SettingProperty]
        public DateTime? MinValue { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        [SettingProperty]
        public DateTime? MaxValue { get; set; }
        /// <summary>
        /// 日期輸出格式(預設使用符合ISO-8601的時間格式)
        /// </summary>
        [SettingProperty]
        public string DATETIME_FORMAT { get; set; } = "yyyy-MM-ddTHH:mm";
        /// <summary>
        /// 輸入格式限制(Regular Expression pattern)
        /// </summary>
        [SettingProperty]
        public string Pattern { get; set; } = "";

        /// <summary>
        /// 間隔秒數(預設值：60秒)
        /// </summary>
        [SettingProperty]
        public int Step { get; set; } = 60;

        public DateTimeLocal()
        {
            UiElementBase.Default.ApplyDefault(this);
        }
        public override string Render(IUiElement? setting = null)
        {
            ApplySetting(setting);
            if (!string.IsNullOrEmpty(Value)) //若有給值，轉成日期型態的SetValue
            {
                DateTime parseValue;
                bool parseOK = DateTime.TryParse(Value, out parseValue);
                if(parseOK) { SetValue = parseValue; }
            }
            if (SetValue == default(DateTime)) SetValue = null;
            string valueAttr = (SetValue == null) ? "" : $"value=\"{SetValue.Value.ToString(DATETIME_FORMAT)}\"";
            string minAttr = MinValue.HasValue? $"min=\"{MinValue.Value.ToString(DATETIME_FORMAT)}\"":"";
            string maxAttr = MaxValue.HasValue ? $"max=\"{MaxValue.Value.ToString(DATETIME_FORMAT)}\"" : "";
            string html = $"<input type=\"datetime-local\" {NameAttribute} {IDAttribute} {valueAttr} {minAttr} {maxAttr} class=\"{CssClass}\" {RequiredDataAttribute()} {TemplateDataAttribute()} step=\"{Step}\" {DisabledAttribute}>";
            return html;
        }

        public override string RenderInnerHTML(IUiElement? setting = null)
        {
            if (SetValue == default(DateTime)) SetValue = null;
            if (SetValue != null)
            {
                return this.SetValue.Value.ToString(DATETIME_FORMAT);
            }
            else
            {
                return "";
            }
        }
    }
}

using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成input type=radio的html tag
    /// </summary>
    public class Radio : UiElementBase
    {
        public override UiElementType ElementType => UiElementType.RadioButton;
        public override bool IsFormFieldElement { get; set; } = true;
        /// <summary>
        /// 要被核取的值(若此值與Value相同，則自動核取)
        /// </summary>
        public string ValueToCheck { get; set; } = "";
        /// <summary>
        /// 是否核取
        /// </summary>
        public bool IsChecked { get; set; } = false;
        public Radio() {
            UiElementBase.Default.ApplyDefault(this);
            WithId = false; //radio button預設不給id(因為通常是2個以上同名的radio button)
        }
        public override string Render(IUiElement? setting = null)
        {
            ApplySetting(setting);

            var tagHtml = $"<input type=\"radio\" {NameAttribute} {IDAttribute}\" value=\"{HtmlEncode(Value)}\" class=\"{CssClass}\" {TemplateDataAttribute()} {GetCheckedAttribute()} {DisabledAttribute} />";

            return tagHtml;
        }

        private string GetCheckedAttribute()
        {
            string checkedAttr = (IsChecked) ? " checked" : "";
            if ((!string.IsNullOrEmpty(Value) && !string.IsNullOrEmpty(ValueToCheck)) && Value == ValueToCheck)
            {
                checkedAttr = " checked";
            }
            return checkedAttr;
        }
    }
}

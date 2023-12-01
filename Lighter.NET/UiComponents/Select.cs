using System.Text;
using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成select的html tag
    /// </summary>
    public class Select : UiElementBase, IOptionElement
    {
        public override UiElementType ElementType => UiElementType.Select;
        public override bool IsFormFieldElement { get; set; } = true;
        /// <summary>
        /// 選項清單
        /// </summary>
        [SettingProperty]
        public List<OptionItem>? Options { get; set; }
        public Select()
        {
            UiElementBase.Default.ApplyDefault(this);
        }
        public Select(List<OptionItem> options, string cssClass= "")
        {
            Options = options;  
            UiElementBase.Default.ApplyDefault(this);
        }
        public Select(string name,List<OptionItem> options, string cssClass = "")
        { 
            Name = name;
            Options = options;
            UiElementBase.Default.ApplyDefault(this);
        }
        public Select(string name,string id, List<OptionItem> options, string cssClass = "")
        {
            Name = name;
            Id = id;
            Options = options;
            UiElementBase.Default.ApplyDefault(this);
        }
        public override string Render(IUiElement? setting = null)
        {
            ApplySetting(setting);
            string startTag = $"<select {NameAttribute} {IDAttribute} class=\"{CssClass}\" {RequiredDataAttribute()} {TemplateDataAttribute()} {DisabledAttribute}>\n";
            string optionTags = RenderInnerHTML();
            string endTag = "</select>\n";
            return $"{startTag}{optionTags}{endTag}";
        }

        public override string RenderInnerHTML(IUiElement? setting = null)
        {
            ApplySetting(setting);
            StringBuilder sb = new StringBuilder();
            string extraDataAttr = "";
            if(Options != null && Options.Count > 0) {
                foreach (var option in Options)
                {
					extraDataAttr = "";
                    if (!string.IsNullOrEmpty(option.ExtraDataName))
                    {
                        extraDataAttr = $@" data-extra-name=""{option.ExtraDataName}"" data-extra-value=""{option.ExtraDataValue}""";
					}
					sb.AppendLine($"<option value=\"{HtmlEncode(option.Value)}\"{GetSelectedAttribute(option)}{extraDataAttr}>{HtmlEncode(option.Text)}</option>\n");
                }
            }
            else
            {
                sb.AppendLine($"<option value=\"\"></option>\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 選取屬性
        /// </summary>
        /// <param name="optionValue">選項值</param>
        /// <returns></returns>
        private string GetSelectedAttribute(OptionItem option)
        {
            /*
             * 若此物件有設定Value值，則選取選項值=Value值的項目
             * 若無Value值，則選取OptionItem.Selected=true的項目
             */
            string selected = "";
            if (!string.IsNullOrEmpty(this.Value) && this.Value == option.Value)
            {
                selected =" selected";
            }

            if (option.Selected)
            {
                selected = " selected";
            }

            return selected;
        }
    }
}

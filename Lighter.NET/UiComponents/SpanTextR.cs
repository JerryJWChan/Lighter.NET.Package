namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 對應Html的span tag (readonly style)，用以顯示在資料表單(form)範圍外，或是「純顯示」的資料欄位值
    /// </summary>
    public class SpanTextR : UiElementBase
    {
        public override UiElementType ElementType =>  UiElementType.Span;
        public override bool IsFormFieldElement { get; set; } = true;
        public SpanTextR()
        {
            UiElementBase.Default.ApplyDefault(this);
            CssClass += " block readonly";
        }

        public override string Render(IUiElement? setting = null)
        {
            ApplySetting(setting);
            if(CssClass.Contains("readonly") == false)
            {
                CssClass += " readonly";
            }
            string html = $"<span {IDAttribute} class=\"{CssClass}\" {TemplateDataAttribute()}>{Value}</span>";
            return html;
        }
    }
}

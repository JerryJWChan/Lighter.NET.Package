using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// UiElement預設值載體
    /// </summary>
    public class UiElementDefault 
    {
        /// <summary>
        /// Css class for form field element
        /// </summary>
        public string FormFieldCssClass { get; set; } = "";
        /// <summary>
        /// Css class for elements other than formfield element
        /// </summary>
        public string OtherElementCssClass { get; set; } = "";
        /// <summary>
        /// Inline css style
        /// </summary>
        public string Style { get; set; } = "";

        /// <summary>
        /// 套用預設屬性
        /// </summary>
        /// <param name="uiElement"></param>
        public void ApplyDefault(IUiElement uiElement)
        {
            if (uiElement == null) return;
            if (string.IsNullOrEmpty(uiElement.Style)) uiElement.Style = this.Style;
            if(uiElement.IsFormFieldElement)
            {
                if (string.IsNullOrEmpty(uiElement.CssClass)) uiElement.CssClass = this.FormFieldCssClass;
            }
            else
            {
                if (string.IsNullOrEmpty(uiElement.CssClass)) uiElement.CssClass = this.OtherElementCssClass;
            }
        }

    }
}

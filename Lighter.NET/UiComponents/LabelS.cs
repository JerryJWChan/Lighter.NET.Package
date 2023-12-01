using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成label的html tag，並自動對必填欄位加上紅色星號標示
    /// </summary>
    public class LabelS:Label
    {
        protected override string GetRequiredStar()
        {
            bool isRequired = (NoStar == false) && ((ModelMetaProperty != null && ModelMetaProperty.IsRequired) || this.IsRequired);
            //return isRequired ? $"<span class=\"label-star\">*</span>" : "";
            return IsRequired ? "★" : "";
        }
    }
}

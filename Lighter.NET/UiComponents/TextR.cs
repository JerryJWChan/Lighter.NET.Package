using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 唯讀的html input[type=text] tag
    /// </summary>
    public class TextR:Text
    {
        public override string Render( IUiElement? setting = null)
        {
            this.IsReadOnly = true;
            return base.Render(setting);
        }

        public override string GetPropertyDescription()
        {
            return this.PlaceholderText; //唯讀欄位除非有設定PlaceHolderText，否則不需要其他model所定義的提示文字
        }
    }
}

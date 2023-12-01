using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 選項元素介面
    /// </summary>
    public interface IOptionElement
    {
        /// <summary>
        /// 選項清單
        /// </summary>
        List<OptionItem>? Options { get; set; }
    }
}

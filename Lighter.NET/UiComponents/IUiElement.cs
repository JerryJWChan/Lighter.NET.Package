using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// UI元素介面(定義要在UI(例如MVC的View)介面中render出來的UI元素的基本規格)，包含屬性和方法
    /// </summary>
    public interface IUiElement: IUiElementEntity
    {
        #region Methods[方法]
        /// <summary>
        /// 設定唯讀
        /// </summary>
        /// <returns></returns>
        IUiElement ReadOnly();
        /// <summary>
        /// 設定必填
        /// </summary>
        /// <returns></returns>
        IUiElement Required();
        /// <summary>
        /// 設定可編輯Style
        /// </summary>
        /// <param name="cssClass">可編輯欄位要套用的css class</param>
        /// <returns></returns>
        IUiElement Editable(string cssClass="editable");
        /// <summary>
        /// 加入Html tag屬性
        /// </summary>
        /// <param name="name">屬性名稱</param>
        /// <param name="value">屬性值</param>
        /// <returns></returns>
        IUiElement AddAttribute(string name, string? value = null);
        /// <summary>
        /// 加入data-*屬性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IUiElement AddDataAttribute(string name, string? value = null);
        /// <summary>
        /// 清除CssClass
        /// </summary>
        IUiElement ClearClass();
        /// <summary>
        /// 加入CssClass(多項時以空白分隔)
        /// </summary>
        /// <param name="cssClassList"></param>
        IUiElement AddClass(string cssClassList);
        /// <summary>
        /// 生成UI元素代表的Html語法和內容字串
        /// </summary>
        /// <returns></returns>
        string Render();
        /// <summary>
        /// 生成UI元素代表的Html語法和內容字串
        /// </summary>
        /// <param name="Value">值</param>
        /// <returns></returns>
        string Render(string value);
        /// <summary>
        /// 生成UI元素代表的Html語法和內容字串
        /// </summary>
        /// <param name="setting">參數設定物件</param>
        /// <returns></returns>
        string Render(IUiElement? setting);
        /// <summary>
        /// 只生成UI元素的innerHTML的部分
        /// </summary>
        /// <param name="setting">參數設定物件</param>
        /// <returns></returns>
        string RenderInnerHTML(IUiElement? setting);

        #endregion
    }
}

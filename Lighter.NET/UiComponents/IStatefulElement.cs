namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 具備狀態的元素介面
    /// </summary>
    public interface IStatefulElement
    {
        /// <summary>
        /// 以狀態值為key的呈現方式對照集
        /// </summary>
        Dictionary<object,object?> Presentations { get; }
        /// <summary>
        /// 是否布林狀態(是/否)
        /// </summary>
        bool IsBoolState { get; }
        /// <summary>
        /// 是否「是」狀態
        /// </summary>
        bool IsTrue { get; }
        /// <summary>
        /// 狀態值
        /// </summary>
        object? State { get; set; }
        /// <summary>
        /// 設定(變更)狀態
        /// </summary>
        void SetState(object? state);
        /// <summary>
        /// 加入狀態呈現對照
        /// </summary>
        /// <param name="state">狀態值</param>
        /// <param name="presentation">呈現方式(任何可代表該狀態的值，例如：字串、數字、圖形、物件…)</param>
        /// <returns></returns>
        IStatefulElement AddPresentation(object? state,object? presentation);
        /// <summary>
        /// 取得某狀態值所對應的呈現方式
        /// </summary>
        /// <param name="state">狀態值</param>
        /// <returns></returns>
        object? GetPresentation(object? state);
    }
}

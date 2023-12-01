using Lighter.NET.Common;

namespace Lighter.NET.UiComponents.Icon
{
    /// <summary>
    /// 狀態svg圖示基底類別
    /// </summary>
    public abstract class StatefulSvgIconBase : SvgIcon, IStatefulElement
    {
        public Dictionary<object, object?> Presentations { get; } = new Dictionary<object, object?>();
        public abstract bool IsBoolState { get; }
        public abstract bool IsTrue { get; }
        public object? State { get; set; }
        public abstract void SetState(object? state);
        public virtual IStatefulElement AddPresentation(object? state, object? svgSymbolId) 
        {
            object key = state ?? DefinedConstant.NULL_OBJECT;
            //狀態重複時，覆蓋，否則新增
            if (Presentations.ContainsKey(key))
            {
                Presentations[key] = svgSymbolId;
            }
            else
            {
                Presentations.Add(key, svgSymbolId);
            }
            return this;
        }
        public virtual object? GetPresentation(object? state)
        {
            object key = state ?? DefinedConstant.NULL_OBJECT;
            bool found = Presentations.TryGetValue(key, out var svgSymbolId);
            if (found)
            {
                return svgSymbolId;
            }
            else
            {
                return null;
            }
        }
    }
}

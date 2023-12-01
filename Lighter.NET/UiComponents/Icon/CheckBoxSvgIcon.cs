namespace Lighter.NET.UiComponents.Icon
{
    using Lighter.NET.Common;
    public class CheckBoxSvgIcon : StatefulSvgIconBase
    {
        private CharBool _innerState = new CharBool();
        public override bool IsBoolState { get; } = true;
        public override bool IsTrue => _innerState.IsTrue;
        public CheckBoxSvgIcon():this(SvgIcons.SymbolIds.Square_Checked_24x24, SvgIcons.SymbolIds.Square_24x24) { }
        public CheckBoxSvgIcon(string trueSymbolId, string falseSymbolId) 
        {
            //1(true)值圖示
            AddPresentation("1", trueSymbolId);
            //0(false)值圖示
            AddPresentation("0", falseSymbolId);
        }
        public CheckBoxSvgIcon(object? state) : this(SvgIcons.SymbolIds.Square_Checked_24x24, SvgIcons.SymbolIds.Square_24x24)
        {
            SetState(state);
        }
        public override void SetState(object? state)
        {
            State = state;
            //依照指定的狀態值，選定要呈現的svg symbol id
            this.SymbolId = GetPresentation(state)?.ToString()??"";
        }
        public override IStatefulElement AddPresentation(object? state, object? svgSymbolId)
        {
            /*先將true/false的狀態值轉成CharBool型別(目的是正規化各種「是/否」的狀態值)後再去對應呈現方式*/
            var charBool = new CharBool(state?.ToString());
            _innerState = charBool;
            base.AddPresentation(charBool.Value, svgSymbolId);
            return this;
        }

        public override object? GetPresentation(object? state)
        {
            /*先將true/false的狀態值轉成CharBool型別(目的是正規化各種「是/否」的狀態值)後再去對應呈現方式*/
            var charBool = new CharBool(state?.ToString());
            return base.GetPresentation(charBool.Value);
        }
    }
}

namespace Lighter.NET.Common
{
    /// <summary>
    /// 自定義的特殊常數值
    /// </summary>
    public static class DefinedConstant
    {
        /// <summary>
        /// 自定義的NULL物件(用於特殊情況下代替null值使用，例如作為key值時)
        /// </summary>
        public static object NULL_OBJECT = new NullObject();
    }
}

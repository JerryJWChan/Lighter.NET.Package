namespace Lighter.NET.Common
{
    /// <summary>
    /// Html編碼處理器
    /// 當不同程式專案需要用到不同的HtmlEncoder時，則可在專案初始設定時給定(注入)
    /// </summary>
    public static class HtmlEncodeProvider
    {
        /// <summary>
        /// 此函式是為了解藕合，
        /// 原因是Microsoft的HtmlEncode函式隨著.net版本一直持續在改變，
        /// 相容性也一直在改變，故需要解藕合，使之後程式好維護。
        /// 當不同程式專案需要用到不同的HtmlEncoder時，則可在專案初始設定時給定(注入)此HtmlEncode的function。
        /// </summary>
        public static Func<string, string> Encode { get; set; } = System.Web.HttpUtility.HtmlEncode;
        /// <summary>
        /// 此函式是為了解藕合，
        /// 原因是Microsoft的HtmlDecode函式隨著.net版本一直持續在改變，
        /// 相容性也一直在改變，故需要解藕合，使之後程式好維護。
        /// 當不同程式專案需要用到不同的HtmlDecoder時，則可在專案初始設定時給定(注入)此HtmlDecode的function。
        /// </summary>
        public static Func<string, string> Decode { get; set; } = System.Web.HttpUtility.HtmlDecode;
    }
}

namespace Lighter.NET.Common
{
    /// <summary>
    /// DataModel的檢核錯誤
    /// </summary>
    public class ModelError
    {
        /// <summary>
        /// 屬性(欄位)名稱
        /// </summary>
        public string PropertyName { get; set; } = "";
        /// <summary>
        /// 檢核錯誤訊息
        /// </summary>
        public string Message { get; set; } = "";
    }
}

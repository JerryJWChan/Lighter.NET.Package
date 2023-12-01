namespace Lighter.NET.Common
{
    /// <summary>
    /// Http請求錯誤訊息顯示ViewModel
    /// </summary>
    public class RequestExceptionViewModel
    {
        /// <summary>
        /// Exception的時間
        /// </summary>
        public DateTime ErrorTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 請求編號
        /// </summary>
        public string? RequestId { get; set; }
        /// <summary>
        /// 是否顯示請求編號
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        /// <summary>
        /// 狀態碼
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 對應狀態碼之簡短描述
        /// </summary>
        public string StatusDescription { get; set; } = "";
        /// <summary>
        /// 錯誤訊息(NOTE: 正式機環境時只可顯示簡短訊息，不可暴露系統敏感資訊)
        /// </summary>
        public string ErrorMessage { get; set; } = "";
        /// <summary>
        /// 原始連結網址
        /// </summary>
        public string OriginalUrl { get; set; } = "";
        /// <summary>
        /// 連結網址參數
        /// </summary>
        public string QueryString { get; set; } = "";
        /// <summary>
        /// Exception的Type
        /// </summary>
        public string ExceptionErrorType { get; set; } = "";
        /// <summary>
        /// Exception的Message
        /// </summary>
        public string ExceptoinErrorMessage { get; set; } = "";
        /// <summary>
        /// Exception的StackTrace
        /// </summary>
        public string ExceptionStackTrace { get; set; } = "";

    }
}
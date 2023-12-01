
namespace Lighter.NET.Common
{
    /// <summary>
    /// 描述Action執行發生錯誤的ViewModel
    /// </summary>
    public class ActionErrorViewModel
    {
        /// <summary>
        /// 頁面標題
        /// </summary>
        public string PageTitle { get; set; } = "執行錯誤";
        /// <summary>
        /// 錯誤名稱
        /// </summary>
		public string? ErrorName { get; set; }
		/// <summary>
		/// 錯誤時間
		/// </summary>
		public DateTime ErrorTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 錯誤來源
        /// </summary>
        public string? ErrorSource { get; set; }
        /// <summary>
        /// 錯誤代碼
        /// </summary>
        public string? ErrorCode { get; set; }
        /// <summary>
        /// 錯誤訊息model
        /// </summary>
        public MessageModel? MessageModel { get; set; }
		/// <summary>
		/// 是否可以「再試一次」，條件如下：
		/// (1)有RetryActionUrl 
		/// (2)有RetryActionUrl參數正確 
		/// (3)如果有RetryData的話，RetryData必須是已經通過檢核的
		/// </summary>
		public bool CanRetry { get; set; } = false;
        /// <summary>
        /// 重試請求採用的Http Request方式(GET/POST)
        /// </summary>
        public HttpRequestMethod RetryMethod { get; set; } = HttpRequestMethod.Undefined;
		/// <summary>
		/// 再試一次的action url
		/// </summary>
		public string RetryActionUrl { get; set; } = "";
        /// <summary>
        /// 再試一次要送回的data
        /// </summary>
        public object? RetryData { get; set; } = null;

        /// <summary>
        /// 再試一次要送回的data的Json
        /// </summary>
        public string RetryDataJson { get; set; } = "";

        /// <summary>
        /// 是否有錯誤
        /// </summary>
        public bool HasError
        {
            get
            {
                return string.IsNullOrEmpty(ErrorCode) == false || string.IsNullOrEmpty(ErrorMessage) == false;
            }
        }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string? ErrorMessage
        {
            get
            {
                if (MessageModel == null) { return ""; }
                return MessageModel.Text;
            }
        }

        /// <summary>
        /// 其他提示訊息(例如：請重新操作一次，或請稍後再試。)
        /// </summary>
        public string HintMessage { get; set; } = "";

        /// <summary>
        /// 是否顯示「回上一頁」按鈕
        /// </summary>
        public bool ShowBackButton { get; set; } = true;
		/// <summary>
		/// 是否顯示「關閉」按鈕
		/// </summary>
		public bool ShowCloseButton { get; set; } = false;
		/// <summary>
		/// 是否顯示「前往」按鈕
		/// </summary>
		public bool ShowGoToButton { get; set; } = false;
		/// <summary>
		/// 「前往」按鈕文字
		/// </summary>
		public string GoToButtonText { get; set; } = "";
		/// <summary>
		/// 「前往」按鈕URL
		/// </summary>
		public string GoToButtonUrl { get; set; } = "";
        /// <summary>
        /// Exception的Type
        /// </summary>
        public string ExceptionErrorType { get; set; } = "";
        /// <summary>
        /// Exception的Message
        /// </summary>
        public string ExceptionErrorMessage { get; set; } = "";
        /// <summary>
        /// Exception的StackTrace
        /// </summary>
        public string ExceptionStackTrace { get; set; } = "";
        /// <summary>
        /// 設定錯誤訊息
        /// </summary>
        /// <param name="errMsg">錯誤訊息</param>
        /// <param name="isPopup">是否跳顯</param>
        /// <param name="caption">訊息標頭</param>
        /// <param name="msgType">訊息種類</param>
        public void SetErrMsg(string errMsg,bool isPopup=false, string caption= "系統訊息", MessageType msgType = MessageType.Error)
        {
			if (string.IsNullOrEmpty(errMsg)) return;
			if (MessageModel != null)
            {
				MessageModel.Text += errMsg;
                if (!string.IsNullOrEmpty(MessageModel.Text))
                {
                    MessageModel.Text += " ";
                }
            }
            else
            {
				MessageModel = new MessageModel() { Caption = caption, Text = errMsg };
			}

            //若訊息的嚴重程度更大，則取代之。(數字越大，嚴重程度越大)
            if((int)msgType > (int)MessageModel.Type)
            {
                MessageModel.Type = msgType;
			}
		}

        /// <summary>
        /// 給入Exception物件
        /// </summary>
        /// <param name="ex"></param>
        public void SetException(Exception ex)
        {
            if (ex == null) return;
            ExceptionErrorType = ex.GetType().Name;
            ExceptionErrorMessage = ex.Message;
            ExceptionStackTrace = ex.StackTrace??"";
        }
	}
}

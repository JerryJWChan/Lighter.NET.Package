namespace Lighter.NET.Common
{
    /// <summary>
    /// 訊息Model
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// 訊息種類
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Info;
        /// <summary>
        /// 訊息種類
        /// </summary>
        public MessageType MsgType { get { return Type; } }
        /// <summary>
        /// 訊息種類
        /// </summary>
        public MessageType MessageType { get { return Type; } }

        /// <summary>
        /// 作業名稱，或訊息來源(出處)
        /// </summary>
        public string JobName { get; set; } = "";
        /// <summary>
        /// 訊息標題
        /// </summary>
        public string Caption { get; set; } = "";
        /// <summary>
        /// 訊息文字
        /// </summary>
        public string Text { get; set; } = "";
        /// <summary>
        /// 是否跳窗訊息
        /// </summary>
        public bool IsPopup { get; set; } = false;
    }
}

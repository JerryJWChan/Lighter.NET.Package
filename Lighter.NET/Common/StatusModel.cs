namespace Lighter.NET.Common
{
    /// <summary>
    /// 狀態Model
    /// </summary>
    public class StatusModel:MessageModel
    {
        /// <summary>
        /// 狀態名稱
        /// </summary>
        public string Status { get; set; } = "";
        public StatusModel() { }
        public StatusModel(string status)
        {
            Status = status;
        }
    }
}

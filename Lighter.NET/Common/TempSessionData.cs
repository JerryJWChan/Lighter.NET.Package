namespace Lighter.NET.Common
{
    /// <summary>
    /// 暫存Session資料
    /// </summary>
    /// <typeparam name="T">暫存資料型別</typeparam>
    public class TempSessionData<T>
    {
        /// <summary>
        /// Session識別碼
        /// </summary>
        public string SessionID { get; set; } = "";
        /// <summary>
        /// Session所屬UserID
        /// </summary>
        public string OwnerUserID { get; set; } = "";
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 過期時間
        /// </summary>
        public DateTime ExpiredTime { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 暫存/狀態資料
        /// </summary>
        public List<T>? Data { get; set; }
    }
}

namespace Lighter.NET.DB
{
    /// <summary>
    /// Db Service組態
    /// </summary>
    public class DbServiceConfig
    {
        /// <summary>
        /// 連線字串
        /// </summary>
        public string ConnectionString { get; set; } = "";
        /// <summary>
        /// 應用程式代號
        /// </summary>
        public string AppCode { get; set; } = "";
        /// <summary>
        /// 資料庫名稱
        /// </summary>
        public string DbName { get; set; } = "";
        /// <summary>
        /// 資料表名稱前綴
        /// </summary>
        public string DbTablePrefix { get; set; } = "";

        /// <summary>
        /// 目前Db存取者資訊的取得函式注入口(取得包括：登入者ID、姓名、Ip)
        /// 此函式需在主程式configure service階段注入，若未注入，則不自動記錄creator或updater資訊
        /// </summary>
        public Func<IDbAccessUser>? DbAccessUserInfoGetter { get; set; }
    }
}

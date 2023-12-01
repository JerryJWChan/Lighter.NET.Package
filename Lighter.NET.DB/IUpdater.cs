namespace Lighter.NET.DB
{
    /// <summary>
    /// 更新者欄位介面
    /// </summary>
    public interface IUpdater
    {
        /// <summary>
        /// 更新者
        /// </summary>
        string? updateBy { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        DateTime? updateAt { get; set; }
        /// <summary>
        /// 更新來自Ip
        /// </summary>
        string? updateIp { get; set; }
    }
}

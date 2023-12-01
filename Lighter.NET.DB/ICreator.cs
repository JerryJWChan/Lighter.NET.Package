namespace Lighter.NET.DB
{
    /// <summary>
    /// 建立者欄位介面
    /// </summary>
    public interface ICreator
    {
        /// <summary>
        /// 建立者
        /// </summary>
        string createBy { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        DateTime createAt { get; set; }
        /// <summary>
        /// 建立來自Ip
        /// </summary>
        string? createIp { get; set; }
    }
}

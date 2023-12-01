namespace Lighter.NET.DB
{
    /// <summary>
    /// Db存取者資訊
    /// </summary>
    public interface IDbAccessUser
    {
        /// <summary>
        /// 使用者帳號
        /// </summary>
        string? UserId { get; set; }
        /// <summary>
        /// 使用者名稱
        /// </summary>
        string? UserName { get; set; }
        /// <summary>
        /// Ip Address
        /// </summary>
        string? IpAddress { get; set; }  
    }
}

namespace Lighter.NET.DB
{
    /// <summary>
    /// Db存取者資訊
    /// </summary>
    public class DbAccessUser : IDbAccessUser
    {
        /// <summary>
        /// the user id of the user doing the db access operation
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// the user name of the user doing the db access operation
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// the source ip address of the db accessing request 
        /// </summary>
        public string? IpAddress { get; set; }
    }
}

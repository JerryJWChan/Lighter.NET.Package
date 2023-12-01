using System.Data.SqlClient;
namespace Lighter.NET.DB
{
    /// <summary>
    /// DbContext工廠(用以建立DbConext)
    /// </summary>
    public class DbContextFactory
    {
        /// <summary>
        /// 建立DbConext的instance
        /// </summary>
        /// <param name="connectionString">連線字串</param>
        /// <param name="reusable">DbContext(連線)是否可以重複使用於多次的Db操作</param>
        /// <returns></returns>
        public DbContextBase CreateDbContext(string connectionString,bool reusable)
        {
            var conn = new SqlConnection(connectionString);
            var ctx = new DbContextBase(conn, reusable);
            return ctx;
        }
    }
}

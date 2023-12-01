using Lighter.NET.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lighter.NET.DB
{
    /// <summary>
    /// A general db access service of Lighter.NET.DB
    /// </summary>
    public class LighterDb : DbServiceBase
    {
        /// <summary>
        /// constructor
        /// </summary>
        public LighterDb():base() { }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dbContextReusable"></param>
        public LighterDb(bool dbContextReusable) : base(dbContextReusable){}

        /// <summary>
        /// Get connection string from DbServiceBase.DbServiceConfig.ConnectionString
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override string GetConnectionString()
        {
            if (string.IsNullOrEmpty(DbServiceBase.DbServiceConfig.ConnectionString))
            {
                throw new InvalidOperationException("The ConnectionString property is not set for DbServiceBase.DbServiceConfig");
            }
            return DbServiceBase.DbServiceConfig.ConnectionString;
        }

        /// <summary>
        /// 使用DbServiceBase.DbServiceConfig.ConnectionString,
        /// 建立一個新的dbcontext(注意：此context應使用在using(){}程式區塊，以確保使用後的資源釋放)
        /// </summary>
        /// <param name="contextReusable">db context是否可重複使用(執行多次命令)(預設值:true)</param>
        /// <returns></returns>
        public static DbContextBase NewContext(bool contextReusable = true)
        {
            if (string.IsNullOrEmpty(DbServiceBase.DbServiceConfig.ConnectionString))
            {
                throw new InvalidOperationException("The ConnectionString property is not set for DbServiceBase.DbServiceConfig");
            }

            return new DbContextFactory().CreateDbContext(DbServiceBase.DbServiceConfig.ConnectionString, contextReusable);
        }

        /// <summary>
        /// 使用傳入的onnectionString,
        /// 建立一個新的dbcontext(注意：此context應使用在using(){}程式區塊，以確保使用後的資源釋放)
        /// </summary>
        /// <param name="connectionString">連線字串</param>
        /// <param name="contextReusable">db context是否可重複使用(執行多次命令)(預設值:true)</param>
        /// <returns></returns>
        public static DbContextBase NewContext(string connectionString , bool contextReusable = true)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connectionString argument is empty");
            }

            return new DbContextFactory().CreateDbContext(connectionString, contextReusable);
        }

        /// <summary>
        /// 使用DbServiceBase.DbServiceConfig.ConnectionString,
        /// 建立一個新的給transaction用的dbcontext(注意：此context應使用在using(){}程式區塊，以確保使用後的資源釋放)
        /// </summary>
        /// <returns></returns>
        public static DbContextBase TransactionContext()
        {
            if (string.IsNullOrEmpty(DbServiceBase.DbServiceConfig.ConnectionString))
            {
                throw new InvalidOperationException("The ConnectionString property is not set for DbServiceBase.DbServiceConfig");
            }
            bool contextReusable = true;
            return new DbContextFactory().CreateDbContext(DbServiceBase.DbServiceConfig.ConnectionString, contextReusable);
        }

        /// <summary>
        /// 使用傳入的onnectionString,
        /// 建立一個新的給transaction用的dbcontext(注意：此context應使用在using(){}程式區塊，以確保使用後的資源釋放)
        /// </summary>
        /// <param name="connectionString">連線字串</param>
        /// <returns></returns>
        public static DbContextBase TransactionContext(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connectionString argument is empty");
            }
            bool contextReusable = true;
            return new DbContextFactory().CreateDbContext(connectionString, contextReusable);
        }

        /// <summary>
        /// 執行交易作業(無回傳查詢資料)
        /// </summary>
        /// <param name="dbOpration">db作業(insert/update/delete)</param>
        /// <param name="isolationLevel">交易隔離等級</param>
        /// <returns>回傳成功或失敗的結果</returns>
        public static ApiResult Transaction(Action<DbContextBase> dbOpration, System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted)
        {
            try
            {
                using (var ctx = TransactionContext())
                using (var transcaction = ctx.Database.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        dbOpration(ctx);

                        transcaction.Commit();
                        return new ApiSuccessResult();
                    }
                    catch (Exception ex)
                    {
                        transcaction.Rollback();
                        var result = new ApiFailResult();
                        result.AddException(ex);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                var result = new ApiFailResult();
                result.AddException(ex);
                return result;
            }
        }
    }

}

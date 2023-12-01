using Lighter.NET.Common;
using System.Linq.Expressions;
using System.Data.SqlClient;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Lighter.NET.DB
{
    /// <summary>
    /// DB Log 記錄基礎函式
    /// </summary>
    public abstract class DbLogServiceBase
    {
        #region static
        /// <summary>
        /// 建立者欄位
        /// </summary>
        protected string[] _creatorFields = new string[] { nameof(ICreator.createAt), nameof(ICreator.createBy), nameof(ICreator.createIp) };
        /// <summary>
        /// 更新者欄位
        /// </summary>
        protected string[] _updaterFields = new string[] { nameof(IUpdater.updateAt), nameof(IUpdater.updateBy), nameof(IUpdater.updateIp) };
        #endregion

        #region Private/Protected field
        /// <summary>
        /// EntityFramework db context
        /// </summary>
        protected DbContextBase? _dbContext;
        /// <summary>
        /// dbcontext是否disposed flag
        /// </summary>
        protected bool _dbContextDisposed = false;

		#endregion

		#region Properties
		#endregion

		#region Constructor
		/// <summary>
		/// DB Log 記錄基礎函式
		/// </summary>
		public DbLogServiceBase() { }

        #endregion

        #region Abstract Methods
        /// <summary>
        /// 取得連線字串
        /// </summary>
        /// <returns></returns>
        public abstract string GetConnectionString();
        #endregion

        #region DbContext
        /// <summary>
        /// 建立一個此服務所使用的DbContext
        /// </summary>
        /// <returns></returns>
        protected DbContextBase NewDbContext()
        {
            string connString = GetConnectionString();
            if (string.IsNullOrEmpty(connString))
            {
                throw new InvalidOperationException($"Invalid connectionstring. The return vale of the GetConnectionString() is not valid.");
            }
            return new DbContextFactory().CreateDbContext(connString, false) ;
        }

        /// <summary>
        /// 取得DbContext
        /// </summary>
        /// <returns></returns>
        public DbContextBase GetDbContext()
        {
            if (_dbContext == null) _dbContext = NewDbContext();
            return _dbContext;
        }
        #endregion

        #region Methods
        #endregion

        #region Dispose
        /// <summary>
        /// 釋放服務資源
        /// </summary>
        public void Dispose()
        {
            DisposeDbContext();
        }
        /// <summary>
        /// 釋放DbContext資源(並關閉db connection)
        /// </summary>
        protected void DisposeDbContext()
        {
            if (_dbContext != null)
            {
                try
                {
                    if (!_dbContextDisposed && _dbContext.Database.Connection != null && _dbContext.Database.Connection.State != System.Data.ConnectionState.Closed)
                    {
                        _dbContext.Database.Connection.Close();
                    }
                    _dbContextDisposed = true;
                    _dbContext.Dispose();
                }
                catch (Exception)
                {
                    //關閉連線時發生某些錯誤，繼續dispose dbcontext
                    _dbContextDisposed = true;
                    _dbContext.Dispose();
                }

            }
        }
        #endregion

    }

}

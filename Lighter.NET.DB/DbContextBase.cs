namespace Lighter.NET.DB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Data.SqlClient;
    using System.Data.Entity;

    /// <summary>
    /// 自定義EF6組態
    /// </summary>
    public class DbConfig : DbConfiguration
    {
		/// <summary>
		/// 自定義EF6組態
		/// </summary>
		public DbConfig()
        {
            SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
        }
    }


    /// <summary>
    /// DbContext基礎類別(注意：EF6之前的版本對connection open/close的處理方式有不同的行為)
    /// </summary>
    [DbConfigurationType(typeof(DbConfig))]
    public class DbContextBase : DbContext , IDisposable
    {
        /// <summary>
        /// dbcontext是否可重覆使用
        /// </summary>
        protected bool _isReusable = false;
        /// <summary>
        /// dbcontext是否可重覆使用
        /// </summary>
        public bool Reusable 
        { 
            get 
            { 
                return _isReusable; 
            } 
        }

        /// <summary>
        /// 此建構子不開放給外部使用(因沒有給入連線字串，除非寫死字串給base()或是用static string給base()，故避免使用)
        /// </summary>
        protected DbContextBase() : base() { }
        /// <summary>
        /// 依傳入的ConnectionString 建立DbContext
        /// </summary>
        /// <param name="connStringOrName">連線字串或名稱</param>
        public DbContextBase(string connStringOrName) : base(connStringOrName)
        {
            //disable initializer 以避免 code first migration去變更或檢查DB schema
            Database.SetInitializer<DbContextBase>(null);
        }

        /// <summary>
        /// 依傳入的ConnectionString 建立DbContext
        /// </summary>
        /// <param name="conn">sql connection</param>
        /// <param name="contextOwnsConnection">true: 表示dbcontext主導connection的關閉動作；false表示dbcontext不主導關閉connnection,
        /// 若需重複執行多項dbcontext操作，則將reusable設成true，但必須在執行最後確保connection closed/disposed，建立使用using(){}語法，
        /// 若contextOwnsConnection為false時，則在dispose dbcontext之前，必須先手動close和dispose dbconnection
        /// </param>
        public DbContextBase(SqlConnection conn, bool contextOwnsConnection) : base(conn, contextOwnsConnection)
        {
            _isReusable = contextOwnsConnection;
            //disable initializer 以避免 code first migration去變更或檢查DB schema
            Database.SetInitializer<DbContextBase>(null);
        }

        /// <summary>
        /// 在目前的DB連線中開始一段transaction(注意：此context應使用在using(){}程式區塊，以確保使用後的資源釋放)
        /// </summary>
        /// <returns></returns>
        public DbContextTransaction BeginTransaction()
        {
            //when using transaction, the DbContextBase must be set as reusable.
            if (!Reusable)
            {
                throw new InvalidOperationException("When using transaction, the DbContextBase must be instanciated with reusable argument of true value.");
            }
            return Database.BeginTransaction();
        }

        /// <summary>
        /// 在目前的DB連線中開始一段transaction(注意：此context應使用在using(){}程式區塊，以確保使用後的資源釋放)
        /// </summary>
        /// <param name="isolationLevel">transaction隔離等級</param>
        /// <returns></returns>
        public DbContextTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            //when using transaction, the DbContextBase must be set as reusable.
            if (!Reusable)
            {
                throw new InvalidOperationException("When using transaction, the DbContextBase must be instanciated with reusable argument of true value.");
            }
            return Database.BeginTransaction(isolationLevel);
        }


        /// <summary>
        /// 設定DbContext執行模式(提高效能參數)
        /// </summary>
        /// <param name="contextModes">dbContext模式：NoDetectChange(停用變更追蹤),NoLazyLoading(停用延遲載入)</param>
        public void SetContextMode(DbContextMode contextModes)
        {
            if (contextModes.HasFlag(DbContextMode.NoDetectChange))
            {
                Configuration.AutoDetectChangesEnabled = false;
            }
            if (contextModes.HasFlag(DbContextMode.NoLazyLoading))
            {
                Configuration.LazyLoadingEnabled = false;
                Configuration.ProxyCreationEnabled = false;
            }
        }

        /// <summary>
        /// 產出輕量化的DbContext(無LazyLoading,無Proxy,無DetectChange)
        /// ,適合需即時反應資料最新狀態的純資料查詢(無編輯寫入)動作(例如報表查詢)
        /// </summary>
        public void LightWeight()
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
        }

        /// <summary>
        /// call the protected Dispose() and ensure the dbconnection is disposed and closed.
        /// </summary>
        public new void Dispose()
        {
            try
            {
                if (Database.Connection != null && Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    Database.Connection.Close();
                }

                if (Database.Connection != null)
                {
                    Database.Connection.Dispose();
                }
            }
            catch (Exception)
            {
                //do nothing. ignore the ObjectDisposedException
            }

            Dispose(disposing: true);
            GC.SuppressFinalize(this);

        }

    }
}

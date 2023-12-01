namespace Lighter.NET.DB
{
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
    using System.Collections;

    /// <summary>
    /// DB操作基礎函式
    /// </summary>
    public abstract class DbServiceBase
    {
        #region Static Properties
        /// <summary>
        /// Db Service組態
        /// </summary>
        public static DbServiceConfig DbServiceConfig = new DbServiceConfig();

        #endregion

        #region Static Method
        /// <summary>
        /// 設定Db Service組態
        /// </summary>
        /// <param name="configSetter">組態設定函式注入</param>
        public static void Configure(Action<DbServiceConfig> configSetter)
        {
            if(configSetter != null)
            {
                configSetter(DbServiceConfig);
            }
        }
        #endregion

        #region Private/Protected field
        /// <summary>
        /// EntityFramework db context
        /// </summary>
        protected DbContextBase? _dbContext;
        /// <summary>
        /// DbContect是否可用於多次Db操作
        /// </summary>
        protected bool _dbContextReusable;
        /// <summary>
        /// dbcontext是否disposed flag
        /// </summary>
        protected bool _dbContextDisposed = false;
        /// <summary>
        /// 建立者欄位
        /// </summary>
        protected string[] _creatorFields = new string[] { nameof(ICreator.createAt), nameof(ICreator.createBy), nameof(ICreator.createIp) };
        /// <summary>
        /// 更新者欄位
        /// </summary>
        protected string[] _updaterFields = new string[] { nameof(IUpdater.updateAt), nameof(IUpdater.updateBy), nameof(IUpdater.updateIp) };
        /// <summary>
        /// db log分類
        /// </summary>
        protected DbLogType _logType = DbLogType.Undefined;
        /// <summary>
        /// 是否針對update和delete操作，記錄log(預設:true)
        /// </summary>
        protected bool _isLog = true;
        /// <summary>
        /// update/delete前的資料
        /// </summary>
        protected virtual object? _dataBefore { get; set; }
        /// <summary>
        /// update後的資料
        /// </summary>
        protected virtual object? _dataAfter { get; set; }
        /// <summary>
        /// log備註
        /// </summary>
        protected string _logMemo { get; set; } = "";

        #endregion

        #region Properties
        /// <summary>
        /// DbContect是否可用於多次Db操作
        /// </summary>
        public bool DbContextResuable 
        {
            get 
            {
                return _dbContextReusable;
            }
        }
        /// <summary>
        /// 資料分頁設定
        /// </summary>
        public PagingSetting? PagingSetting { get; protected set; }
        /// <summary>
        /// 是否分頁處理
        /// </summary>
        public bool IsPaging
        {
            get
            {
                return PagingSetting != null && PagingSetting.IsValid();
            }
        }

        /// <summary>
        /// 需要語系對應的欄位名稱
        /// </summary>
        public List<string> CultureMappingColumnNames { get; set; } = new List<string>();
        #endregion

        #region Constructor
        /// <summary>
        /// constructor of DbServiceBase
        /// </summary>
        public DbServiceBase() : this(false) { }

        /// <summary>
        /// constructor of DbServiceBase
        /// </summary>
        /// <param name="dbContextReusable">db context是否可重複使用(執行多次命令)</param>
        public DbServiceBase(bool dbContextReusable)
        {
            _dbContextReusable = dbContextReusable;
			/*此處先不產生dbcontext，待要執行sql操作時再產生，縮短佔用db connection的時間*/
			//_dbContext = NewDbContext(dbContextReusable);
		}

        /// <summary>
        /// constructor of DbServiceBase
        /// </summary>
        /// <param name="lighterDbContext"></param>
        public DbServiceBase(DbContextBase lighterDbContext)
        {
            _dbContextReusable = lighterDbContext.Reusable;
            _dbContext = lighterDbContext;
        }

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
        /// <param name="dbContextReusable">db context是否可重複使用(執行多次命令)</param>
        /// <returns></returns>
        protected DbContextBase NewDbContext(bool dbContextReusable)
        {
            string connString = GetConnectionString();
            if (string.IsNullOrEmpty(connString))
            {
                throw new InvalidOperationException($"Invalid connectionstring. The return vale of the GetConnectionString() is not valid.");
            }
            return new DbContextFactory().CreateDbContext(connString, dbContextReusable);
        }

        /// <summary>
        /// 取得DbContext
        /// </summary>
        /// <returns></returns>
        public DbContextBase GetDbContext()
        {
            if (_dbContext == null || _dbContextDisposed) _dbContext = NewDbContext(DbContextResuable);
			_dbContextDisposed = false;
			return _dbContext;
        }

		/// <summary>
		/// 取得DbContext
		/// </summary>
        /// <param name="reusable">是否可重複使用</param>
		/// <returns></returns>
		public DbContextBase GetDbContext(bool reusable)
		{
            _dbContextReusable = reusable;
            return GetDbContext();
		}

		#endregion

		#region Methods

        /// <summary>
        /// 可重複使用
        /// </summary>
        /// <returns></returns>
        public virtual DbServiceBase Reusable()
        {
            _dbContextReusable = true;
            return this;
        }

		/// <summary>
		/// 設定語系欄位對應
		/// </summary>
		/// <returns></returns>
		protected virtual List<CultureMappingColumn> CultureMappingColumnConfig()
        {
            return new List<CultureMappingColumn>();
        }

        /// <summary>
        /// 轉換成SqlParameter陣列
        /// </summary>
        /// <param name="anonymousParameterObject">anonymous物件參數</param>
        /// <returns></returns>
        protected SqlParameter[] ConvertToSqlParameters(object? anonymousParameterObject)
        {
            if (anonymousParameterObject == null) return new SqlParameter[0];

            Type objectType = anonymousParameterObject.GetType();
            List<SqlParameter> parameters = new List<SqlParameter>();
            var props = objectType.GetProperties();
            foreach (var prop in props)
            {
                var paramValue = prop.GetValue(anonymousParameterObject);
                if (paramValue == null) paramValue = DBNull.Value; //null值處理
                parameters.Add(new SqlParameter(prop.Name, paramValue));
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// 取得Lambda表示式指定的屬性物件
        /// </summary>
        /// <typeparam name="TType">類別參數項</typeparam>
        /// <typeparam name="TReturn">(單一)屬性表示式</typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected PropertyInfo? GetLambdaProperty<TType, TReturn>(Expression<Func<TType, TReturn>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (!(selector is LambdaExpression)) throw new ArgumentException($"{nameof(selector)} must be a LambdaExpression.");
            LambdaExpression lambda = selector;
            var memberExpression = lambda.Body is UnaryExpression expression
                ? expression.Operand as MemberExpression
                : lambda.Body as MemberExpression;

            return memberExpression?.Member as PropertyInfo;
        }

        /// <summary>
        /// 取得Lambda表示式指定的屬性名稱
        /// </summary>
        /// <typeparam name="TType">類別參數項</typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="selector">(單一)屬性表示式</param>
        /// <returns></returns>
        protected string GetLambdaPropertyName<TType, TReturn>(Expression<Func<TType, TReturn>> selector)
        {
            return GetLambdaProperty(selector)?.Name ?? "";
        }
        #endregion

        #region Select
        /// <summary>
        /// 查詢TModel資料集
        /// </summary>
        /// <typeparam name="TViewModel">指定的回傳型別TViewModel</typeparam>
        /// <param name="sql">SQL語法</param>
        /// <returns></returns>
        public List<TViewModel> Select<TViewModel>(string sql) where TViewModel : new()
        {
            var parameters = new SqlParameter[0];
            return Select<TViewModel>(parameters, sql );
        }


        /// <summary>
        /// 查詢ViewModel資料集
        /// </summary>
        /// <typeparam name="TViewModel">指定的回傳型別ViewModel</typeparam>
        /// <param name="sql">SQL語法，若有含參數項，則要給入一個參數物件</param>
        /// <param name="anonymousParameter">anonymous條件參數物件</param>
        /// <returns></returns>
        public List<TViewModel> Select<TViewModel>(string sql, object? anonymousParameter) where TViewModel : new()
        {
            SqlParameter[] parameters;
            if (anonymousParameter == null)
            {
                if(sql.IndexOf('@') < 0) //SQL語法中不含參數項
                {
                    parameters = new SqlParameter[0];
                    return Select<TViewModel>(parameters, sql);
                }
                else
                {
                    //SQL語法中含參數項，但未給入參數
                    throw new ArgumentNullException($"{nameof(anonymousParameter)}.");
                }
            }

           
            if (anonymousParameter.IsAnonymousType())
            {
                parameters = ConvertToSqlParameters(anonymousParameter);
                return Select<TViewModel>(parameters, sql);
            }
            else
            {
                Type paramType = anonymousParameter.GetType();
                
                if(paramType.Name == "SqlParameter[]")
                {
                    parameters = (anonymousParameter as IEnumerable<SqlParameter>)!.ToArray();
                    //parameters = ((IEnumerable)anonymousParameter).Cast<SqlParameter>().ToArray();
                    return Select<TViewModel>(parameters, sql);
                }
                else
                {
                    throw new ArgumentException($"the argument {nameof(anonymousParameter)} is not a valid anonymous type, nor a SqlParameter[]");
                }
            }

           
            //var ctx = GetDbContext();
            //var parameters = ConvertToSqlParameters(anonymousParameter);
            //List<TViewModel> list;
            //if (IsPaging)
            //{
            //    //分頁查詢
            //    list = PagingSelect<TViewModel>(sql, parameters, ctx);
            //}
            //else
            //{
            //    //一般查詢
            //    list = ctx.Database.SqlQuery<TViewModel>(sql, parameters)?.ToList() ?? new List<TViewModel>();
            //}

            ////dispose db context if needed
            //if (DbContextResuable == false) DisposeDbContext();

            //return list;
        }

        /// <summary>
        /// 查詢ViewModel資料集
        /// </summary>
        /// <typeparam name="TViewModel">指定的回傳型別ViewModel</typeparam>
        /// <param name="sql">SQL語法，若有含參數項，則要給入一個參數物件</param>
        /// <param name="parameters">SqlParameter陣列</param>
        /// <returns></returns>
        public List<TViewModel> Select<TViewModel>( SqlParameter[] parameters,string sql) where TViewModel : new()
        {
            var ctx = GetDbContext();
            List<TViewModel> list;
            if (IsPaging)
            {
                //分頁查詢
                list = PagingSelect<TViewModel>(sql, parameters, ctx);
            }
            else
            {
                //一般查詢
                list = ctx.Database.SqlQuery<TViewModel>(sql, parameters)?.ToList() ?? new List<TViewModel>();
            }

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return list;
        }

        /// <summary>
        /// 查詢單一值
        /// </summary>
        /// <typeparam name="TValue">回傳值型別</typeparam>
        /// <param name="sql">SQL語法，若有含參數項，則要給入一個參數物件</param>
        /// <param name="anonymousParameter">anonymous參數物件</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">若回傳值含超過一筆資料，則拋Error</exception>
        public TValue? SelectScalar<TValue>(string sql, object? anonymousParameter = null)
        {
            var ctx = GetDbContext();
            var parameters = ConvertToSqlParameters(anonymousParameter);
            var list = ctx.Database.SqlQuery<TValue>(sql, parameters)?.ToList();
            
            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            //確認查詢結果只有單一值
            if (list != null && list.Count > 1) throw new InvalidDataException("The result set of the sql query has multiple values while the SelectScalar() expects only single value result.");

            TValue? value = (list != null) ? list.FirstOrDefault() : default(TValue);
            return value;
        }

        /// <summary>
        /// 查詢並傳回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="anonymousParameter">anonymous條件參數物件</param>
        /// <returns></returns>
        public DataSet SelectDataSet(string sql, object anonymousParameter)
        {
            var parameters = ConvertToSqlParameters(anonymousParameter);
            return SelectDataSet(sql, parameters);
        }

        /// <summary>
        /// 查詢並傳回DataSet
        /// </summary>
        /// <param name="sql">查詢語法(可含多筆查詢以分號區隔)</param>
        /// <param name="parameters">SqlParameters</param>
        /// <returns></returns>
        public DataSet SelectDataSet(string sql, params SqlParameter[] parameters)
        {
            var dbContext = GetDbContext();
            DataSet dataSet = new DataSet();
            DbConnection conn = dbContext.Database.Connection;
            DbProviderFactory? dbFactory = DbProviderFactories.GetFactory(conn);
            if (dbFactory != null)
            {
                using (var cmd = dbFactory.CreateCommand())
                {
                    if (cmd != null)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (var adapter = dbFactory.CreateDataAdapter())
                        {
                            if (adapter != null)
                            {
                                adapter.SelectCommand = cmd;
                                adapter.Fill(dataSet);
                            }
                        }
                    }
                }
            }

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return dataSet;
        }

        /// <summary>
        /// 查詢並傳回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="anonymousParameter">anonymous條件參數物件</param>
        /// <returns></returns>
        public DataTable SelectDataTable(string sql, object anonymousParameter)
        {
            var parameters = ConvertToSqlParameters(anonymousParameter);
            return SelectDataTable(sql, parameters);
        }

        /// <summary>
        /// 查詢並傳回DataTable
        /// </summary>
        /// <param name="sql">查詢語法</param>
        /// <param name="parameters">SqlParameters</param>
        /// <returns></returns>
        public DataTable SelectDataTable(string sql, params SqlParameter[]? parameters)
        {
            var ctx = GetDbContext();
            DataTable dt;
            if (IsPaging)
            {
                //分頁查詢
                dt = PagingSelectDataTable(sql, parameters, ctx);
            }
            else
            {
                //一般查詢
                dt = QueryDataTable(sql, parameters, ctx);
            }

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return dt;
        }

        /// <summary>
        /// 分頁查詢(回傳DataTable)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public DataTable PagingSelectDataTable(string sql, SqlParameter[]? parameters, DbContextBase? ctx = null)
        {
            if (ctx == null) ctx = GetDbContext();
            //階段一：(1)取得資料總筆數(2)更新分頁參數(3)產生分頁查詢sql
            string pagingSql = PagingSelect_Phase1(sql, parameters, ctx);

            //階段二：分頁查詢
            //因重複使用相同參數，需先clone以避免The SqlParameter is already contained by another SqlParameterCollection問題
            var clonedParameters = CloneSqlParameters(parameters);
            var dt = QueryDataTable(pagingSql, clonedParameters, ctx);

            //階段三：再更新分頁參數之起迄列編號
            PagingSelect_Phase3(dt.Rows.Count);

            return dt;
        }

        /// <summary>
        /// 查詢並回傳DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected DataTable QueryDataTable(string sql, SqlParameter[]? parameters, DbContextBase ctx)
        {
            DataTable dataTable = new DataTable();
            DbConnection conn = ctx.Database.Connection;
            DbProviderFactory? dbFactory = DbProviderFactories.GetFactory(conn);
            if (dbFactory != null)
            {
                using (var cmd = dbFactory.CreateCommand())
                {
                    if (cmd != null)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (var adapter = dbFactory.CreateDataAdapter())
                        {
                            if (adapter != null)
                            {
                                adapter.SelectCommand = cmd;
                                adapter.Fill(dataTable);
                            }
                        }
                    }
                }
            }
            return dataTable;
        }

        #endregion

        #region PagingSelect

        /// <summary>
        /// 設定資料分頁參數
        /// </summary>
        /// <param name="dataPageSetting">資料分頁參數</param>
        /// <returns></returns>
        public DbServiceBase SetPaging(ref PagingSetting dataPageSetting)
        {
            if (dataPageSetting == null || dataPageSetting.IsValid() == false)
            {
                throw new ArgumentNullException(nameof(dataPageSetting), $"SetPaging() failed. [dataPageSetting] must not be null. and its RowsPerPage and PageNo property value must larger than 0");
            }
            this.PagingSetting = dataPageSetting;
            return this;
        }

        /// <summary>
        /// 分頁查詢(泛型)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public List<TViewModel> PagingSelect<TViewModel>(string sql, SqlParameter[]? parameters, DbContextBase? ctx = null) where TViewModel : new()
        {
            if (ctx == null) ctx = GetDbContext();
            //階段一：(1)取得資料總筆數(2)更新分頁參數(3)產生分頁查詢sql
            string pagingSql = PagingSelect_Phase1(sql, parameters, ctx);

            //階段二：分頁查詢
            List<TViewModel> list;
            if (parameters != null && parameters.Length > 0)
            {
                //因重複使用相同參數，需先clone以避免The SqlParameter is already contained by another SqlParameterCollection問題
                var clonedParameters = CloneSqlParameters(parameters);
                list = ctx.Database.SqlQuery<TViewModel>(pagingSql, clonedParameters)?.ToList() ?? new List<TViewModel>();
            }
            else
            {
                list = ctx.Database.SqlQuery<TViewModel>(pagingSql)?.ToList() ?? new List<TViewModel>();
            }

            //階段三：再更新分頁參數之起迄列編號
            PagingSelect_Phase3(list.Count);

            return list;
        }
        /// <summary>
        /// 分頁查詢：階段一(1)取得資料總筆數 (2)更新分頁參數 (3)產生分頁查詢sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected string PagingSelect_Phase1(string sql, SqlParameter[]? parameters, DbContextBase ctx)
        {
            //檢核分頁查詢sql語法(必須包含 order by 語法)
            if (ValidatePagingSelectSql(sql) == false)
            {
                throw new ArgumentNullException(nameof(sql), "When doing paging select/query, the sql syntax must contain [order by] clause.");
            }

            if (PagingSetting!.TotalRowCount == 0 || PagingSetting.ForceRefreshTotalRowCount)
            {
                //取得全部列數
                PagingSetting.TotalRowCount = GetTotalRowCount(ctx, sql, parameters);
                double pageCount = (double)PagingSetting.TotalRowCount / PagingSetting.RowsPerPage;
                PagingSetting.PageCount = (int)Math.Ceiling(pageCount);
            }
            int offsetCount = (PagingSetting.PageNo - 1) * PagingSetting.RowsPerPage;
            int fetchCount = PagingSetting.RowsPerPage;
            //組合分頁查詢sql
            string pagingSql = $"{sql} offset {offsetCount} rows fetch next {fetchCount} rows only";
            return pagingSql;
        }

        /// <summary>
        /// 分頁查詢：階段三：更新分頁參數之起迄列編號
        /// </summary>
        /// <param name="rowCount">查詢結果資料筆數</param>
        protected void PagingSelect_Phase3(int rowCount)
        {
            int offsetCount = (PagingSetting!.PageNo - 1) * PagingSetting.RowsPerPage;
            if (rowCount == 0)
            {
                PagingSetting.StartRowNo = 0;
                PagingSetting.EndRowNo = 0;
            }
            else
            {
                PagingSetting.StartRowNo = offsetCount + 1;
                PagingSetting.EndRowNo = offsetCount + rowCount;
            }
        }
        /// <summary>
        /// 檢核分頁查詢sql語法(必須包含 order by 語法)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ValidatePagingSelectSql(string sql)
        {
            int orderByIndex = sql.LastIndexOf("order by", StringComparison.OrdinalIgnoreCase);
            return orderByIndex > -1;
        }

        /// <summary>
        /// 複製SqlParameterCollection(以供多次查詢時，避免：The SqlParameter is already contained by another SqlParameterCollection問題)
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected SqlParameter[]? CloneSqlParameters(SqlParameter[]? parameters)
        {
            if (parameters == null) return null;
            if (parameters.Length == 0) return new SqlParameter[] { };
            int count = parameters.Length;
            SqlParameter[] cloned = new SqlParameter[count];
            for (int i = 0; i < parameters.Length; i++)
            {
                cloned[i] = new SqlParameter(parameters[i].ParameterName, parameters[i].Value);
            }
            return cloned;
        }
        #endregion

        #region ExecuteSQL
        /// <summary>
        /// 執行SQL指令
        /// </summary>
        /// <param name="sql">SQL語法，若有含參數項，則要給入一個參數物件</param>
        /// <param name="anonymousParameter">anonymous參數物件</param>
        /// <param name="allowNoWhereUpdateOrDelete">是否允許無where條件的update或delete動作(預設值：false)</param>
        /// <returns></returns>
        public int ExecuteSQL(string sql, object anonymousParameter, bool allowNoWhereUpdateOrDelete = false)
        {
            //檢核update和delete的sql中必須包含where條件
            sql = sql.TrimStart();
            if ((!allowNoWhereUpdateOrDelete) && sql.Contains("update ", StringComparison.OrdinalIgnoreCase) || sql.Contains("delete ", StringComparison.OrdinalIgnoreCase))
            {
                if (sql.Contains(" where ", StringComparison.OrdinalIgnoreCase) == false)
                {
                    throw new InvalidOperationException("Any db update or delete operation without where condition is no allowed.");
                }
            }

            var ctx = GetDbContext();
            var parameters = ConvertToSqlParameters(anonymousParameter);
            int effectCount = ctx.Database.ExecuteSqlCommand(sql, parameters);

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return effectCount;
        }
        #endregion

        #region Call Stored Procedure
        /// <summary>
        /// 執行Stored Procedure(無參數)回傳TViewModel資料集
        /// </summary>
        /// <typeparam name="TViewModel">指定的回傳型別ViewModel</typeparam>
        /// <param name="storedProcedureName">Stored Procedure名稱</param>
        /// <returns></returns>
        public List<TViewModel> ExecuteProcedure<TViewModel>(string storedProcedureName) where TViewModel : new()
        {
            return ExecuteProcedure<TViewModel>(storedProcedureName,null);
        }
        /// <summary>
        /// 執行Stored Procedure(帶參數)回傳TViewModel資料集
        /// </summary>
        /// <typeparam name="TViewModel">指定的回傳型別ViewModel</typeparam>
        /// <param name="storedProcedureName">Stored Procedure名稱，若有含參數項，則要給入一個參數物件</param>
        /// <param name="anonymousParameter">anonymous參數物件(參數名稱和值型別必須與Stored Procedure的定義相符)</param>
        /// <returns></returns>
        public List<TViewModel> ExecuteProcedure<TViewModel>(string storedProcedureName, object? anonymousParameter) where TViewModel : new()
        {
            var parameters = ConvertToSqlParameters(anonymousParameter);
            if(parameters.Length > 0)
            {
                //若storedProcedureName中沒有帶入參數項，則自動補上參數項
                if (storedProcedureName.IndexOf('@') < 0)
                {
                    var paramArgExpression = string.Join(',', parameters.Select(x => $"@{x.ParameterName}").ToArray());
                    storedProcedureName = $"{storedProcedureName} {paramArgExpression}";
                }
            }
            //在EF 6中 執行Stored Procedure和Select都是透過相同的指令，只差別在sql表達式而已
            return Select<TViewModel>(parameters, storedProcedureName);
        }

        /// <summary>
        /// 執行Stored Procedure(無參數)回傳單一值
        /// </summary>
        /// <typeparam name="TValue">指定的回傳型別Value</typeparam>
        /// <param name="storedProcedureName">Stored Procedure名稱</param>
        /// <returns></returns>
        public TValue? ExecuteScalarProcedure<TValue>(string storedProcedureName) 
        {
            return ExecuteScalarProcedure<TValue>(storedProcedureName, null);
        }
        /// <summary>
        /// 執行Stored Procedure(帶參數)回傳TViewModel資料集
        /// </summary>
        /// <typeparam name="TValue">指定的回傳型別Value</typeparam>
        /// <param name="storedProcedureName">Stored Procedure名稱，若有含參數項，則要給入一個參數物件</param>
        /// <param name="anonymousParameter">anonymous參數物件(參數名稱和值型別必須與Stored Procedure的定義相符)</param>
        /// <returns></returns>
        public TValue? ExecuteScalarProcedure<TValue>(string storedProcedureName, object? anonymousParameter)
        {
            var parameters = ConvertToSqlParameters(anonymousParameter);
            if (parameters.Length > 0)
            {
                //若storedProcedureName中沒有帶入參數項，則自動補上參數項
                if (storedProcedureName.IndexOf('@') < 0)
                {
                    var paramArgExpression = string.Join(',', parameters.Select(x => $"@{x.ParameterName}").ToArray());
                    storedProcedureName = $"{storedProcedureName} {paramArgExpression}";
                }
            }
            //在EF 6中 執行Stored Procedure和Select都是透過相同的指令，只差別在sql表達式而已
            return SelectScalar<TValue>(storedProcedureName, anonymousParameter);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 取得總資料筆數(相同sql資料查詢條件下)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        private int GetTotalRowCount(DbContextBase ctx, string sql, SqlParameter[]? parameters = null)
        {
            string countSql = BuildRowCountSql(sql);
            int totalRowCount = 0;
            if (parameters != null && parameters.Length > 0)
            {
                totalRowCount = ctx.Database.SqlQuery<int>(countSql, parameters).First();
            }
            else
            {
                totalRowCount = ctx.Database.SqlQuery<int>(countSql).First();
            }

            return totalRowCount;

        }

        /// <summary>
        /// 將原sql改成查詢資料筆數的sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string BuildRowCountSql(string sql)
        {
            /*
             * (1)若有union語法，則用子查詢計算列數。
             * (2)若無union語法，則去掉from之前，和order by之後的部分再計算列數
             */

            int index_from = -1;
            int index_orderBy = -1;
			bool hasUnion = false;

			//判斷是否有union語法(從from之後開始找關鍵字)
			index_from = sql.IndexOf("from", StringComparison.OrdinalIgnoreCase);
			if (index_from >= 0) 
            {
				int unionIndex = sql.IndexOf("union", index_from, comparisonType: StringComparison.OrdinalIgnoreCase);
				hasUnion = unionIndex > 0;
				if (hasUnion)
				{
					int[] emptyChars = { 9, 13, 32 };
					char before = sql[unionIndex - 1];
					char after = sql[unionIndex + 5];
					//確認union非連續字串的一部分
					hasUnion = emptyChars.Contains(before) && emptyChars.Contains(after);
				}
            }

            
            if(hasUnion)
            {
				//只去掉order by之後的部分
				index_orderBy = sql.LastIndexOf("order by", StringComparison.OrdinalIgnoreCase);
				if (index_orderBy >= 0) { sql = sql.Substring(0, index_orderBy); }
				return $"select count(*) from ( {sql} ) sub1";
			}
			else
            {
			    //去掉from之前，和order by之後的部分
                if (index_from >= 0) { sql = sql.Substring(index_from); }
                index_orderBy = sql.LastIndexOf("order by", StringComparison.OrdinalIgnoreCase);
                if (index_orderBy >= 0) { sql = sql.Substring(0, index_orderBy); }
                return $"select count(*) {sql}";
            }

        }
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

            if (_dbContext == null || _dbContextDisposed) return;

            try
            {
                _dbContextDisposed = true;
                bool contextOwnsConnection = _dbContextReusable;
                /*
                 * 若contextOwnsConnection=true, 則_dbcontext.Dispose()會一併close且dispose dbconnection,
                 * 若contextOwnsConnection=false, 則_dbcontext.Dispose()時，不會close且dispose dbconnection，必須手動close且dispose dbconnection
                 */

                if (!contextOwnsConnection)
                {
                   try
                    {
                        if (_dbContext.Database.Connection != null && _dbContext.Database.Connection.State != System.Data.ConnectionState.Closed)
                        {
                            _dbContext.Database.Connection.Close();
                        }

                        if (_dbContext.Database.Connection != null)
                        {
                            _dbContext.Database.Connection.Dispose();
                        }
                    }
                    catch (Exception)
                    {
                        //do nothing. ignore the ObjectDisposedException
                    }
                }

                try
                {
                    _dbContext.Dispose();
                }
                catch (Exception)
                {
                    //do nothing. ignore the ObjectDisposedException
                }

            }
            catch (Exception)
            {
                //關閉連線時發生某些錯誤，繼續dispose dbcontext
                _dbContextDisposed = true;
                try
                {
                    _dbContext.Dispose();
                }
                catch (Exception)
                {
                    //do nothing. ignore the ObjectDisposedException
                }

            }
        }
        #endregion
    }

}

using Lighter.NET.Common;
namespace Lighter.NET.DB
{

    using System.Data;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Data.SqlClient;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Runtime.CompilerServices;
    using System.Text.Encodings.Web;
    using System.Text.Unicode;

    /// <summary>
    /// DB操作基礎函式(泛型TDataModel)
    /// </summary>
    /// <typeparam name="TDataModel">對應的Data Table Model或ViewModel</typeparam>
    /// <typeparam name="TLogModel">這支DbService用來寫入update或delete Log的Model</typeparam>
    public abstract class DbServiceBase<TDataModel,TLogModel> : DbServiceBase, IDisposable 
        where TDataModel : class, new()
        where TLogModel : IDbLog, new()
    {
        #region Static

        /// <summary>
        /// DataModel型別
        /// </summary>
        protected static Type _modelType;
        /// <summary>
        /// DataModel的「全部」欄位屬性資訊(不包含唯讀欄位)
        /// </summary>
        protected static PropertyInfo[] _modelProperties;
        /// <summary>
        /// DataModel的「鍵值」欄位屬性資訊
        /// </summary>
        protected static PropertyInfo[] _keyProperties;
        /// <summary>
        /// DataModel的「鍵值」欄位名稱
        /// </summary>
        protected static string[] _keyNames;
        /// <summary>
        /// 是否含有主鍵欄位
        /// </summary>
        protected static bool _hasKey;
        /// <summary>
        /// LogModel的「全部」欄位屬性資訊(不包含唯讀欄位)
        /// </summary>
        protected static PropertyInfo[] _logProperties;
        /// <summary>
        /// 寫入log的SQL Script
        /// </summary>
        protected static string _logInsertSQL = "";
        static DbServiceBase()
        {
			
			_modelType = typeof(TDataModel);
            _modelProperties = _modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite == true)?.ToArray() ?? new PropertyInfo[0];
            //若DataModel有主鍵欄位則取出欄位備用
            var noKeyAttr = _modelType.GetCustomAttribute<NoKeyAttribute>(false);
            _hasKey = noKeyAttr == null;
			_keyProperties = GetModelKeyProperties(_modelProperties);
            _keyNames = _keyProperties.Select(p => p.Name).ToArray();
            _logProperties = typeof(TLogModel).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite == true)?.ToArray() ?? new PropertyInfo[0];
        }
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

        #region Abstract Property
        /// <summary>
        /// 完整Data Table名稱(格式：[DbName].[Schema].[TableName]，範例： DbName.dbo.DataTableName)
        /// 這支DbService所對應的db table name
        /// ,db table name that used to access data by this DbService
        /// </summary>
        public abstract string DataTableName { get; protected set; }
        /// <summary>
        /// 完整Log Table名稱(格式：[DbName].[Schema].[TableName]，範例： DbName.dbo.LogTableName)
        /// 這支DbService在做update和delete時，要寫log的tb table name
        /// , db table name that used to write update or delete log by this DbService
        /// </summary>
        public abstract string LogTableName { get; protected set; }
        #endregion

        #region Properties
        /// <summary>
        /// update/delete前的資料
        /// </summary>
        protected new List<TDataModel> _dataBefore { get; set; } = new List<TDataModel>();
        /// <summary>
        /// update後的資料
        /// </summary>
        protected new List<TDataModel> _dataAfter { get; set; } = new List<TDataModel>();
        #endregion

        #region Exist
        /// <summary>
        /// 檢核符合參數條件的資料是否已存在
        /// </summary>
        /// <param name="anonymousParameter">參數條件</param>
        /// <returns></returns>
        public bool Exist(object anonymousParameter)
        {
            var ctx = GetDbContext();
            var parameters = ConvertToSqlParameters(anonymousParameter);
            string[] whereKeyNames = parameters.Select(x => x.ParameterName.TrimStart('@')).ToArray();
            string whereScript = GetWhereScript(whereKeyNames);
            string sql = $"select top 1 * from {DataTableName} {whereScript} ";
            var list = ctx.Database.SqlQuery<TDataModel>(sql, parameters).FirstOrDefault();
            bool exist = (list != null);
            return exist;
        }

        /// <summary>
        /// 檢核model的Key值是否已存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool KeyExist(TDataModel model)
        {
            if (!_hasKey)
            {
                throw new InvalidOperationException($"The {_modelType.Name} has no primary key defined.");
            }

            var parameters = GetSqlParameters(model, _keyNames);
            string whereScript = GetWhereScript(_keyNames);
            string sql = $"select top 1 * from {DataTableName} {whereScript} ";

            var ctx = GetDbContext();
            var list = ctx.Database.SqlQuery<TDataModel>(sql, parameters).FirstOrDefault();
            bool exist = (list != null);
            return exist;
        }
        #endregion

        #region Column Mapping
        /// <summary>
        /// 使用語系欄位對應
        /// </summary>
        /// <param name="columnSelectors">欄位選擇器(指定要「被」語系對應的欄位)</param>
        /// <returns></returns>
        public DbServiceBase<TDataModel,TLogModel> UseCultureColumn(params Expression<Func<TDataModel, object?>>[] columnSelectors)
        {
            if (columnSelectors != null && columnSelectors.Length > 0)
            {
                for (int i = 0; i < columnSelectors.Length; i++)
                {
                    string columnName = columnSelectors[i].GetLambdaPropertyName();
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        if (!CultureMappingColumnNames.Contains(columnName))
                        {
                            CultureMappingColumnNames.Add(columnName);
                        }
                    }
                }
            }

            return this;
        }
		#endregion

		#region Method
		/// <summary>
		/// 可重複使用
		/// </summary>
		/// <returns></returns>
		public override DbServiceBase<TDataModel, TLogModel> Reusable()
		{
			_dbContextReusable = true;
			return this;
		}
		#endregion

		#region Select
		/// <summary>
		/// 查詢TTableModel資料集
		/// </summary>
		/// <param name="sql">SQL語法</param>
		/// <returns></returns>
		public List<TDataModel> Select(string sql)
        {
            var ctx = GetDbContext();
            List<TDataModel> list;

            if (IsPaging)
            {
                //分頁查詢
                list = PagingSelect(sql, ctx);
            }
            else
            {
                //一般查詢
                list = ctx.Database.SqlQuery<TDataModel>(sql)?.ToList() ?? new List<TDataModel>();
            }

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return list;
        }

        /// <summary>
        /// 查詢TTableModel資料集
        /// </summary>
        /// <param name="sql">SQL語法，若有含參數項，則要給入一個參數物件</param>
        /// <param name="anonymousParameter">anonymous條件參數物件</param>
        /// <returns></returns>
        public List<TDataModel> Select(string sql, object anonymousParameter)
        {
            var ctx = GetDbContext();
            var parameters = ConvertToSqlParameters(anonymousParameter);
            List<TDataModel> list;
            if (IsPaging)
            {
                //分頁查詢
                list = PagingSelect(sql, parameters, ctx);
            }
            else
            {
                //一般查詢
                list = ctx.Database.SqlQuery<TDataModel>(sql, parameters)?.ToList() ?? new List<TDataModel>();
            }

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return list;
        }


        /// <summary>
        /// 查詢符合指定過濾條件的資料
        /// </summary>
        /// <param name="anonymousParameter">過濾條件(AND運算)</param>
        /// <returns></returns>
        public List<TDataModel> Select(object anonymousParameter)
        {
            var ctx = GetDbContext();
            var parameters = ConvertToSqlParameters(anonymousParameter);
            string[] whereKeyNames = parameters.Select(x => x.ParameterName.TrimStart('@')).ToArray();
            string whereScript = GetWhereScript(whereKeyNames);
            string sql = $"select * from {DataTableName} {whereScript} ";
            var list = ctx.Database.SqlQuery<TDataModel>(sql, parameters)?.ToList();
            if (list == null) list = new List<TDataModel>();

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return list;
        }

        /// <summary>
        /// 查詢全部TTableModel資料集
        /// </summary>
        /// <returns></returns>
        public List<TDataModel> SelectAll()
        {
            var ctx = GetDbContext();
            string sql = $"select * from {DataTableName}";
            var list = ctx.Database.SqlQuery<TDataModel>(sql)?.ToList();
            if (list == null) list = new List<TDataModel>();

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return list;
        }

        /// <summary>
        /// 查詢全部TViewModel資料集
        /// </summary>
        /// <typeparam name="TViewModel">指定的回傳型別ViewModel</typeparam>
        /// <returns></returns>
        public List<TViewModel> SelectAll<TViewModel>()
        {
            var ctx = GetDbContext();
            string sql = $"select * from {DataTableName}";
            var list = ctx.Database.SqlQuery<TViewModel>(sql)?.ToList();
            if (list == null) list = new List<TViewModel>();

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();
            return list;
        }

        /// <summary>
        /// 查詢選項清單
        /// </summary>
        /// <param name="textField">文字欄位選擇器</param>
        /// <param name="valueField">值欄位選擇器</param>
        /// <param name="orderbyField">排序欄位選擇器</param>
        /// <param name="orderByType">排序方式</param>
        /// <returns></returns>
        public List<DbOptionItem> SelectOptionItems(
            Expression<Func<TDataModel, object?>> textField,
            Expression<Func<TDataModel, object?>> valueField,
            Expression<Func<TDataModel, object?>>? orderbyField = null,
            OrderByType orderByType = OrderByType.ASC)
        {
            string textFieldName = GetLambdaPropertyName(textField);
            string valueFieldName = GetLambdaPropertyName(valueField);
            string orderBy = "";
            if (orderbyField != null)
            {
                string orderByFieldName = GetLambdaPropertyName(orderbyField);
                if (orderByFieldName != null)
                {
                    orderBy = $" order by {orderByFieldName} {orderByType.ToString()}";
                }
            }

            //若有使用語系對應欄位
            if (CultureMappingColumnNames != null && CultureMappingColumnNames.Count > 0)
            {
                if (CultureMappingColumnNames.Contains(textFieldName))
                {
                    var mappingConfig = CultureMappingColumnConfig();
                    textFieldName = mappingConfig.Map(textFieldName);
                }
            }

            string sql = $@"select COALESCE(cast({textFieldName} as varchar(255)),'') as Text,
                                   COALESCE(cast({valueFieldName} as varchar(255)),'') as Value 
                            from {DataTableName} {orderBy}";
            var ctx = GetDbContext();
            var list = ctx.Database.SqlQuery<DbOptionItem>(sql)?.ToList();
            if (list == null) list = new List<DbOptionItem>();

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();
            return list;
        }



        /// <summary>
        /// 查詢指定欄位並傳回DataTable
        /// </summary>
        /// <param name="columnSelector"></param>
        /// <param name="anonymousParameter"></param>
        /// <returns></returns>
        public DataTable SelectDataTable(Expression<Func<TDataModel, object>> columnSelector, object anonymousParameter)
        {
            var selectColumnNames = GetColumnNames(columnSelector);
            var parameters = ConvertToSqlParameters(anonymousParameter);
            var whereColumnNames = parameters.Select(x => x.ParameterName.TrimStart('@')).ToArray();
            string whereScript = GetWhereScript(whereColumnNames);
            string sql = $"select {string.Join(',', selectColumnNames)} from {DataTableName} {whereScript}";
            return SelectDataTable(sql, parameters);
        }


        #region SelectSingle
        /// <summary>
        /// 查詢單獨一筆資料
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public TDataModel? SelectSingle(string sql)
        {
            return Select(sql).FirstOrDefault();
        }
        /// <summary>
        /// 查詢單獨一筆資料
        /// </summary>
        /// <param name="anonymousParameter">匿名型別的查詢參數</param>
        /// <returns></returns>

        public TDataModel? SelectSingle(object anonymousParameter)
        {
            return Select(anonymousParameter).FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="anonymousParameter"></param>
        /// <returns></returns>
        public TDataModel? SelectSingle(string sql, object anonymousParameter)
        {
            return Select(sql, anonymousParameter).FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="sql"></param>
        /// <param name="anonymousParameter"></param>
        /// <returns></returns>
        public TModel? SelectSingle<TModel>(string sql, object anonymousParameter) where TModel : new()
        {
            return Select<TModel>(sql, anonymousParameter).FirstOrDefault();
        }
        #endregion (SelectSingle)

        #endregion (Select)

        #region PagingSelect
        /// <summary>
        /// 設定資料分頁參數
        /// </summary>
        /// <param name="dataPageSetting">資料分頁參數</param>
        /// <returns></returns>
        public virtual new DbServiceBase<TDataModel, TLogModel> SetPaging(ref PagingSetting dataPageSetting)
        {
            if (dataPageSetting == null || dataPageSetting.IsValid() == false)
            {
                throw new ArgumentNullException(nameof(dataPageSetting), $"SetPaging() failed. [dataPageSetting] must not be null. and its RowsPerPage and PageNo property value must larger than 0");
            }
            this.PagingSetting = dataPageSetting;
            return this;
        }

        /// <summary>
        /// 分頁查詢
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ctx">dbcontext used to execute the select operation</param>
        /// <returns></returns>
        public List<TDataModel> PagingSelect(string sql, DbContextBase? ctx = null)
        {
            return PagingSelect(sql, null, ctx);
        }

        /// <summary>
        /// 分頁查詢
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public List<TDataModel> PagingSelect(string sql, SqlParameter[]? parameters, DbContextBase? ctx = null)
        {
            if (ctx == null) ctx = GetDbContext();
            //階段一：(1)取得資料總筆數(2)更新分頁參數(3)產生分頁查詢sql
            string pagingSql = PagingSelect_Phase1(sql, parameters, ctx);

            //階段二：分頁查詢
            List<TDataModel> list;
            if (parameters != null && parameters.Length > 0)
            {
                //因重複使用相同參數，需先clone以避免The SqlParameter is already contained by another SqlParameterCollection問題
                var clonedParameters = CloneSqlParameters(parameters);
                list = ctx.Database.SqlQuery<TDataModel>(pagingSql, clonedParameters)?.ToList() ?? new List<TDataModel>();
            }
            else
            {
                list = ctx.Database.SqlQuery<TDataModel>(pagingSql)?.ToList() ?? new List<TDataModel>();
            }

            //階段三：再更新分頁參數之起迄列編號
            PagingSelect_Phase3(list.Count);

            return list;
        }

        #endregion (PagingSelect)

        #region Insert

        /// <summary>
        /// 新增單筆
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Insert(TDataModel model)
        {
            if (model == null) throw new ArgumentNullException("[model] argument is null");

            int effectCount = Insert(new List<TDataModel>() { model });

            return effectCount;
        }

        /// <summary>
        /// 新增多筆
        /// </summary>
        /// <param name="modelList"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Insert(List<TDataModel> modelList)
        {
            if (modelList == null || modelList.Count == 0) throw new ArgumentNullException(nameof(modelList), $"[{nameof(modelList)}] argument is null or empty");

            var columnNames = GetColumnNames();
            int effectCount = DoInsert(modelList, columnNames);

            return effectCount;
        }

        /// <summary>
        /// 新增單筆(只寫入指定欄位)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columnSelector">欄位選擇器</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Insert(TDataModel model, Expression<Func<TDataModel, object>> columnSelector)
        {
            if (model == null) throw new ArgumentNullException("[model] argument is null");

            int effectCount = Insert(new List<TDataModel>() { model }, columnSelector);

            return effectCount;
        }

        /// <summary>
        /// 新增多筆(只寫入指定欄位)
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="columnSelector">欄位選擇器</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Insert(List<TDataModel> modelList, Expression<Func<TDataModel, object>> columnSelector)
        {
            if (modelList == null || modelList.Count == 0) throw new ArgumentNullException(nameof(modelList), $"[{nameof(modelList)}] argument is null or empty");

            var columnNames = GetColumnNames(columnSelector);
            int effectCount = DoInsert(modelList, columnNames);

            return effectCount;
        }

        /// <summary>
        /// 執行Insert動作
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="columnNames">指定要寫入資料的欄位</param>
        /// <returns></returns>
        private int DoInsert(List<TDataModel> modelList, string[] columnNames)
        {
            string[] columnNamesFinal;
            //自動附加[建立者/時間]欄位
            bool hasCreatorFields = _modelType.IsAssignableTo(typeof(ICreator));
            if (hasCreatorFields)
            {
                columnNamesFinal = columnNames.Except(_updaterFields).Union(_creatorFields).ToArray();
            }
            else
            {
                columnNamesFinal = columnNames;
            }

            var dbCtx = GetDbContext();
            var sql = GetInsertSql(DataTableName, columnNamesFinal);
            int effectCount = 0;
            var userInfo = GetUserInfo();
            foreach (var model in modelList)
            {

                if (hasCreatorFields)
                {
                    var creator = (model as ICreator)!;
                    creator.createAt = DateTime.Now;
                    creator.createBy = $"{userInfo.UserId}_{userInfo.UserName}";
                    creator.createIp = userInfo.IpAddress;
                }

                var parameters = GetSqlParameters(model, columnNamesFinal);
                dbCtx.Database.ExecuteSqlCommand(sql, parameters);
                effectCount++;
            }

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return effectCount;
        }
        #endregion

        #region Update

        /// <summary>
        /// 更新單筆
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callerFuctionName"></param>
        /// <returns>effected count</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Update(TDataModel model, [CallerMemberName] string callerFuctionName = "")
        {
            if (model == null) throw new ArgumentNullException("[model] argument is null");
            int effectCount = Update(new List<TDataModel>() { model }, callerFuctionName);
            return effectCount;
        }

        /// <summary>
        /// 更新單筆(包含特定欄位)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columnSelector">欄位選擇器</param>
        /// <param name="callerFuctionName"></param>
        /// <returns>effected count</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Update(TDataModel model, Expression<Func<TDataModel, object>> columnSelector, [CallerMemberName] string callerFuctionName = "")
        {
            if (model == null) throw new ArgumentNullException("[model] argument is null");
            int effectCount = Update(new List<TDataModel>() { model }, columnSelector, callerFuctionName);
            return effectCount;
        }

		/// <summary>
		/// 更新多筆
		/// </summary>
		/// <param name="modelList"></param>
        /// <param name="callerFuctionName"></param>
		/// <returns>effected count</returns>
		public int Update(List<TDataModel> modelList, [CallerMemberName] string callerFuctionName = "")
        {
			//無主鍵不允許更新model(防止誤更新非指定資料)
			if (_hasKey == false) throw new InvalidOperationException($"Update() method is not allowed for [{_modelType.Name}] model type which has no primary key defined with [Key] attribute");

			if (modelList == null || modelList.Count == 0) throw new ArgumentNullException(nameof(modelList), $"[{nameof(modelList)}] argument is null or empty");
            var columnNames = GetTableColumnNames();
            //var keyNames = GetModelKeys();
            //var keyNames = GetColumnNames(keySelector);
            int effectCount = DoUpdate(modelList, columnNames, _keyNames, callerFuctionName);
            return effectCount;
        }

        /// <summary>
        /// 更新多筆(包含特定欄位)
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="columnSelector">欄位選擇器</param>
        /// <param name="callerFuctionName"></param>
        /// <returns>effected count</returns>
        public int Update(List<TDataModel> modelList, Expression<Func<TDataModel, object>> columnSelector, [CallerMemberName] string callerFuctionName = "")
        {
            //無主鍵不允許更新model(防止誤更新非指定資料)
            if (_hasKey == false) throw new InvalidOperationException($"Update() method is not allowed for [{_modelType.Name}] model type which has no primary key defined with [Key] attribute");
            if (modelList == null || modelList.Count == 0) throw new ArgumentNullException(nameof(modelList), $"[{nameof(modelList)}] argument is null or empty");
            var columnNames = GetColumnNames(columnSelector);
            //var keyNames = GetColumnNames(keySelector);
            //var keyNames = GetModelKeys();
            int effectCount = DoUpdate(modelList, columnNames, _keyNames, callerFuctionName);
            return effectCount;
        }

		/// <summary>
		/// 更新單筆(排除特定欄位)
		/// </summary>
		/// <param name="model"></param>
		/// <param name="exceptColumnSelector">欄位選擇器</param>
        /// <param name="callerFuctionName"></param>
		/// <returns>effected count</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public int UpdateExcept(TDataModel model, Expression<Func<TDataModel, object>> exceptColumnSelector, [CallerMemberName] string callerFuctionName = "")
		{
			if (model == null) throw new ArgumentNullException("[model] argument is null");
			int effectCount = UpdateExcept(new List<TDataModel>() { model }, exceptColumnSelector, callerFuctionName);
			return effectCount;
		}

		/// <summary>
		/// 更新多筆(排除特定欄位)
		/// </summary>
		/// <param name="modelList"></param>
		/// <param name="exceptColumnSelector">欄位選擇器</param>
        /// <param name="callerFuctionName"></param>
		/// <returns>effected count</returns>
		public int UpdateExcept(List<TDataModel> modelList, Expression<Func<TDataModel, object>> exceptColumnSelector, [CallerMemberName] string callerFuctionName = "")
		{
			//無主鍵不允許更新model(防止誤更新非指定資料)
			if (_hasKey == false) throw new InvalidOperationException($"Update() method is not allowed for [{_modelType.Name}] model type which has no primary key defined with [Key] attribute");
			if (modelList == null || modelList.Count == 0) throw new ArgumentNullException(nameof(modelList), $"[{nameof(modelList)}] argument is null or empty");
			var exceptColumnNames = GetColumnNames(exceptColumnSelector);
            var columnNames = GetTableColumnNames().Except(exceptColumnNames).ToArray();
			int effectCount = DoUpdate(modelList, columnNames, _keyNames, callerFuctionName);
			return effectCount;
		}

		/// <summary>
		/// 執行Update
		/// </summary>
		/// <param name="modelList"></param>
		/// <param name="columnNames">指定要更新的欄位</param>
		/// <param name="keyNames">條件式的Key值欄位</param>
        /// <param name="callerFuctionName"></param>
		/// <returns>effected count</returns>
		private int DoUpdate(List<TDataModel> modelList, string[] columnNames, string[] keyNames, [CallerMemberName] string callerFuctionName = "")
        {
            //無主鍵不允許更新model(防止誤更新非指定資料)
            if (_hasKey == false) throw new InvalidOperationException($"Update() method is not allowed for [{_modelType.Name}] model type which has no primary key defined with [Key] attribute");

			string[] columnNamesFinal;
            //自動附加[更新者/時間]欄位
            bool hasUpdaterFields = _modelType.IsAssignableTo(typeof(IUpdater));
            if (hasUpdaterFields)
            {
                columnNamesFinal = columnNames.Except(_creatorFields).Union(_updaterFields).ToArray();
            }
            else
            {
                columnNamesFinal = columnNames;
            }

            var sql = GetUpdateSql(DataTableName, columnNamesFinal, keyNames);
            //檢核若sql中不含where條件，則不允許update動作(保險措施)
            if (sql.Contains(" where ", StringComparison.OrdinalIgnoreCase) == false)
            {
                throw new InvalidOperationException("Any db update operation without where condition is not allowed.");
            }

            //鍵值欄位也要加入參數項
            var paramNames = columnNamesFinal.Union(keyNames).ToArray();
            int effectCount = 0;
            var dbCxt = GetDbContext();
            var userInfo = GetUserInfo();

            //NTOE:先寫Log再Update
            //(1)Log
            //若未給入更新前資料，則從Db撈出現有資料
            if (_dataBefore == null || _dataBefore.Count==0)
            {
                _dataBefore = GetModelListBefore(modelList, keyNames);
            }

            //寫入log
            Log(_dataBefore, DbLogType.Update, modelList,_logMemo, callerFuctionName);

            //(2)Update
            foreach (var model in modelList)
            {
                if (hasUpdaterFields)
                {
                    var updater = (model as IUpdater)!;
                    updater.updateAt = DateTime.Now;
                    updater.updateBy = $"{userInfo.UserId}_{userInfo.UserName}";
                    updater.updateIp = userInfo.IpAddress;
                }
                var parameters = GetSqlParameters(model, paramNames);
                dbCxt.Database.ExecuteSqlCommand(sql, parameters);
                effectCount++;
            }

            //(3)清除Log參數和暫存資料
            ResetLog();

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return effectCount;
        }
        #endregion

        #region Delete
        /// <summary>
        /// 刪除(符合條件參數)的資料
        /// </summary>
        /// <param name="anonymousParameter">條件參數(例如：new {key=value})</param>
        /// <param name="callerFuctionName">呼叫端的函式名稱</param>
        /// <returns>effected count</returns>
        public int Delete(object anonymousParameter, [CallerMemberName] string callerFuctionName = "")
        {
			//無主鍵不允許刪除model(防止誤刪除非指定資料)
			if (_hasKey == false) throw new InvalidOperationException($"Delete() method is not allowed for [{_modelType.Name}] model type which has no primary key defined with [Key] attribute");

			var dataListToDelete = Select(anonymousParameter);
            SetLog(dataListToDelete);
            return Delete(dataListToDelete, callerFuctionName);
        }

		/// <summary>
		/// 刪除model(model中的key值欄位必須有值)
		/// </summary>
		/// <param name="model"></param>
		/// <param name="callerFuctionName">呼叫端的函式名稱</param>
		/// <returns>effected count</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public int Delete(TDataModel model, [CallerMemberName] string callerFuctionName = "")
        {
            if (model == null) throw new ArgumentNullException("[model] argument is null");
            int effectCount = Delete(new List<TDataModel>() { model }, callerFuctionName);
            return effectCount;
        }

		/// <summary>
		/// 刪除model list(model中的key值欄位必須有值)
		/// </summary>
		/// <param name="modelList"></param>
		/// <param name="callerFuctionName">呼叫端的函式名稱</param>
		/// <returns>effected count</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public int Delete(List<TDataModel> modelList, [CallerMemberName] string callerFuctionName = "")
        {
			//無主鍵不允許刪除model(防止誤刪除非指定資料)
			if (_hasKey == false) throw new InvalidOperationException($"Delete() method is not allowed for [{_modelType.Name}] model type which has no primary key defined with [Key] attribute");

			if (modelList == null || modelList.Count == 0) throw new ArgumentNullException(nameof(modelList), $"[{nameof(modelList)}] argument is null or empty");
            //var keyNames = GetModelKeys();
            string sql = GetDeleteSql(DataTableName, _keyNames);
            int effectCount = 0;
            var dbCtx = GetDbContext();

            //NTOE:先寫Log再Delete
            //(1)Log
            //若未給入更新前資料，則從Db撈出現有資料
            if (_dataBefore == null || _dataBefore.Count == 0)
            {
                _dataBefore = GetModelListBefore(modelList, _keyNames);
            }

            //寫入log
            Log(_dataBefore, DbLogType.Delete, null, _logMemo, callerFuctionName);

            //(2)Delete
            foreach (var model in modelList)
            {
                var parameters = GetSqlParameters(model, _keyNames);
                dbCtx.Database.ExecuteSqlCommand(sql, parameters);
                effectCount++;
            }

            //(3)清除Log參數和暫存資料
            ResetLog();

            //dispose db context if needed
            if (DbContextResuable == false) DisposeDbContext();

            return effectCount;
        }

		#endregion

		#region Save Change List
		/// <summary>
		/// 儲存變更資料集
		/// </summary>
		/// <param name="changeList">變更資料集</param>
		/// <returns></returns>
		public int SaveChangeList(List<StatefulModel<TDataModel>> changeList)
        {
            if (changeList == null || changeList.Count == 0) return 0;
            int effectCount = 0;
            bool originalReusable = DbContextResuable; //先保存原本的reusable設定
            DisposeDbContext(); //先dispose掉先前的context,在此transcaction之內，重新起一支context
            var dbCtx = GetDbContext(true); //在此transaction之內，設成可重複使用
            
            using(var transaction = dbCtx.Database.BeginTransaction())
            {
                try
                {
                    //(1)insert
                    var insertList = changeList.Where(x => x.State == "added")?.Select(x=>x.Model)?.ToList()??null;
                    if(insertList != null && insertList.Count > 0)
                    {
						effectCount += Insert(insertList);
                    }
					
					//(2)update
					var updateList = changeList.Where(x => x.State == "updated")?.Select(x => x.Model)?.ToList() ?? null;
					if (updateList != null && updateList.Count > 0)
					{
						effectCount += Update(updateList);
					}
					//(3)delete
					var deleteList = changeList.Where(x => x.State == "deleted")?.Select(x => x.Model)?.ToList() ?? null;
					if (deleteList != null && deleteList.Count > 0)
					{
						effectCount += Delete(deleteList);
					}

					transaction.Commit();
                    
                    //dispose掉此transaction中所用的context
                    DisposeDbContext();

                    //將reusable恢復成原本的值
                    _dbContextReusable = originalReusable;
                }
                catch (Exception)
                {
                    transaction.Rollback();

					//dispose掉此transaction中所用的context
					DisposeDbContext();

					//將reusable恢復成原本的值
					_dbContextReusable = originalReusable;

					throw;
                }
            }
			
            //dispose掉此transaction中所用的context
			DisposeDbContext();

			//將reusable恢復成原本的值
			_dbContextReusable = originalReusable;

			return effectCount;
		}

        #endregion

        #region Log
        /// <summary>
        /// 設定Log
        /// </summary>
        /// <param name="isLog">是否記錄log(針對update和delete)，預設:true</param>
        /// <returns></returns>
        public DbServiceBase<TDataModel, TLogModel> SetLog(bool isLog=true)
        {
            ResetLog();
            _isLog = isLog;
            return this;
        }

        /// <summary>
        /// 設定Log
        /// </summary>
        /// <param name="dataBefore">變更前的資料物件</param>
        /// <returns></returns>
        public DbServiceBase<TDataModel, TLogModel> SetLog(TDataModel? dataBefore)
        {
            ResetLog();
            if (dataBefore != null)
            {
                _dataBefore.Add(dataBefore);
            }

            return this;
        }

        /// <summary>
        /// 設定Log
        /// </summary>
        /// <param name="dataBefore">變更前的資料物件</param>
        /// <param name="logMemo">log備註</param>
        /// <returns></returns>
        public DbServiceBase<TDataModel, TLogModel> SetLog(TDataModel? dataBefore, string logMemo)
        {
            ResetLog();
            if (dataBefore != null)
            {
                _dataBefore.Add(dataBefore);
            }
            _logMemo = logMemo;

            return this;
        }

        /// <summary>
        /// 設定Log
        /// </summary>
        /// <param name="dataBefore">變更前的資料物件</param>
        /// <returns></returns>
        public DbServiceBase<TDataModel, TLogModel> SetLog(List<TDataModel>? dataBefore)
        {
            ResetLog();
            return SetLog(dataBefore,"");
        }

        /// <summary>
        /// 設定Log
        /// </summary>
        /// <param name="dataBefore">變更前的資料物件</param>
        /// <param name="logMemo">log備註</param>
        /// <returns></returns>
        public DbServiceBase<TDataModel, TLogModel> SetLog(List<TDataModel>? dataBefore, string logMemo)
        {
            ResetLog();
            if (dataBefore != null)
            {
                _dataBefore = dataBefore;
            }
            _logMemo = logMemo;
            return this;
        }

        /// <summary>
        /// 設定Log
        /// </summary>
        /// <param name="logMemo">log備註</param>
        public DbServiceBase<TDataModel, TLogModel> SetLog(string logMemo)
        {
            ResetLog();
            _logMemo = logMemo;
            return this;
        }

        /// <summary>
        /// 重新log參數和暫存資料
        /// </summary>
        public void ResetLog()
        {
            _isLog = true;
            _logMemo = "";
            _dataBefore = new List<TDataModel>();
            _dataAfter = new List<TDataModel>();
        }

        /// <summary>
        /// 加入資料變更Log
        /// </summary>
        /// <param name="dataBefore">變更前的modelList</param>
        /// <param name="logType">log分類</param>
        /// <param name="dataAfter">變更後的modelList</param>
        /// <param name="memo">remarks for the log</param>
        /// <param name="callerFuctionName">呼叫來源函式名稱</param>
        /// <returns></returns>
        protected int Log(List<TDataModel> dataBefore,DbLogType logType = DbLogType.Undefined, List<TDataModel>? dataAfter=null, string memo = "", [CallerMemberName] string callerFuctionName = "")
        {
            /*
             * 若dataAfter無值，則先暫存dataBefore，待後續有執行update或delete時再寫入Log
             * 若dataAfter有值，則寫入Log
             */

            int effectCount = 0;
            /*避免Json序列化時，對中文進行unicode編碼轉換*/
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            var log = new TLogModel();
            log.log_guid = Guid.NewGuid();
            log.log_type = (int)logType;
            log.data_bf_json = JsonSerializer.Serialize(dataBefore, jsonOptions);
            log.data_af_json = JsonSerializer.Serialize(dataAfter, jsonOptions);
            log.table_name = DataTableName;
            log.source_func_name = callerFuctionName;
            log.SetUpdater();
            log.memo = memo;
            log.source_app_code = DbServiceConfig.AppCode;
            //保存變更前資料的第1筆的Key值
            if (dataBefore != null && dataBefore.Count > 0)
            {
                log.data_key = _keyProperties[0].GetValue(dataBefore.First())?.ToString() ?? "";
                if (_keyProperties.Length > 1)
                {
                    log.data_key2 = _keyProperties[1].GetValue(dataBefore.First())?.ToString() ?? "";
                }
            }
            if (_logInsertSQL == "")
            {
                _logInsertSQL = GetInsertSql(LogTableName, _logProperties.Select(p => p.Name).ToArray());
            }
            var dbCtx = GetDbContext();
            var parameters = GetSqlParameters(log, _logProperties);
            effectCount = dbCtx.Database.ExecuteSqlCommand(_logInsertSQL, parameters);

            return effectCount;
        }
        #endregion

        #region Utils (Private/Protected)

        /// <summary>
        /// 取得TModel的類別宣告中有指定[Key] attribute的member property
        /// </summary>
        /// <returns></returns>
        protected static PropertyInfo[] GetModelKeyProperties(PropertyInfo[]? props = null)
        {
            if (!_hasKey)
            {
                //無主鍵
                return new PropertyInfo[0];
            }

            if (props == null)
            {
                props = _modelType.GetProperties();
            }

            Type keyAttributeType = typeof(KeyAttribute);

            var keyProps = props.Where(p=> Attribute.IsDefined(p,keyAttributeType,true))?.ToArray();
            if (keyProps == null || keyProps.Length ==0) throw new MissingMemberException($"The [Key] attribute is not specified for any one of the {_modelType.FullName} class members.");
            return keyProps;

            //List<string> keys = new List<string>();
            //foreach (var prop in props)
            //{
            //    var keyAttr = prop.GetCustomAttributes(typeof(KeyAttribute), false);
            //    if (keyAttr != null && keyAttr.Length > 0)
            //    {
            //        keys.Add(prop.Name);
            //    }
            //}

            //if (keys.Count == 0) throw new MissingMemberException($"The [Key] attribute is not specified for any one of the {DataTableName} class members.");
            //return keys.ToArray();
        }

        /// <summary>
        /// 取得TModel的類別宣告中有指定[Key] attribute的member property Name
        /// </summary>
        /// <returns></returns>
        protected static string[] GetModelKeyNames(PropertyInfo[]? props = null)
        {
			if (!_hasKey)
			{
				//無主鍵
				return new string[0];
			}

			if (props == null)
            {
                props = _modelType.GetProperties();
            }
            List<string> keys = new List<string>();
            foreach (var prop in props)
            {
                var keyAttr = prop.GetCustomAttributes(typeof(KeyAttribute), false);
                if (keyAttr != null && keyAttr.Length > 0)
                {
                    keys.Add(prop.Name);
                }
            }

            if (keys.Count == 0) throw new MissingMemberException($"The [Key] attribute is not specified for any one of the {_modelType.Name} class members.");
            return keys.ToArray();
        }

        /// <summary>
        /// 取得db table的全部欄位名稱陣列
        /// </summary>
        /// <returns></returns>
        private string[] GetTableColumnNames()
        {
            return _modelProperties.Select(x => x.Name).ToArray(); ;
        }

        /// <summary>
        /// 取得指定的欄位名稱陣列
        /// </summary>
        /// <param name="columnSelector">欄位選擇器</param>
        /// <returns></returns>
        private string[] GetColumnNames(Expression<Func<TDataModel, object>>? columnSelector = null)
        {
            var tableColumns = GetTableColumnNames();
            if (columnSelector != null)
            {
                var body = columnSelector.Body as NewExpression;
                if (body == null) return new string[] { };
                var selectedColumnNames = body.Type.GetProperties().Where(p => tableColumns.Contains(p.Name)).Select(p => p.Name).ToArray();
                return selectedColumnNames;
            }
            else
            {
                return tableColumns;
            }
        }

        /// <summary>
        /// 產生Insert的SQL指令
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        private string GetInsertSql(string tableName, string[] columnNames)
        {
            string columns = string.Join(',', columnNames);
            string valueParams = string.Join(',', columnNames.Select(x => $"@{x}").ToArray());
            string sql = $"insert into {tableName} ({columns}) values ({valueParams}) ;";
            return sql;
        }

        /// <summary>
        /// 產生Update的SQL指令
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        /// <param name="keyNames"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string GetUpdateSql(string tableName, string[] columnNames, string[] keyNames)
        {
            if (keyNames == null || keyNames.Length == 0) throw new ArgumentNullException(nameof(keyNames) + $" is required when calling GetUpdateSql() to avoid fault updating.");

            string setColumns = "";
            for (int i = 0; i < columnNames.Length; i++)
            {
                setColumns += $"{columnNames[i]}=@{columnNames[i]},";
            }
            setColumns = setColumns.TrimEnd(',');

            string whereScript = GetWhereScript(keyNames);
            string sql = $"update {tableName} set {setColumns} {whereScript} ;";
            return sql;
        }

        /// <summary>
        /// 產生Deleete的SQL指令
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keyNames"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string GetDeleteSql(string tableName, string[] keyNames)
        {
            if (keyNames == null || keyNames.Length == 0) throw new ArgumentNullException(nameof(keyNames) + $" is required when calling GetDeleteSql() to avoid fault deleting.");

            string whereScript = GetWhereScript(keyNames);
            string sql = $"delete from {tableName} {whereScript} ;";
            return sql;
        }

        /// <summary>
        /// 取得Where條件式
        /// </summary>
        /// <param name="whereKeyNames">where條件的欄位名稱</param>
        /// <param name="parameterNamePostFix">Sql參數名後綴(若單一筆資料，可略；若多筆資料，則需要後綴來避免參數名稱重複)</param>
        /// <param name="withWhereKeyword">是否包含where關鍵字(預設值:true)</param>
        /// <returns></returns>
        private string GetWhereScript(string[] whereKeyNames , string parameterNamePostFix = "", bool withWhereKeyword = true)
        {
            string whereScript = "";
            for (int i = 0; i < whereKeyNames.Length; i++)
            {
                if (whereScript != "") whereScript += " and ";
                whereScript += $"{whereKeyNames[i]}=@{whereKeyNames[i]}{parameterNamePostFix}";
            }

            if (whereScript != "" && withWhereKeyword) whereScript = $" where {whereScript} ";
            return whereScript;
        }

        /// <summary>
        /// 產生SqlParameter(帶值)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columnNames"></param>
        /// <param name="parameterNamePostFix">Sql參數名後綴(若單一筆資料，可略；若多筆資料，則需要後綴來避免參數名稱重複)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private SqlParameter[] GetSqlParameters(TDataModel model, string[] columnNames, string parameterNamePostFix="")
        {
            /*
             * 此函式適用於當參數欄位數 <= model的屬性欄位數時
             */
            SqlParameter[] parameters = new SqlParameter[columnNames.Length];
            for (int i = 0; i < columnNames.Length; i++)
            {
                var prop = _modelProperties.FirstOrDefault(p => p.Name == columnNames[i]);
                if (prop != null)
                {
                    var value = prop.GetValue(model) ?? (object)DBNull.Value;
                    parameters[i] = new SqlParameter($"@{columnNames[i]}{parameterNamePostFix}", value);

                    //if (value is Guid)
                    //{
                    //    var sqlParam = new SqlParameter($"@{columnNames[i]}{parameterNamePostFix}",SqlDbType.UniqueIdentifier);
                    //    sqlParam.Value = value.ToString();
                    //    parameters[i] = sqlParam;
                    //}
                    //else
                    //{
                        
                    //}
                }
                else
                {
                    throw new ArgumentException($"the column name [{columnNames[i]}] is not found in table [{_modelType.Name}]");
                }
            }
            return parameters;
        }

        /// <summary>
        /// 產生SqlParameter(帶值)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columnProperties">欄位屬性資訊</param>
        /// <param name="parameterNamePostFix">Sql參數名後綴(若單一筆資料，可略；若多筆資料，則需要後綴來避免參數名稱重複)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private SqlParameter[] GetSqlParameters<T>(T model, PropertyInfo[] columnProperties, string parameterNamePostFix = "")
        {
            /*
             * 此函式適用於當參數欄位數 == model的屬性欄位數時
             */
            SqlParameter[] parameters = new SqlParameter[columnProperties.Length];
            for (int i = 0; i < columnProperties.Length; i++)
            {
                var value = columnProperties[i].GetValue(model) ?? (object)DBNull.Value;
                parameters[i] = new SqlParameter($"@{columnProperties[i].Name}{parameterNamePostFix}", value);
            }
            return parameters;
        }

        /// <summary>
        /// 取得Db存取者資訊
        /// </summary>
        /// <returns></returns>
        protected IDbAccessUser GetUserInfo()
        {
            if (DbServiceConfig.DbAccessUserInfoGetter != null)
            {
                var userInfo = DbServiceConfig.DbAccessUserInfoGetter();
                if (userInfo != null)
                {
                    return userInfo;
                }
                else
                {
                    return new DbAccessUser();
                }
            }
            else
            {
                return new DbAccessUser();
            }
        }

        /// <summary>
        /// 取得更新前的資料集
        /// </summary>
        /// <param name="modelList">此次要更新的model list</param>
        /// <param name="keyNames">鍵值欄位陣列</param>
        /// <returns></returns>
        protected List<TDataModel> GetModelListBefore(List<TDataModel> modelList, string[] keyNames)
        {
            string sql = "";
            string sqlALL = "";
            string whereScript = "";
            List <SqlParameter> parametersALL = new List<SqlParameter>();
            int index = 0;
            foreach (var model in modelList)
            {
                index++;
                whereScript = GetWhereScript(keyNames, index.ToString());
                sql = $"select * from {DataTableName} {whereScript}";
                if (sqlALL.Length > 0) sqlALL += " union ";
                sqlALL += sql;
                var sqlParameters = GetSqlParameters(model, keyNames, index.ToString());
                parametersALL.AddRange(sqlParameters);
            }
            var dbCtx = GetDbContext();
            var dataList = dbCtx.Database.SqlQuery<TDataModel>(sqlALL, parametersALL.ToArray())?.ToList() ?? new List<TDataModel>();
            return dataList;
        }

        #endregion

    }
}

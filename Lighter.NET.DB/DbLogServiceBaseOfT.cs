using Lighter.NET.Common;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.SqlClient;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Lighter.NET.DB
{
    /// <summary>
    /// DB Log 記錄基礎函式(泛型)
    /// </summary>
    public abstract class DbLogServiceBase<TLogModel> : DbLogServiceBase, IDisposable
    {
        #region Static

        /// <summary>
        /// LogModel型別
        /// </summary>
        protected static Type _modelType;
        /// <summary>
        /// LogModel的「全部」欄位屬性資訊(不包含唯讀欄位)
        /// </summary>
        protected static PropertyInfo[] _modelProperties;

        static DbLogServiceBase()
        {
            _modelType = typeof(TLogModel);
            _modelProperties = _modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite == true)?.ToArray() ?? new PropertyInfo[0];

        }
        #endregion

        #region Constructor
        /// <summary>
        /// DB Log 記錄基礎函式
        /// </summary>
        public DbLogServiceBase()
        {
			/*此處先不產生dbcontext，待要執行sql操作時再產生，縮短佔用db connection的時間*/
			//_dbContext = NewDbContext();
		}

		#endregion

		#region Abstract Property
		/// <summary>
		/// 完整Log Table名稱(格式：[DbName].[Schema].[TableName]，範例： DbName.dbo.LogTableName)
		/// </summary>
		public abstract string LogTableName { get; protected set; }
        #endregion

        #region Insert

        /// <summary>
        /// 新增單筆
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Insert(TLogModel model)
        {
            if (model == null) throw new ArgumentNullException("[model] argument is null");

            int effectCount = Insert(new List<TLogModel>() { model });

            return effectCount;
        }

        /// <summary>
        /// 新增多筆
        /// </summary>
        /// <param name="modelList"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Insert(List<TLogModel> modelList)
        {
            if (modelList == null || modelList.Count == 0) throw new ArgumentNullException(nameof(modelList), $"[{nameof(modelList)}] argument is null or empty");

            var columnNames = GetTableColumnNames();
            int effectCount = DoInsert(modelList, columnNames);

            return effectCount;
        }


        /// <summary>
        /// 執行Insert動作
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="columnNames">指定要寫入資料的欄位</param>
        /// <returns></returns>
        private int DoInsert(List<TLogModel> modelList, string[] columnNames)
        {
            //自動附加[建立者/時間]欄位
            bool hasCreatorFields = _modelType.IsAssignableTo(typeof(IUpdater));

            var dbCtx = GetDbContext();
            var sql = GetInsertSql(LogTableName, columnNames);
            int effectCount = 0;

            foreach (var model in modelList)
            {
                if (hasCreatorFields)
                {
                    (model as ICreator)!.SetCreator();
                }

                var parameters = GetSqlParameters(model, columnNames);
                dbCtx.Database.ExecuteSqlCommand(sql, parameters);
                effectCount++;
            }

            //dispose db context
            DisposeDbContext();

            return effectCount;
        }
        #endregion

        #region Utils (Private/Protected)
  
        /// <summary>
        /// 取得db table的全部欄位名稱陣列
        /// </summary>
        /// <returns></returns>
        private string[] GetTableColumnNames()
        {
            return _modelProperties.Select(x => x.Name).ToArray(); ;
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
        /// 產生SqlParameter(帶值)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columnNames"></param>
        /// <param name="parameterNamePostFix">Sql參數名後綴(若單一筆資料，可略；若多筆資料，則需要後綴來避免參數名稱重複)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private SqlParameter[] GetSqlParameters(TLogModel model, string[] columnNames, string parameterNamePostFix = "")
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

        #endregion


    }
}

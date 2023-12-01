using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Lighter.NET.DB
{

    /// <summary>
    /// SQL where條件建構器
    /// </summary>
    public class WhereBuilder
    {
        /// <summary>
        /// where條件表示式
        /// </summary>
        protected string _whereExpr = "";
        /// <summary>
        /// where參數項和參數值
        /// </summary>
        protected Dictionary<string, object?> _whereParams = new Dictionary<string, object?>();
        /// <summary>
        /// 是否略過Null和空值參數(預設：true)
        /// </summary>
        protected bool _skipNullOrEmpty = true;
        /// <summary>
        /// 條件表示式(含WHERE關鍵字)
        /// </summary>
        public string WhereExpression
        {
            get
            {
                _whereExpr = _whereExpr.Trim();
                if(_whereExpr.StartsWith("where", StringComparison.OrdinalIgnoreCase))
                {
                    return _whereExpr;
                }
                else
                {
                    return $" WHERE {_whereExpr}";
                }
            }
        }
        /// <summary>
        /// 條件表示式(無WHERE關鍵字)
        /// </summary>
        public string ConditionExpression
        {
            get
            {
                _whereExpr = _whereExpr.Trim();
                if (_whereExpr.StartsWith("where", StringComparison.OrdinalIgnoreCase))
                {
                    return _whereExpr.Substring(5);
                }
                else
                {
                    return _whereExpr;
                }
            }
        }

        /// <summary>
        /// Where條件式中所用到的參數
        /// </summary>
        public SqlParameter[] WhereParameters
        {
            get
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                foreach (var item in _whereParams)
                {
                    object paramValue;
                    if (item.Value == null) 
                    {
                        paramValue = DBNull.Value; //null值處理
                    }
                    else
                    {
                        paramValue = item.Value;
                    }
                    parameters.Add(new SqlParameter(item.Key, paramValue));
                }
                return parameters.ToArray();
            }
        }
        /// <summary>
        /// SQL where條件建構器
        /// </summary>
        public WhereBuilder() { }
        /// <summary>
        /// SQL where條件建構器
        /// </summary>
        /// <param name="skipNullOrEmpty">是否略過Null或空值的參數項的條件</param>
        public WhereBuilder(bool skipNullOrEmpty) 
        {
            _skipNullOrEmpty = skipNullOrEmpty;
        }

        /// <summary>
        /// 第一個Where條件式
        /// </summary>
        /// <param name="fieldExpression">欄位表示式(必須包含比較運算子)</param>
        /// <param name="paramValue">參數值</param>
        /// <param name="skipNullOrEmpty">是否略過Null或空值的參數項的條件</param>
        /// <param name="paramName">參數名(預設採用「參數值」的變數名稱)</param>
        /// <returns></returns>
        public WhereBuilder WHERE(string fieldExpression, object? paramValue, bool skipNullOrEmpty = true, [CallerArgumentExpression("paramValue")] string paramName = "")
        {
            return AddCondition("WHERE", fieldExpression, paramValue, skipNullOrEmpty, paramName);
        }

        /// <summary>
        /// 串接條件式
        /// </summary>
        /// <param name="fieldExpression">欄位條件式</param>
        /// <returns></returns>
        public WhereBuilder Append(string fieldExpression)
        {
            fieldExpression = fieldExpression.Trim();

            _whereExpr += $" {fieldExpression} ";
            return this;
        }

        /// <summary>
        /// 加入參數項
        /// </summary>
        /// <param name="paramName">參數名</param>
        /// <param name="paramValue">參數值</param>
        /// <param name="replaceSameParamName">若相同的參數名稱已存在時，是否取代</param>
        /// <returns></returns>
        public WhereBuilder AddParameter(string paramName, object? paramValue, bool replaceSameParamName = false) 
        {
            if (!_whereParams.ContainsKey(paramName)) { 
                _whereParams.Add(paramName, paramValue);
            }
            else
            {
                if (replaceSameParamName)
                {
                    _whereParams[paramName] = paramValue;
                }
            }
            return this;    
        }

        /// <summary>
        /// AND條件式
        /// </summary>
        /// <param name="fieldExpression">欄位表示式(必須包含比較運算子)</param>
        /// <param name="paramValue">參數值</param>
        /// <param name="skipNullOrEmpty">是否略過Null或空值的參數項的條件</param>
        /// <param name="paramName">參數名(預設採用「參數值」的變數名稱)</param>
        /// <returns></returns>
        public WhereBuilder AND(string fieldExpression, object? paramValue, bool skipNullOrEmpty = true, [CallerArgumentExpression("paramValue")] string paramName = "")
        {
            string logicOperator = (_whereExpr == "") ? "" : "AND";
            return AddCondition(logicOperator, fieldExpression, paramValue, skipNullOrEmpty, paramName);
        }

        /// <summary>
        /// OR條件式
        /// </summary>
        /// <param name="fieldExpression">欄位表示式(必須包含比較運算子)</param>
        /// <param name="paramValue">參數值</param>
        /// <param name="skipNullOrEmpty">是否略過Null或空值的參數項的條件</param>
        /// <param name="paramName">參數名(預設採用「參數值」的變數名稱)</param>
        /// <returns></returns>
        public WhereBuilder OR(string fieldExpression, object? paramValue, bool skipNullOrEmpty = true, [CallerArgumentExpression("paramValue")] string paramName = "")
        {
            string logicOperator = (_whereExpr == "") ? "" : "OR";
            return AddCondition(logicOperator, fieldExpression, paramValue, skipNullOrEmpty, paramName);
        }

        /// <summary>
        /// 加入條件
        /// </summary>
        /// <param name="logicOperator">邏輯運算子(and , or ,或沒有)</param>
        /// <param name="fieldExpression">欄位表示式(必須包含比較運算子)</param>
        /// <param name="paramValue">參數值</param>
        /// <param name="skipNullOrEmpty">是否略過Null或空值的參數項的條件</param>
        /// <param name="paramName">參數名(預設採用「參數值」的變數名稱)</param>
        /// <returns></returns>
        private WhereBuilder AddCondition(string logicOperator,string fieldExpression, object? paramValue, bool skipNullOrEmpty , string paramName)
        {
            string[] paramNames = paramName.Split('.');
            if (paramNames.Length > 1) paramName = paramNames[paramNames.Length - 1];
            if (_skipNullOrEmpty || skipNullOrEmpty)
            {
                if (paramValue == null) return this;
                Type paramType = paramValue.GetType();
                Type? nullableType = Nullable.GetUnderlyingType(paramType);
                bool isNullable = (nullableType != null);
                if ((paramType.IsClass || isNullable) && paramValue == null)
                {
                    //略過此條件
                    return this;
                }

                if((paramType == typeof(string) || nullableType == typeof(string)) && paramValue.ToString()=="")
                {
                    //略過此條件
                    return this;
                }
            }

            fieldExpression = fieldExpression.Trim();
            string compareOperator = fieldExpression.Substring(fieldExpression.Length - 1, 1);
            string[] possibleOperators = {"=", ">", "<"};
            bool containsOperator = possibleOperators.Contains(compareOperator) || fieldExpression.EndsWith("like", StringComparison.OrdinalIgnoreCase);
            if(containsOperator)
            {
                //有包含比較運算子
                _whereExpr += $" {logicOperator} {fieldExpression} @{paramName} ";
            }
            else
            {
                //無包含比較運算子(預設用=進行比較)
                _whereExpr += $" {logicOperator} {fieldExpression} =@{paramName} ";
            }
            
            if (!_whereParams.ContainsKey(paramName))
            {
                _whereParams.Add(paramName, paramValue);
            }
            return this;
        }
    }

}

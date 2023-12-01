using Lighter.NET.Common;
using Lighter.NET.Helpers;
using System.Linq.Expressions;

namespace Lighter.NET.DB
{
    /// <summary>
    /// 語系欄位對應延伸函式
    /// </summary>
    public static class CultureMappingColumnExtension
    {
        /// <summary>
        /// 對應出目前語系所用欄位
        /// </summary>
        /// <param name="mappingConfig">欄位對應組態</param>
        /// <param name="columnName">要「被對應」的欄位名稱</param>
        /// <returns></returns>
        public static string Map(this List<CultureMappingColumn>? mappingConfig, string columnName)
        {
            if (mappingConfig == null) return columnName;
            var cultureName = LangHelper.GetCultureName();
            var found = mappingConfig.FirstOrDefault(x=> x.ColumnName == columnName &&  x.CultureName == cultureName);
            if (found != null)
            {
                return found.MapToColumnName;
            }
            else
            {
                return columnName;
            }
        }

        /// <summary>
        /// 加入語系對應欄位
        /// </summary>
        /// <typeparam name="TDataModel">資料表的DataModel</typeparam>
        /// <param name="mappingConfig">語系對應組態</param>
        /// <param name="columnSelector">要「被對應」的欄位選擇器</param>
        /// <param name="cultureName">語系列舉值</param>
        /// <param name="mapTocolumnSelector">對應後的欄位選擇器</param>
        public static List<CultureMappingColumn> AddMappingColumn<TDataModel>(this List<CultureMappingColumn>? mappingConfig, Expression<Func<TDataModel, object?>> columnSelector, CultureName cultureName, Expression<Func<TDataModel, object?>> mapTocolumnSelector)
        {
            if (mappingConfig == null) mappingConfig = new List<CultureMappingColumn>();
            string columnName = columnSelector.GetLambdaPropertyName();
            string mapToColumnName = mapTocolumnSelector.GetLambdaPropertyName();
            mappingConfig.Add(new CultureMappingColumn(columnName,cultureName,mapToColumnName));
            return mappingConfig;
        }
    }
}

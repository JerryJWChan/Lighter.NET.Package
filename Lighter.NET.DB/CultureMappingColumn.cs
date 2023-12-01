using System.Linq.Expressions;
using Lighter.NET.Common;
namespace Lighter.NET.DB
{
    /// <summary>
    /// 語系對應欄位
    /// </summary>
    public class CultureMappingColumn
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string ColumnName { get; set; } = "";
        /// <summary>
        /// 語系名稱
        /// </summary>
        public CultureName CultureName { get; set; } = CultureName.Current;
        /// <summary>
        /// 對應後的欄位名稱
        /// </summary>
        public string MapToColumnName { get; set; } = "";
        /// <summary>
        /// 欄位語系對應
        /// </summary>
        /// <param name="columnName">欄位名稱</param>
        /// <param name="cultureName">語系名稱</param>
        /// <param name="mapToColumnName">對應後的欄位名稱</param>
        public CultureMappingColumn(string columnName, CultureName cultureName, string mapToColumnName) 
        {
            ColumnName = columnName;
            CultureName= cultureName;
            MapToColumnName = mapToColumnName;
        }
    }

}

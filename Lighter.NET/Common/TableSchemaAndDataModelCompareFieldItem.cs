namespace Lighter.NET.Common
{
    /// <summary>
    /// Table Schema和DataModel欄位比較的欄位項目
    /// </summary>
    public class TableSchemaAndDataModelCompareFieldItem
    {
        /// <summary>
        /// Table欄位名
        /// </summary>
        public string? TableColumnName { get; set; }
        /// <summary>
        /// Table欄位資料型別
        /// </summary>
        public string? TableColumnDataType { get; set; }
        /// <summary>
        /// Table欄位是否可null
        /// </summary>
        public bool TableColumnNullable { get; set; }
        /// <summary>
        /// DataModel欄位名
        /// </summary>
        public string? ModelFieldName { get; set; }
        /// <summary>
        /// DataModel欄位型別
        /// </summary>
        public string? ModelFieldDataType { get; set; }
        /// <summary>
        /// DataModel欄位是否可Null
        /// </summary>
        public bool ModelFieldNullable { get; set; }
    }
}

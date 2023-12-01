namespace Lighter.NET.DB
{
    /// <summary>
    /// Table Column Schema Model
    /// </summary>
    public class TableColumnSchemaModel
    {
        /// <summary>
        /// 欄位名
        /// </summary>
        public string ColumnName { get; set; } = "";
        /// <summary>
        /// 資料型別
        /// </summary>
        public string DataType { get; set; } = "";
        /// <summary>
        /// 資料型別最大長度(byte)，-1:表示MAX(不限)
        /// </summary>
        public Int16 MaxLength { get; set; } = 0;
        /// <summary>
        /// 數字或日期有效位數
        /// </summary>
        public byte Precision { get; set; } = 0;
        /// <summary>
        /// 是否可Null
        /// </summary>
        public bool IsNullable { get; set; } = true;
        /// <summary>
        /// 是否主鍵
        /// </summary>
        public bool IsPrimaryKey { get; set; } = false;
        /// <summary>
        /// 欄位說明
        /// </summary>
        public string? Description { get; set; }
    }
}

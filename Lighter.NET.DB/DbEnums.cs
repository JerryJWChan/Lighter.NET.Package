namespace Lighter.NET.DB
{
    /// <summary>
    /// 排序方式
    /// </summary>
    public enum OrderByType
    {
        /// <summary>
        /// ascendant order
        /// </summary>
        ASC,
        /// <summary>
        /// descendant order
        /// </summary>
        DESC
    }

    /// <summary>
    /// Db Log分類
    /// </summary>
    public enum DbLogType
    {
        /// <summary>
        /// 未定義
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// 新增
        /// </summary>
        Insert = 1,
        /// <summary>
        /// 更新
        /// </summary>
        Update = 2,
        /// <summary>
        /// 刪除
        /// </summary>
        Delete = 3,
        /// <summary>
        /// 匯入
        /// </summary>
        Import = 4,
        /// <summary>
        /// 匯出/下載
        /// </summary>
        Export = 5,
        /// <summary>
        /// 查詢
        /// </summary>
        Select =6
    }
}

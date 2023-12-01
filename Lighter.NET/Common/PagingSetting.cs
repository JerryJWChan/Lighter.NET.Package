namespace Lighter.NET.Common
{
    /// <summary>
    /// 資料分頁設定
    /// </summary>
    public class PagingSetting
    {
        /// <summary>
        /// 預設每個資料分頁的列數
        /// </summary>
        public static int DefalutRowsPerPage { get; set; } = 20;
        /// <summary>
        /// 全部列數
        /// </summary>
        public int TotalRowCount { get; set; }
        /// <summary>
        /// 每頁列數(必須大於0)
        /// </summary>
        public int RowsPerPage { get; set; } = DefalutRowsPerPage;
        /// <summary>
        /// 目前第幾頁(必須大於0)
        /// </summary>
        public int PageNo { get; set; } = 1;
        /// <summary>
        /// 當前資料頁的首列編號
        /// </summary>
        public int StartRowNo { get; set; }
        /// <summary>
        /// 當前資料頁的末列編號
        /// </summary>
        public int EndRowNo { get; set; }
        /// <summary>
        /// 頁數
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 強制更新總資料筆數(每次查詢時是否必須更新TotalRowCount)
        /// (若查詢條件相同，且資料本身性質屬於更新速度不快的情況下，設成false可提高查詢效能)
        /// </summary>
        public bool ForceRefreshTotalRowCount { get; set; } = false;
        public PagingSetting() { }
        public PagingSetting(int rowsPerPage) 
        {
            RowsPerPage = rowsPerPage;
        }
        /// <summary>
        /// 檢核設定(條件：RowsPerPage > 0 and PageNo > 0)
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return RowsPerPage > 0 && PageNo > 0;
        }

    }
}

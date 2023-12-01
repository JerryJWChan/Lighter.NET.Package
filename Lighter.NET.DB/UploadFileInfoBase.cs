namespace Lighter.NET.DB
{
    /// <summary>
    /// 檔案描述資訊記錄基礎類別
    /// </summary>
    public class UploadFileInfoBase
    {
        /// <summary>
        /// 記錄參照id(歸屬記錄的識別id)
        /// </summary>
        public string ref_id { get; set; } = "";
        /// <summary>
        /// 檔案的唯一識別編號(例如：GUID)
        /// </summary>
        public string f_id { get; set; } = "";
        /// <summary>
        /// 檔案版本號(數字流水號)，預設1
        /// </summary>
        public int ver { get; set; } = 1;
        /// <summary>
        /// 檔案分類
        /// </summary>
        public string kind { get; set; } = "";
        /// <summary>
        /// 檔案是否包含個資(1:是，0:否)
        /// </summary>
        public int has_privacy { get; set; } = 0;
        /// <summary>
        /// 檔案標題
        /// </summary>
        public string title { get; set; } = "";
        /// <summary>
        /// 檔名(原始檔名)
        /// </summary>
        public string fname_orig { get; set; } = "";
        /// <summary>
        /// 檔名(新)(資安NOTE：轉成不易識讀的檔名，例如GUID)
        /// 若f_id是用GUID，則可與file_id相同
        /// </summary>
        public string fname { get; set; } = "";
        /// <summary>
        /// 檔案類型(副檔名)
        /// </summary>
        public string ext { get; set; } = "";
        /// <summary>
        /// 檔案大小(byte)
        /// </summary>
        public long size { get; set; } = 0;
        /// <summary>
        /// 網址
        /// </summary>
        public string url { get; set; } = "";
        /// <summary>
        /// (頟外的)檔案描述資料([{key1:value1},{key2,value2},...])，格式：JSON
        /// </summary>
        public List<KeyValuePair<string,string>> metadata { get; set; } = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// 檔案排序
        /// </summary>
        public int ord { get; set; } = 0;

    }
}

namespace Lighter.NET.DB
{
    /// <summary>
    /// 檔案描述資訊記錄
    /// </summary>
    public class UploadFileInfoModel : UploadFileInfoBase, ICreator,IUpdater
    {
        /// <summary>
        /// 檔案存放路徑(資安NOTE：此欄位勿傳至前端)
        /// </summary>
        public string? f_path { get; set; }
        /// <summary>
        /// the user name of id of whom upload the file
        /// </summary>
        public string createBy { get; set; } = "";
        /// <summary>
        /// the file uploading time
        /// </summary>
        public DateTime createAt { get; set; }
        /// <summary>
        /// the source ip address of the file uploading
        /// </summary>
        public string? createIp { get; set; }
        /// <summary>
        /// the user name of id of whom re-upload the file
        /// </summary>
        public string? updateBy { get;set; }
        /// <summary>
        /// the file uploading time
        /// </summary>
        public DateTime? updateAt { get;set; }
        /// <summary>
        /// the source ip address of the file uploading
        /// </summary>
        public string? updateIp { get;set; }
    }

    /// <summary>
    /// 檔案描述資訊記錄(簡化欄位)(資安NOTE:減少不必要欄位傳到前端去)
    /// </summary>
    public class UploadFileInfoSimpleModel : UploadFileInfoBase 
    {
        //no more here
    }
}

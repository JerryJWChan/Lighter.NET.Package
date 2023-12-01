using System.ComponentModel.DataAnnotations;

namespace Lighter.NET.DB
{
    /// <summary>
    /// 資料異動Log table model
    /// </summary>
    public class DbLogModel : IDbLog
    {
        /// <summary>
        /// 順序號(對應到DB的自動遞增數字，設定成non-cluster index)
        /// </summary>
        public int serial_num { get; }
        /// <summary>
        /// Guid key識別號(DB中此欄位要設定成primary key + non-cluster index)
        /// </summary>
        [Key]
        public System.Guid log_guid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// log分類(對應到LogType enum)
        /// </summary>
        public int log_type { get;set; }
        /// <summary>
        /// log來源系統名稱(若log到外部Db時要帶入此欄位值)
        /// </summary>
        public string? source_app_code { get; set; }
        /// <summary>
        /// log來源功能名稱
        /// </summary>
        public string? source_func_name { get; set; }
        /// <summary>
        /// Table名稱(格式：[DbName].[Schema].[TableName]，範例： DbName.dbo.TableName)
        /// </summary>
        public string? table_name { get; set; }
        /// <summary>
        /// 對應原記錄的primary key值(一律轉成字串)
        /// </summary>
        public string? data_key { get; set; }
        /// <summary>
        /// 若原記錄的primary key是複合鍵，則將第2組key值記錄於此欄位(一律轉成字串)，多欄位鍵值以逗號分隔
        /// </summary>
        public string? data_key2 { get; set; }
        /// <summary>
        /// 異動前data model的json
        /// </summary>
        public string? data_bf_json { get; set; }
        /// <summary>
        /// 異動後data model的json
        /// </summary>
        public string? data_af_json { get; set; }
        /// <summary>
        /// 備註/說明
        /// </summary>
        public string? memo { get; set; }
        /// <summary>
        /// the user id or name of the user writing the log record
        /// </summary>
        public string? updateBy { get;set; }
        /// <summary>
        /// the datetime of the log record
        /// </summary>
        public DateTime? updateAt { get;set; }
        /// <summary>
        /// the source ip address of the request causing the log record
        /// </summary>
        public string? updateIp { get;set; }
    }
}

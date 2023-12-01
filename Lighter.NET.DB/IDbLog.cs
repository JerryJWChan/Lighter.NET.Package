namespace Lighter.NET.DB
{
    /// <summary>
    /// Db table異動log的欄位介面定義
    /// </summary>
    public interface IDbLog:IUpdater
    {
        /// <summary>
        /// log序號(唯讀欄位，對應到DB的自動遞增數字，且要設定成cluster index)
        /// </summary>
        int serial_num { get; }
        /// <summary>
        /// Guid key識別號(DB中此欄位要設定成primary key + non-cluster index)
        /// </summary>
        System.Guid log_guid { get; set; }
        /// <summary>
        /// log分類(對應到LogType enum)
        /// </summary>
        int log_type { get; set; }
        /// <summary>
        /// log來源系統代號(若log到外部Db時要帶入此欄位值)
        /// </summary>
        string? source_app_code { get; set; }
        /// <summary>
        /// log來源功能名稱
        /// </summary>
        string? source_func_name { get; set; }
        /// <summary>
        /// Table名稱(格式：[DbName].[Schema].[TableName]，範例： DbName.dbo.TableName)
        /// </summary>
        string? table_name { get; set; }
        /// <summary>
        /// 對應原記錄的primary key值(一律轉成字串)
        /// </summary>
        string? data_key { get; set; }
        /// <summary>
        /// 若原記錄的primary key是複合鍵，則將第2組key值記錄於此欄位(一律轉成字串)
        /// </summary>
        string? data_key2 { get; set; }
        /// <summary>
        /// 異動前data model的json
        /// </summary>
        string? data_bf_json { get;set; }
        /// <summary>
        /// 異動後data model的json
        /// </summary>
        string? data_af_json { get;set; }
        /// <summary>
        /// 備註/說明
        /// </summary>
        string? memo { get;set; }

    }
}

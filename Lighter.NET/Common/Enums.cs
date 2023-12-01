namespace Lighter.NET.Common
{
    /// <summary>
    /// 資料賦值模式
    /// </summary>
    public enum TypeAssignMode
    {
        /// <summary>
        /// 嚴格型別(型別完全相同才可進行賦值(x = y)轉換)
        /// </summary>
        StrictType,
        /// <summary>
        /// 鬆散型別(型別不必相同，若可自動轉換(例如：int to double)，則進行賦值(x = y)轉換)
        /// </summary>
        LooseType
    }
    /// <summary>
    /// 日期格式列舉(日期_表示-，時間_表示:)
    /// </summary>
    public enum FormatEnum
    {
        #region 日期格式
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        [EnumStringValue("yyyy-MM-dd")]
        yyyy_MM_dd,
        /// <summary>
        /// yyyy-MM-dd HH:mm
        /// </summary>
        [EnumStringValue("yyyy-MM-dd HH:mm")]
        yyyy_MM_dd_HH_mm,
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        [EnumStringValue("yyyy-MM-dd HH:mm:ss")]
        yyyy_MM_dd_HH_mm_ss,
        /// <summary>
        /// yyyy-MM-ddTHH:mm
        /// </summary>
        [EnumStringValue("yyyy-MM-ddTHH:mm")]
        yyyy_MM_ddTHH_mm,
        /// <summary>
        /// yyyy-MM-ddTHH:mm:ss
        /// </summary>
        [EnumStringValue("yyyy-MM-ddTHH:mm:ss")]
        yyyy_MM_ddTHH_mm_ss,
        /// <summary>
        /// HH:mm
        /// </summary>
        [EnumStringValue("HH:mm")]
        HH_mm,
        /// <summary>
        /// HH:mm:ss
        /// </summary>
        [EnumStringValue("HH:mm:ss")]
        HH_mm_ss,
        /// <summary>
        /// yyyyMMdd
        /// </summary>
        [EnumStringValue("yyyyMMdd")]
        yyyyMMdd,
        /// <summary>
        /// yyyyMMddHHmm
        /// </summary>
        [EnumStringValue("yyyyMMddHHmm")]
        yyyyMMddHHmm,
        /// <summary>
        /// yyyyMMddHHmmss
        /// </summary>
        [EnumStringValue("yyyyMMddHHmmss")]
        yyyyMMddHHmmss
        #endregion

        #region 數字格式

        #endregion

        #region 貨幣格式

        #endregion
    }

    /// <summary>
    /// 訊息種類列舉
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 資訊
        /// </summary>
        Info=0,
        /// <summary>
        /// 警告
        /// </summary>
        Warning=1,
        /// <summary>
        /// 錯誤
        /// </summary>
        Error=2,
        /// <summary>
        /// 確認
        /// </summary>
        Confirm=3,
		/// <summary>
		/// 偵錯
		/// </summary>
		Debug=4
	}

    /// <summary>
    /// 檔案大小計算單位
    /// </summary>
    public enum FileSizeUnit
    {
        Byte = 1,
        KB = 1024,
        MB = 1024*1024,
        GB = 1024*1024*1024
    }

    /// <summary>
    /// 捲軸方向
    /// </summary>
    public enum ScrollDirection
    {
        Undefined = 0,
        None,
        Horizontal,
        Vertical,
        Both
    }

    /// <summary>
    /// 連結目標視窗類別
    /// </summary>
    public enum HyperLinkTargetType
    {
        /// <summary>
        /// 新視窗
        /// </summary>
        [EnumStringValue("_blank")]
        Blank,
        /// <summary>
        /// 原視窗
        /// </summary>
        [EnumStringValue("_self")]
        Self,
        /// <summary>
        /// 上層視窗
        /// </summary>
        [EnumStringValue("_parent")]
        Parent,
        /// <summary>
        /// 頂層視窗
        /// </summary>
        [EnumStringValue("_top")]
        Top
    }

    /// <summary>
    /// Json反序列化空物件"{}"時之行為規則
    /// </summary>
    public enum JsonDeserialzeEmptyObjectBehavior
    {
        /// <summary>
        /// 回傳null或class default
        /// </summary>
        ReturnNullOrDefault,
        /// <summary>
        /// 依照JsonProvider內建的行為
        /// </summary>
        UseJsonProviderBuildInBehavior
    }

    /// <summary>
    /// 語系名稱列舉
    /// </summary>
    public enum CultureName
    {
        /// <summary>
        /// 現用語系
        /// </summary>
        Current = 0,
        /// <summary>
        /// 中文(繁體)
        /// </summary>
        CT,
        /// <summary>
        /// 中文(簡體)
        /// </summary>
        CS,
        /// <summary>
        /// 英文
        /// </summary>
        EN
    }

    /// <summary>
    /// 版面放置區域
    /// </summary>
    public enum Placement
    {
        /// <summary>
        /// 預設
        /// </summary>
        Default = 0,
        /// <summary>
        /// 置於上方
        /// </summary>
        Top,
        /// <summary>
        /// 置於下方
        /// </summary>
        Bottom,
        /// <summary>
        /// 置於左方
        /// </summary>
        Left,
        /// <summary>
        /// 置於右方
        /// </summary>
        Right,
        /// <summary>
        /// 置於中間
        /// </summary>
        Center
    }

    /// <summary>
    /// Css Class 的siz-x表式示列舉(x表列舉項目小寫)
    /// </summary>
    public enum CssClassSize
    {
        Undefined = 0,
        XS,
        S,
        M,
        L,
        XL,
        XXL,
        XXL2,
        XXL3,
        XXL4,
        XXL5,
        XXL6
    }

    /// <summary>
    /// Http request method
    /// </summary>
    public enum HttpRequestMethod
    {
        Undefined=0,
        GET,
        POST,
        PUT,
        DELETE
    }

    /// <summary>
    /// 按鈕種類
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// 一般按鈕
        /// </summary>
        Button,
        /// <summary>
        /// 表單送出按鈕
        /// </summary>
        Submit,
        /// <summary>
        /// 連結按鈕
        /// </summary>
        LinkButton,
        /// <summary>
        /// 重置按鈕
        /// </summary>
        Reset
    }

	/// <summary>
	/// 證件號碼種類
	/// </summary>
	public enum CIDType
	{
		/// <summary>
		/// 身份證號
		/// </summary>
		PID = 0,
		/// <summary>
		/// 公司統一編號
		/// </summary>
		UBN,
		/// <summary>
		/// 身份證號 or 公司統一編號
		/// </summary>
		Both_PID_UBN
	}

    /// <summary>
    /// Log記錄分級
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 足跡
        /// </summary>
        Trace=0,
        /// <summary>
        /// 除錯
        /// </summary>
        Debug=1,
        /// <summary>
        /// 資訊
        /// </summary>
        Info=2,
        /// <summary>
        /// 警告
        /// </summary>
        Warning=3,
        /// <summary>
        /// 錯誤
        /// </summary>
        Error=4,
        /// <summary>
        /// 災害
        /// </summary>
        Fatal=5
    }
}

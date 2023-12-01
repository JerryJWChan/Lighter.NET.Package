namespace Lighter.NET.Common
{
	/// <summary>
	/// 列舉項目轉成下拉選項時的可調參數
	/// </summary>
	public class EnumConvertToOptionsArgs
	{
		/// <summary>
		/// 是否排除Undefined或「未定義」的項目(預設:false)
		/// </summary>
		public bool excludeUndefinedItem { get; set; } = false;
		/// <summary>
		/// 使用StringValueArritbute(文字值)的值作為選項文字(預設:false)
		/// </summary>
		public bool useStringValueAttritbuteAsText { get; set; } = false;
		public bool useStringValueAttributeAsValue { get; set; } = false;
		/// <summary>
		/// 使用LangAttribute(多語系文字)的值作為選項文字(預設:false)
		/// </summary>
		public bool useLangAttributeAsText { get; set; } = false;
		/// <summary>
		/// 使用LangArritbute(多語系文字)的值作為額外選項資料項值(預設:false)
		/// </summary>
		public bool useLangAttributeAsExtraData { get; set; } = false;
		/// <summary>
		/// 額外選項資料項值變數名稱(預設：extraData)
		/// </summary>
		public string extraDataName { get; set; } = "extraData";
	}
}

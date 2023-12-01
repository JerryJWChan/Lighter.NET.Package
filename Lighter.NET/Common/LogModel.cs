using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lighter.NET.Common
{
	/// <summary>
	///Log記錄Model
	/// </summary>
	public class LogModel : IMetaModel
	{
		/// <summary>
		/// 記錄時間
		/// </summary>
		public DateTime LogTime { get; set; } = DateTime.Now;
		/// <summary>
		/// 記錄分級(對應到LogLevel列舉)
		/// </summary>
		public int Level { get; set; }
		/// <summary>
		/// Logger Name
		/// </summary>
		public string Logger { get; set; } = "";
		/// <summary>
		/// 應用程式編號
		/// </summary>
		public string? AppCode { get; set; }

		/// <summary>
		/// 功能名稱
		/// </summary>
		public string? FunctionName { get; set; }

		/// <summary>
		/// 錯誤代碼
		/// </summary>
		public string? ErrorCode { get; set; }

		/// <summary>
		/// 簡短錯誤訊息(NOTE: 正式機環境時只可顯示簡短訊息，不可暴露系統敏感資訊)
		/// </summary>
		public string SimpleMessage { get; set; } = "";
		/// <summary>
		/// 完整訊息
		/// </summary>
		public string Message { get; set; } = "";

		/// <summary>
		/// Exception 的文字表達式
		/// </summary>
		public string Exception { get; set; } = "";

		/// <summary>
		/// 呼叫logger的函式完整名稱
		/// </summary>
		public string CallSite { get; set; } = "";

		/// <summary>
		/// 登入者帳號
		/// </summary>
		public string UserId { get; set; } = "";

		/// <summary>
		/// Client IP位址
		/// </summary>
		public string ClientIp { get; set; } = "";

		/// <summary>
		/// 錯誤發生時之狀態資料(例如各項參數)Json
		/// </summary>
		public string StateData { get; set; } = "";
		
	}
}

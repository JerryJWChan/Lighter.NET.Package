using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lighter.NET.Common
{
	/// <summary>
	/// Http Request錯誤記錄Model
	/// </summary>
	public class HttpRequestErrorLogModel: LogModel
	{
		/// <summary>
		/// 請求編號
		/// </summary>
		public string? RequestId { get; set; }

		/// <summary>
		/// Session ID
		/// </summary>
		public string? SessionID { get; set; }

		/// <summary>
		/// Controller Name
		/// </summary>
		public string? Controller { get; set; }

		/// <summary>
		/// Action Name
		/// </summary>
		public string? Action { get; set; }

		/// <summary>
		/// Http Method
		/// </summary>
		public string? RequestMethod { get; set; }

		/// <summary>
		/// Http狀態碼
		/// </summary>
		public int? StatusCode { get; set; }

		/// <summary>
		/// 請求網址
		/// </summary>
		public string? RequestUrl { get; set; }

		/// <summary>
		/// 原始網址(轉址前)
		/// </summary>
		public string? OriginalUrl { get; set; }

		/// <summary>
		/// Post Data or Json Data
		/// </summary>
		public string? PostData { get; set; }

		/// <summary>
		/// User瀏覽器資訊
		/// </summary>
		public string? UserAgent { get; set; }

		/// <summary>
		/// Cookie資料
		/// </summary>
		public string? Cookies { get; set;}

	}
}

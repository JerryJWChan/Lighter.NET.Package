using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lighter.NET.Common
{
	/// <summary>
	/// 帶有狀態的Model
	/// </summary>
	public class StatefulModel<TModel>
	{
		/// <summary>
		/// 暫時性Id(用於對應Client編輯過程中的每個表格列，因為新增列時，還沒有可識別的Id，故需先配給一個TempId)
		/// </summary>
		public string TempId { get; set; } = "";
		/// <summary>
		/// Model的狀態(例如：original, added, updated, deleted)
		/// </summary>
		public string State { get; set; } = "initial";
		/// <summary>
		/// data model
		/// </summary>
		public TModel Model { get; set; }

		/// <summary>
		/// 帶有狀態的Model
		/// </summary>
		public StatefulModel()
		{
		}

		/// <summary>
		/// 帶有狀態的Model
		/// </summary>
		/// <param name="initialState">初始狀態</param>
		public StatefulModel(string initialState)
		{
			State = initialState;
		}

		/// <summary>
		/// 帶有狀態的Model
		/// </summary>
		/// <param name="initialState">初始狀態</param>
		/// <param name="model">model</param>
		public StatefulModel(string initialState, TModel model)
		{
			State = initialState;
			Model = model;
		}
	}
}

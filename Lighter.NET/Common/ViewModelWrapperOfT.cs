using System.Linq.Expressions;

namespace Lighter.NET.Common
{
	public class ViewModelWrapper<TModel>: ViewModelWrapper where TModel : class,new()
	{
		public ViewModelWrapper()
		{
			AddModel(new TModel());
		}

		public ViewModelWrapper(TModel model)
		{
			AddModel(model);
		}

		/// <summary>
		/// 加入選項清單
		/// </summary>
		/// <typeparam name="TModel">要用到此選項清單的DataModel type</typeparam>
		/// <param name="columSelector">要對應的欄位選擇器</param>
		/// <param name="optionList">選項清單</param>
		/// <returns></returns>
		public ViewModelWrapper AddOptions(Expression<Func<TModel, object?>> columnSelector, List<OptionItem> optionList)
		{
			var columnName = columnSelector.GetLambdaPropertyName();
			string key = typeof(TModel).ToString() + columnName;
			_Add(optionList, key);
			return this;
		}


		/// <summary>
		/// 取得選項清單
		/// </summary>
		/// <typeparam name="TModel">要用到此選項清單的DataModel type</typeparam>
		/// <param name="columnSelector">要對應的欄位選擇器</param>
		/// <returns></returns>
		public List<OptionItem> GetOptions(Expression<Func<TModel, object?>> columnSelector)
		{
			var columnName = columnSelector.GetLambdaPropertyName();
			string key = typeof(TModel).ToString() + columnName;
			var optionList = GetModel<List<OptionItem>>(key);
			if (optionList == null) optionList = new List<OptionItem>();
			return optionList;
		}

		/// <summary>
		/// 加入檢核錯誤
		/// </summary>
		/// <param name="columnSelector">欄位選擇子</param>
		/// <param name="message"></param>
		public void AddModelError(Expression<Func<TModel, object?>> columnSelector,string message)
		{
            var columnName = columnSelector.GetLambdaPropertyName();
			AddModelError(columnName, message);
        }
	}
}

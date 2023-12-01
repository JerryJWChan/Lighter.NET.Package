namespace Lighter.NET.Common
{
    /// <summary>
    /// Model檢核器介面
    /// </summary>
    /// <typeparam name="TModel">受檢核的Model型別</typeparam>
    public interface IModelValidator<TModel>
    {
        /// <summary>
        /// 判斷是否檢核通過
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool IsValid(TModel model);
        /// <summary>
        /// 檢核錯誤清單
        /// </summary>
        List<ModelError> Errors { get; }
        /// <summary>
        /// 檢核動作(在此方法中執行檢核動作，並將任何錯誤用AddError()方法，加入錯誤清單)
        /// </summary>
        Action<TModel?> Validate { get; }
    }
}

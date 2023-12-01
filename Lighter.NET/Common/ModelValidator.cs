using Microsoft.Extensions.Localization;
using System.Linq.Expressions;
using System.Reflection;

namespace Lighter.NET.Common
{
    /// <summary>
    /// 檢核器抽象類別
    /// </summary>
    /// <typeparam name="TModel">被檢核的model型別</typeparam>
    public abstract class ModelValidator<TModel> : IModelValidator<TModel>
    {
        protected PropertyInfo[]? RequiredProperties { get; set; }
        public List<ModelError> Errors { get; } = new List<ModelError>();
        /// <summary>
        /// Controller的語系轉換器
        /// </summary>
        public IStringLocalizer _cTexts { get; set; }
        /// <summary>
        /// Validator的語系轉換器
        /// </summary>
        public IStringLocalizer _vTexts { get; set; }
        /// <summary>
        /// Model的語系轉換器
        /// </summary>
        public IStringLocalizer _mTexts { get; set; }
        public ModelValidator()
        {
            var defaultLocalizer = LangHelper.GetLocalizer<DefaultResource>();
            _cTexts = defaultLocalizer;
            _vTexts = defaultLocalizer;
            _mTexts = defaultLocalizer;
        }

        public abstract Action<TModel?> Validate { get; }
        protected void AddError(string propertyName, string message)
        {
            Errors.Add(new ModelError()
            {
                PropertyName = propertyName,
                Message = message
            });
        }
        /// <summary>
        /// 設定Controller的語系轉換器
        /// </summary>
        /// <param name="cTexts"></param>
        /// <returns></returns>
        public ModelValidator<TModel> SetControllerLocalizer(IStringLocalizer cTexts)
        {
            _cTexts = cTexts;
            return this;
        }
        /// <summary>
        /// 設定Validator的語系轉換器
        /// </summary>
        /// <param name="vTexts"></param>
        /// <returns></returns>
        public ModelValidator<TModel> SetValidatorLocalizer(IStringLocalizer vTexts)
        {
            _vTexts = vTexts;
            return this;
        }
        /// <summary>
        /// 設定Model的語系轉換器
        /// </summary>
        /// <param name="mTexts"></param>
        /// <returns></returns>
        public ModelValidator<TModel> SetModelLocalizer(IStringLocalizer mTexts)
        {
            _mTexts = mTexts;
            return this;
        }
        public bool IsValid(TModel model) {
            if (model == null)
            {
                AddError(nameof(model), $"model[{(typeof(TModel))}] is null");
                return false;
            }

            if (RequiredProperties != null && RequiredProperties.Length > 0) { ValidateRequiredFields(model); }
            if (Validate != null) { Validate(model); }
            bool isValid = Errors.Count == 0; 
            return isValid;
        }
        /// <summary>
        /// 檢核指定的必填欄位
        /// </summary>
        /// <param name="model"></param>
        protected void ValidateRequiredFields(TModel model)
        {
            if(model  == null) { return; } 
            if (RequiredProperties == null) return;
            object? value;
            foreach (var prop in RequiredProperties)
            {
                value = prop.GetValue(model);
                if (value == null) AddError(prop.Name, _vTexts["未填寫"]);
            }
        }
        /// <summary>
        /// 指定必填欄位
        /// </summary>
        /// <param name="selector">欄位選擇器</param>
        public void SetRequiredFields(Expression<Func<TModel, object>> anonymousSelector)
        {
            RequiredProperties = anonymousSelector.GetSelectedProperties();
        }
    }

}

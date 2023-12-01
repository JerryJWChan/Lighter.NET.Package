namespace Lighter.NET.Common
{
    using Microsoft.Extensions.Localization;
    using System.Collections.Generic;
    /// <summary>
    /// 複合式語系轉換器
    /// </summary>
    public class CompositeLocalizer : IStringLocalizer
    {
        private List<IStringLocalizer> _localizers = new List<IStringLocalizer>();
        /// <summary>
        /// 建構時可傳入多個localizer(至少一個，否則報錯)
        /// </summary>
        /// <param name="localizers"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CompositeLocalizer(params IStringLocalizer[] localizers)
        {
            if (localizers == null || localizers.Length == 0)
            {
                throw new ArgumentNullException(nameof(localizers));
            }
            _localizers.AddRange(localizers);
        }
        /// <summary>
        /// 一般對照取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LocalizedString this[string name]
        {
            get
            {
                LocalizedString mappedString = new LocalizedString(name, name); ;
                foreach (var localizer in _localizers)
                {
                    mappedString = localizer[name];
                    if (mappedString.ResourceNotFound == false)
                    {
                        break;
                    }
                }
                return mappedString;
            }
        }

        /// <summary>
        /// 對照取值，並帶入參數後組成結果字串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                LocalizedString mappedString = new LocalizedString(name, name); ;
                foreach (var localizer in _localizers)
                {
                    mappedString = localizer[name, arguments];
                    if (mappedString.ResourceNotFound == false)
                    {
                        break;
                    }
                }
                return mappedString;
            }
        }

        /// <summary>
        /// 取得全部對照字串
        /// </summary>
        /// <param name="includeParentCultures"></param>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            IEnumerable<LocalizedString> allStrings = Enumerable.Empty<LocalizedString>();
            foreach (var localizer in _localizers)
            {
                allStrings = allStrings.Concat(localizer.GetAllStrings(includeParentCultures));
            }
            return allStrings;
        }
    }
}

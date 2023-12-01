using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Lighter.NET.Common
{
    /// <summary>
    /// 多語系輔助函式
    /// </summary>
    public static class LangHelper
    {
        /// <summary>
        /// 取得字串語系轉換器
        /// </summary>
        /// <typeparam name="TResourceForClass">語系資源檔所對應的目標Class類別</typeparam>
        /// <param name="resourcePath">資源檔根目錄</param>
        /// <returns></returns>
        public static IStringLocalizer<TResourceForClass> GetLocalizer<TResourceForClass>(string resourcePath = "Resources")
        {
            //多語系字串轉換器
            var localizeOptions = Options.Create(new LocalizationOptions() { ResourcesPath = resourcePath });
            var nullLogger = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory();
            var factory = new ResourceManagerStringLocalizerFactory(localizeOptions, nullLogger);
            var localizer = new StringLocalizer<TResourceForClass>(factory);
            return localizer;
        }

        /// <summary>
        /// 預設語系轉換器
        /// </summary>
        public static IStringLocalizer DefaultLocalizer { get; set; } = GetLocalizer<DefaultResource>();

		/// <summary>
		/// 取得語系轉換後的字串
		/// </summary>
		/// <typeparam name="TResourceForClass">語系資源檔所對應的目標Class類別</typeparam>
		/// <param name="text">原字串(可帶{0}{1}格式)</param>
		/// <param name="args">要套進格式的參數</param>
		/// <returns></returns>
		public static string GetLocalizedText<TResourceForClass>(string text, params object[] args)
        {
            string resourcePath = "Resources";
            var localizer = GetLocalizer<TResourceForClass>(resourcePath);
            if (localizer != null)
            {
                string mapToText = localizer.GetString(text, args);
                if (!string.IsNullOrEmpty(mapToText))
                {
                    return mapToText;
                }
                else
                {
                    return text;
                }
            }
            else
            {
                return text;
            }
        }

        /// <summary>
        /// 取得現行語系列舉
        /// </summary>
        /// <returns></returns>
        public static CultureName GetCultureName()
        {
            string sysCultureName = Thread.CurrentThread.CurrentCulture.Name;
            switch (sysCultureName)
            {
                case "zh-Hant-TW":
                case "zh-Hant":
                case "zh-CHT":
                case "zh-TW":
                    return CultureName.CT;
                case "en":
                    return CultureName.EN;
                case string s when s.StartsWith("en-"):
                    return CultureName.EN;
                case "zh-Hans-CN":
                case "zh-Hans":
                case "zh-CN":
                case "zh-CHS":
                case "zh":
                    return CultureName.CS;

                default:
                    return CultureName.CT;
            }
        }

        /// <summary>
        /// 解析語系代碼成列舉值
        /// </summary>
        /// <returns></returns>
        public static CultureName ParseCultureName(string cultureCode)
        {
            switch (cultureCode)
            {
                case "":
                    return GetCultureName();
                case "zh-TW":
                case "zh-Hant-TW":
                case "zh-CHT":
                case "CT":
                case "zh-Hant":
                case "ct":
                case "CHT":
                    return CultureName.CT;
                case "en":
                case "EN":
                    return CultureName.EN;
                case string s when s.StartsWith("en-"):
                    return CultureName.EN;
                case "zh-CN":
                case "zh-Hans-CN":
                case "zh-CHS":
                case "CS":
                case "cs":
                case "zh-Hans":
                case "CHS":
                case "chs":
                case "zh":
                    return CultureName.CS;

                default:
                    return CultureName.CT;
            }
        }

        /// <summary>
        /// 轉成語系代碼
        /// </summary>
        /// <param name="cultureName">語系列舉</param>
        /// <returns></returns>
        public static string ToCultureCode(this CultureName cultureName)
        {
            string cultureCode = "";
            switch (cultureName)
            {
                case CultureName.Current:
                case CultureName.CT:
                default:
                    //cultureCode = "zh-Hant-TW";   //ISO Standard
                    cultureCode = "zh-TW";          //Microsoft Alias Name
                    break;
                case CultureName.CS:
                    //cultureCode = "zh-Hans-CN";   //ISO Standard
                    cultureCode = "zh-CN";          //Microsoft Alias Name
                    break;
                case CultureName.EN:
                    cultureCode = "en";
                    break;
            }
            return cultureCode;
        }

        /// <summary>
        /// 設定語系
        /// </summary>
        public static void SetCulture(CultureName cultureName, HttpContext? httpContext = null)
        {
            string cultureCode = cultureName.ToCultureCode();

            //(1)設定current thread語系
            var cultureInfo = new System.Globalization.CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            //(2)設定request語系(for後續的request):此處用的是CookieRequestCultureProvider

        }

        /// <summary>
        /// 設定語系
        /// </summary>
        public static void SetCulture(string cultureCode)
        {
            var cultureName = ParseCultureName(cultureCode);
            SetCulture(cultureName);
        }

        /// <summary>
        /// 變更語系API網址
        /// </summary>
        /// <returns></returns>
        public static string GetChangeCultureUrl(CultureName cultureName, string returnUrl, string setCultureControllerName = "Verification", string setCultureActionName = "SetLanguage")
        {
            string cultureCode = cultureName.ToCultureCode();
            string url = $"~/{setCultureControllerName}/{setCultureActionName}?culture={cultureCode}&returnUrl={returnUrl}";
            return url;
        }
    }
}

using System.Collections.Concurrent;
using System.Reflection;

namespace Lighter.NET.Common
{
    /// <summary>
    /// 型別延伸函式
    /// </summary>
    public static class TypeExtension
    {
        #region DateTime轉特定格式字串
        /// <summary>
        /// Nullable type
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string yyyy_MM_dd(this DateTime? date)
        {
            return date.HasValue ? date.Value.yyyy_MM_dd() : "";
            
        }
        public static string yyyy_MM_dd_HH_mm(this DateTime? date)
        {
            return date.HasValue ? date.Value.yyyy_MM_dd_HH_mm() : "";
        }
        public static string yyyy_MM_dd_HH_mm_ss(this DateTime? date)
        {
            return date.HasValue ? date.Value.yyyy_MM_dd_HH_mm_ss() : "";
        }
        public static string yyyy_MM_ddTHH_mm(this DateTime? date)
        {
            return date.HasValue ? date.Value.yyyy_MM_ddTHH_mm() : "";
        }
        public static string yyyy_MM_ddTHH_mm_ss(this DateTime? date)
        {
            return date.HasValue ? date.Value.yyyy_MM_ddTHH_mm_ss() : "";
        }

        public static string yyyyMMddHHmm(this DateTime? date)
        {
            return date.HasValue ? date.Value.yyyyMMddHHmm() : "";
        }
        public static string yyyyMMddHHmmss(this DateTime? date)
        {
            return date.HasValue ? date.Value.yyyyMMddHHmmss() : "";
        }
        /// <summary>
        /// Non-Nullable type
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string yyyy_MM_dd(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd") ;
        }
        public static string yyyy_MM_dd_HH_mm(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm") ;
        }
        public static string yyyy_MM_dd_HH_mm_ss(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss") ;
        }
        public static string yyyy_MM_ddTHH_mm(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm");
        }
        public static string yyyy_MM_ddTHH_mm_ss(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static string yyyyMMddHHmm(this DateTime date)
        {
            return date.ToString("yyyyMMddTHHmm");
        }

        public static string yyyyMMddHHmmss(this DateTime date)
        {
            return date.ToString("yyyyMMddTHHmmss");
        }
        #endregion

        #region Anonymous Type
        /// <summary>
        /// 判斷物件是否是AnonymousType
        /// </summary>
        /// <param name="any">任何物件</param>
        /// <returns></returns>
        public static bool IsAnonymousType(this object? any)
        {
            if (any == null) return false;
            string expr = any?.ToString()??"";
            if (string.IsNullOrEmpty(expr)) return false;
            bool isAnonymous = expr.StartsWith('{') && expr.EndsWith('}');
            return isAnonymous;
        }
        #endregion

        #region ClassExtension
        /// <summary>
        /// 取值(依照欄位名稱)
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="classInstance">物件實際</param>
        /// <param name="propertyName">屬性名稱</param>
        /// <param name="returnNullWhenClassInstanceIsNull">當classInstance為null時，回傳null；若否，則拋exception</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static object? GetValue<TClass>(this TClass classInstance, string propertyName, bool returnNullWhenClassInstanceIsNull = true) where TClass : class
        {
            if (classInstance == null)
            {
                if (returnNullWhenClassInstanceIsNull)
                {
                    return null;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            var prop = typeof(TClass).GetProperty(propertyName);
            if (prop == null)
            {
                throw new Exception($"The propertyName={propertyName} is not a valid property name of the class instance.");
            }
            return prop.GetValue(classInstance);

        }
        #endregion

    }
}

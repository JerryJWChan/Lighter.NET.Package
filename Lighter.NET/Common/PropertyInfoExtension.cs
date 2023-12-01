using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Lighter.NET.Common
{
    /// <summary>
    /// PropertyInfo延伸函式
    /// </summary>
    public static class PropertyInfoExtension
    {
        /// <summary>
        /// 屬性定義是否標註[Required]
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static bool IsRequired(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return false;
            return propertyInfo.IsDefined(typeof(RequiredAttribute));
        }

        /// <summary>
        /// 取得DisplayAttribute
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static DisplayAttribute? GetDisplay(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return null;
            var attr = propertyInfo.GetCustomAttribute(typeof(DisplayAttribute));
            if (attr == null) return null;
            return (DisplayAttribute)attr;
        }

        /// <summary>
        /// 取得DisplayAttribute中定義的Name，若無，則用PropertyName
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static string GetDisplayName(this PropertyInfo propertyInfo)
        {
            var displayAttr = GetDisplay(propertyInfo);
            if (displayAttr == null) return "";
            string fallbackName = (propertyInfo != null)? propertyInfo.Name:"";
            return displayAttr.Name ?? fallbackName;
        }

        /// <summary>
        /// 取得DisplayAttribute中定義的ShortName，若無，則用DisplayName，再無，則用PropertyName
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="fallbackToDisplayName"></param>
        /// <returns></returns>
        public static string GetShortName(this PropertyInfo propertyInfo, bool fallbackToDisplayName=true)
        {
            var displayAttr = GetDisplay(propertyInfo);
            if (displayAttr == null) return "";
            string fallbackName = (propertyInfo != null) ? propertyInfo.Name : "";
            string displayName = displayAttr.Name ?? fallbackName;
            return displayAttr.ShortName?? displayName; 
        }
    }
}

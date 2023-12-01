using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Lighter.NET.Common
{
    /// <summary>
    /// Model屬性的描述資料
    /// </summary>
    public class MetaProperty : IMetaProperty
    {
        /// <summary>
        /// 屬性資訊
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object? Value { get; set; }
        public MetaProperty(PropertyInfo propertyInfo, object? value)
        {
            PropertyInfo = propertyInfo;
            Value = value;
        }
        /// <summary>
        /// 是否必填欄位
        /// </summary>
        public bool IsRequired { 
            get 
            { 
                if(PropertyInfo== null) return false;
                return PropertyInfo.IsDefined(typeof(RequiredAttribute));
            } 
        } 
        /// <summary>
        /// 屬性宣告的Display標註
        /// </summary>
        public DisplayAttribute? Display
        {
            get
            {
                if (PropertyInfo == null) return null;
                var attr = PropertyInfo.GetCustomAttribute(typeof(DisplayAttribute));
                if(attr == null) return null;   
                return (DisplayAttribute)attr;
            }
        }
    }
}

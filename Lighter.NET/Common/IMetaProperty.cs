using System.Reflection;

namespace Lighter.NET.Common
{
    /// <summary>
    /// 元屬性介面
    /// (用於保存PropertyInfo和Value，減少重複使用Reflection取值)
    /// </summary>
    public interface IMetaProperty
    {
        PropertyInfo PropertyInfo { get; set; }
        object? Value { get; set; }
        ///// <summary>
        ///// 屬性名
        ///// </summary>
        //string Name { get; set; }
        ///// <summary>
        ///// 屬性值
        ///// </summary>
        //object? Value { get; set; }
        ///// <summary>
        ///// 顯示名
        ///// </summary>
        //string DisplayName { get; set; }

        ///// <summary>
        ///// 值型別
        ///// </summary>
        //Type DataType { get; set; }
        ///// <summary>
        ///// 是否必填
        ///// </summary>
        //bool Required { get; set; } 

    }
}

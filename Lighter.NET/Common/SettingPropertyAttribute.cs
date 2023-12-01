namespace Lighter.NET.Common
{
    /// <summary>
    /// 設定屬性(標註此attribute的property可作為參數設定)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited =true,AllowMultiple =false)]
    public class SettingPropertyAttribute:Attribute
    {
    }
}

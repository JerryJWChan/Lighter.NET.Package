namespace Lighter.NET.Common
{
/// <summary>
/// Enum型別的各別列舉項目所要對應的字串值，不同於emum_member.ToString(), .ToString()得到的只是enum項目的名稱而已
/// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple =false, Inherited = false)]
    public class EnumStringValueAttribute:Attribute
    {
        /// <summary>
        /// 列舉項目所要對應的字串值
        /// </summary>
        public string Value { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">此列舉項目所要對應的字串值</param>
        public EnumStringValueAttribute(string value) 
        {
            Value = value;
        }
    }
}

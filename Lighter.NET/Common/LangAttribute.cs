namespace Lighter.NET.Common
{
    /// <summary>
    /// 語系對照Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class LangAttribute:Attribute
    {
        /// <summary>
        /// 語系文字對照項
        /// </summary>
        public Lang Lang { get; set; } = new Lang();
        /// <summary>
        /// 語系對照Attribute
        /// </summary>
        /// <param name="lang">語系文字對照項</param>
        public LangAttribute(Lang lang) 
        {
            Lang = lang;
        }
        /// <summary>
        /// 語系對照Attribute
        /// </summary>
        /// <param name="CT">繁體中文</param>
        /// <param name="EN">英文</param>
        /// <param name="CS">簡體中文</param>
        public LangAttribute(string CT="(未定義)",string EN = "(未定義)", string CS = "(未定義)")
        {
            if (EN == "(未定義)") EN = CT;
            if (CS == "(未定義)") CS = CT;

            Lang = new Lang() { CT = CT, EN = EN , CS = CS };
        }
    }
}

using System.ComponentModel;
using System.Globalization;

namespace Lighter.NET.Common
{
    /// <summary>
    /// 對應DB中使用char(1)表示「是/否」的型別
    /// </summary>
    [TypeConverter(typeof(CharBoolConverter))]
    public class CharBool
    {
        /// <summary>
        /// Value屬性的內存
        /// </summary>
        private string _value = "";

        /// <summary>
        /// 是(勾選)的值，預設為1
        /// </summary>
        public string TrueValue { get; set; } = "1";
        /// <summary>
        /// 否(取消勾選)的值，預設為0
        /// </summary>
        public string FalseValue { get; set; } = "0";
        /// <summary>
        /// 依照true/false狀態回傳對應的字串值
        /// </summary>
        public string Value
        {
            get
            {
                return IsTrue ? TrueValue : FalseValue;
            }
            set
            {
                if(string.IsNullOrEmpty(value)) value = DefaultValue;
                _value = value;
            }
        }
        public string DefaultValue { get; set; } = "";
        /// <summary>
        /// 是否勾選
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return IsTrueValue(_value);
            }
        }

        /// <summary>
        /// 內含字串是否表示True (成立的字串："TRUE,FALSE", "TRUE", "YES", "ON", "Y", "1", "T")
        /// </summary>
        /// <param name="value">字串</param>
        /// <returns></returns>
        public bool IsTrue
        {
            get 
            {
                return IsTrueValue(_value);
            }
        }

        /// <summary>
        /// 取得正規化的值(將所有可能代表true的值轉換成"1",其他為"0")
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns></returns>
        public static string GetNormalizedValue(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "0";
            //比對true的可能值
            bool isTrue = new[] { "TRUE,FALSE", "TRUE", "YES", "ON", "Y", "1", "T" }.Contains(value.ToUpper());
            return isTrue ? "1" : "0";
        }

        public CharBool() { }
        public CharBool(string? value)
        {
            Value = value??"";
        }
        /// <summary>
        /// CharBool to string conversion
        /// </summary>
        /// <param name="self">CharBool物件</param>
        public static implicit operator string(CharBool self)
        {
            if(self==null) self= new CharBool();
            return self.Value;
        }

        /// <summary>
        /// string to CharBool conversion
        /// </summary>
        /// <param name="value">字串</param>
        public static implicit operator CharBool(string value)
        {
            CharBool result = new CharBool();
            result.Value = value;
            return result;
        }

        /// <summary>
        /// CharBool to bool conversion
        /// </summary>
        /// <param name="self"></param>
        public static implicit operator bool(CharBool self)
        {
            return self.IsTrue;
        }

        /// <summary>
        /// bool to CharBool conversion
        /// </summary>
        /// <param name="boolValue">(true/false)</param>
        public static implicit operator CharBool(bool boolValue)
        {
            CharBool result = new CharBool("test");
            string strValue = boolValue ? result.TrueValue : result.FalseValue;
            result.Value = strValue;
            return result;
        }

        /// <summary>
        /// 傳入的字串是否表示true，包含："TRUE,FALSE", "TRUE", "YES", "ON", "Y", "1", "T" 
        /// </summary>
        /// <param name="value">字串</param>
        /// <returns></returns>
        private bool IsTrueValue(string value)
        {
            if (string.IsNullOrEmpty(value)) value = this.DefaultValue;
            value = value.ToUpper();
            //(1)比對true的可能值
            bool isTrue = new[] { "TRUE,FALSE", "TRUE", "YES", "ON", "Y", "1", "T" }.Contains(value);
            //(2)若(1)的比對無符合時，再比對與TureValue設定若相同則表示有勾選
            if (isTrue == false && string.IsNullOrEmpty(TrueValue) == false && value == TrueValue) isTrue = true;

            return isTrue;
        }
    }

    public class CharBoolConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if(sourceType == typeof(string)) return true;   
            if(sourceType == typeof(bool)) return true;
            if (sourceType == typeof(object)) return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if(value == null) return new CharBool();
            if(value is CharBool) return (CharBool)value;
            string strValue = value?.ToString()??"";
            return new CharBool(strValue);

            //return base.ConvertFrom(context, culture, value);
        }
    }
}

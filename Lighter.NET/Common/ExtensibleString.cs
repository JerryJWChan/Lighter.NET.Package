using Microsoft.Extensions.Hosting;

namespace Lighter.NET.Common
{
    /// <summary>
    /// 具有延展性的String，可和另外的string進行And和Or運算
    /// </summary>
    public class ExtensibleString : IMetaModel
    {
        private string? _value = "";
        private string _seperator = "";
        /// <summary>
        /// 結果字串值
        /// </summary>
        public string? Value 
        { 
            get 
            { 
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        /// <summary>
        /// 分隔字元/字串(And()操作時用
        /// </summary>
        public string Seperator 
        { 
            get 
            { 
                return _seperator;
            }
            set
            {
                _seperator = value;
            }
        }
        public ExtensibleString() { }
        public ExtensibleString(string? value) 
        { 
            _value = value; 
        }
        /// <summary>
        /// 初始字串
        /// </summary>
        /// <param name="value">字串值</param>
        /// <param name="seperator">分隔字元/字串(And()操作時用)</param>
        public ExtensibleString(string? value, string seperator)
        {
            _value = value;
            _seperator = seperator??"";
        }
        /// <summary>
        /// 或運算(在原字串與另一個字串中，取第1個非空的字串)
        /// </summary>
        /// <param name="anotherString">另一個字串</param>
        /// <returns></returns>
        public ExtensibleString Or(string? anotherString)
        {
            if(string.IsNullOrEmpty(_value) && !string.IsNullOrEmpty(anotherString))
            {
                _value = anotherString;
            }
            return this;
        }

        /// <summary>
        /// 且運算(將原字串與另一字串，串接起來)
        /// </summary>
        /// <param name="anotherString">另一字串</param>
        /// <param name="ignoreEmptyString">是否忽略空字串</param>
        /// <returns></returns>
        public ExtensibleString And(string? anotherString, bool ignoreEmptyString = true)
        {
            if (string.IsNullOrEmpty(anotherString) && ignoreEmptyString) return this;
            if (string.IsNullOrEmpty(anotherString)) anotherString = "";
            if (ignoreEmptyString)
            {
                if (string.IsNullOrEmpty(_value))
                {
                    _value = anotherString;
                }
                else
                {
                    _value = $"{_value}{_seperator}{anotherString}";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_value)) _value = "";
                _value = $"{_value}{_seperator}{anotherString}";
            }

            return this;
        }

        /// <summary>
        /// 且運算(將原字串與另一字串，串接起來)
        /// </summary>
        /// <param name="anotherString">另一字串</param>
        /// <param name="seperator">分隔字元/字串</param>
        /// <param name="ignoreEmptyString">是否略過空字串</param>
        /// <returns></returns>
        public ExtensibleString And(string? anotherString, string seperator, bool ignoreEmptyString = true)
        {
            if (string.IsNullOrEmpty(anotherString) && ignoreEmptyString) return this;
            if (string.IsNullOrEmpty(anotherString)) anotherString = "";
            if (ignoreEmptyString)
            {
                if (string.IsNullOrEmpty(_value))
                {
                    _value = anotherString;
                }
                else
                {
                    _value = $"{_value}{seperator}{anotherString}";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_value)) _value = "";
                _value = $"{_value}{seperator}{anotherString}";
            }

            return this;

            //if (string.IsNullOrEmpty(anotherString) && ignoreEmptyString) return this;
            //_value = $"{_value}{seperator}{anotherString}";
            //return this;
        }

        /// <summary>
        /// 加入字串(將原字串與另一字串，串接起來)
        /// </summary>
        /// <param name="anotherString">另一字串</param>
        /// <param name="delimiter">分隔字元/字串</param>
        /// <param name="ignoreEmptyString">是否略過空字串</param>
        public ExtensibleString Add(string? anotherString, string delimiter = ",", bool ignoreEmptyString = true)
        {
            return this.And(anotherString, delimiter, ignoreEmptyString);
        }

        /// <summary>
        /// 轉成字串(即Value值)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value??"";
        }

        /// <summary>
        /// 指派(Value)給字串變數
        /// </summary>
        /// <param name="extStr"></param>
        public static implicit operator string(ExtensibleString extStr)
        {
            return extStr._value ?? "";
        }

    }

    /// <summary>
    /// String調用ExtensibleString的延伸函式
    /// </summary>
    public static class StringToExtensibleStringExtension
    {
        /// <summary>
        /// 或運算(在原字串與另一字串中，取第1個非空的字串)
        /// </summary>
        /// <param name="firstStr">原字串</param>
        /// <param name="secondStr">另一字串</param>
        /// <returns></returns>
        public static ExtensibleString Or (this string? firstStr, string? secondStr)
        {
            var extStr = new ExtensibleString(firstStr).Or(secondStr);
            return extStr;
        }

        /// <summary>
        /// 且運算(將原字串與另一字串，串接起來)
        /// </summary>
        /// <param name="firstStr">原字串</param>
        /// <param name="secondStr">另一字串</param>
        /// <returns></returns>
        public static ExtensibleString And(this string? firstStr, string? secondStr)
        {
            var extStr = new ExtensibleString(firstStr).And(secondStr);
            return extStr;
        }

        /// <summary>
        /// 且運算(將原字串與另一字串，串接起來，使用seperator分隔)
        /// </summary>
        /// <param name="firstStr">原字串</param>
        /// <param name="secondStr">另一字串</param>
        /// <param name="seperator">分隔字元/字串</param>
        /// <returns></returns>

        public static ExtensibleString And(this string? firstStr, string? secondStr, string seperator)
        {
            var extStr = new ExtensibleString(firstStr,seperator).And(secondStr);
            return extStr;
        }

        /// <summary>
        /// 且運算(將原字串與另一字串，串接起來，使用delimiter分隔)
        /// </summary>
        /// <param name="firstStr">原字串</param>
        /// <param name="secondStr">另一字串</param>
        /// <param name="delimiter">分隔字元/字串</param>
        /// <returns></returns>
        public static ExtensibleString Add(this string? firstStr, string? secondStr, string delimiter = ",")
        {
            var extStr = new ExtensibleString(firstStr, delimiter).And(secondStr);
            return extStr;
        }
    }
}

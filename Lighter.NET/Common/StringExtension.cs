using System.Text;

namespace Lighter.NET.Common
{
    /// <summary>
    /// 字串延伸函式
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 字串是否null或空值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string? value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 取值(確保結果非null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fallbackValue">若字串為null時的預設值</param>
        /// <returns></returns>
        public static string Get(this string value, string fallbackValue = "")
        {
            if (string.IsNullOrEmpty(value)) { return fallbackValue; }
            return value;
        }

		/// <summary>
		/// 重複字串至指定重複次數
		/// </summary>
		/// <param name="text">原始字串</param>
		/// <param name="repeatCount">重複次數(陣列元素個數)</param>
		/// <returns></returns>
		public static string Repeat(this string text,int repeatCount)
        {
            var textArr = System.Linq.Enumerable.Repeat(text, repeatCount).ToArray();
            return string.Join("", textArr);
		}

		/// <summary>
		/// 重複字元至指定重複次數
		/// </summary>
		/// <param name="character">原始字元</param>
		/// <param name="repeatCount">重複次數(陣列元素個數)</param>
		/// <returns></returns>
		public static string Repeat(this char character, int repeatCount)
		{
			var charArr = System.Linq.Enumerable.Repeat(character, repeatCount).ToArray();
			return string.Join("", charArr);
		}

		/// <summary>
		/// 重複字串並轉成陣列
		/// </summary>
		/// <param name="text">原始字串</param>
		/// <param name="repeatCount">重複次數(陣列元素個數)</param>
		/// <returns></returns>
		public static string[] RepeatToArray(this string text, int repeatCount)
        {
            return System.Linq.Enumerable.Repeat(text, repeatCount).ToArray();
        }

        /// <summary>
        /// 將字串(0或1)轉成html checkbox的checked屬性
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CheckFlag(this string? value)
        {
            return (string.IsNullOrEmpty(value) == false && value == "1") ? "checked" : "";
        }

        /// <summary>
        /// 字串是否可表示True
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTrue(this string? value)
        {
            var charBool = new CharBool(value);
            return charBool.IsTrue;
        }

        /// <summary>
        /// 將字串序列合併成單一字串
        /// </summary>
        /// <param name="values"></param>
        /// <param name="separator">分隔字串/字元</param>
        /// <returns></returns>
        public static string Combine(this IEnumerable<string>? values, string separator="")
        {
            if (values == null) return "";
            StringBuilder sb = new StringBuilder();
            if(string.IsNullOrEmpty(separator)) { separator = ""; }
            if(separator.Length > 0)
            {
                foreach (var item in values)
                {
                    sb.Append(item).Append(separator);
                }

                if (sb.Length > 0) sb.Remove(sb.Length - separator.Length + 1, separator.Length);
                return sb.ToString();
            }
            else
            {
                foreach (var item in values)
                {
                    sb.Append(item);
                }
                return sb.ToString();
            }
        }
        /// <summary>
        /// 將字元序列合併成單一字串
        /// </summary>
        /// <param name="values"></param>
        /// <param name="separator">分隔字串/字元</param>
        /// <returns></returns>
        public static string Combine(this IEnumerable<char>? values, string separator = "")
        {
            if (values == null) return "";
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(separator)) { separator = ""; }
            if (separator.Length > 0)
            {
                foreach (var item in values)
                {
                    sb.Append(item).Append(separator);
                }

                if (sb.Length > 0) sb.Remove(sb.Length - separator.Length + 1, separator.Length);
                return sb.ToString();
            }
            else
            {
                foreach (var item in values)
                {
                    sb.Append(item);
                }
                return sb.ToString();
            }
        }
        /// <summary>
        /// 是否全部是重複字完
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAllRepeatCharacter(this string? value)
        {
            if(string.IsNullOrEmpty(value)) { return false; }
            //全部空白
            if (value.Length > 0 && value.Trim().Length == 0) return true;
            value= value.Trim();
            var chars = value.ToCharArray();
            //重複字元
            for (int i = 0; i < chars.Length-1; i++)
            {
                if (chars[i] != chars[i+1]) return false;
            }

            return true;
            ////重複字元
            //string match = Enumerable.Repeat(value.Substring(0, 1), value.Length).Combine();
            //return value == match;
        }
        /// <summary>
        /// 是否全部是重複字完
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCharacters">忽略字元</param>
        /// <returns></returns>
        public static bool IsAllRepeatCharacter(this string? value, params char[] ignoreCharacters)
        {
            if (string.IsNullOrEmpty(value)) { return false; }
            if(ignoreCharacters != null && ignoreCharacters.Length >= 0)
            {
                string filtered =  value.Except(ignoreCharacters).Combine();
                return IsAllRepeatCharacter(filtered);
            }
            else
            {
                return IsAllRepeatCharacter(value);
            }
        }

        /// <summary>
        /// 截斷至長度上限maxLength
        /// </summary>
        /// <param name="original">原始字串</param>
        /// <param name="maxLength">長度上限</param>
        /// <returns></returns>
        public static string TruncateToMaxLength(this string original,int maxLength)
        {
            if (string.IsNullOrEmpty(original)) { return ""; }
            return original!.Substring(0, Math.Min(original.Length, maxLength));
        }

        /// <summary>
        /// 過濾掉指定的字元
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charArray">要過濾掉字元陣列</param>
        /// <returns></returns>
        public static string FilterOut(this string value, char[] charArray)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if(charArray == null || charArray.Length==0) return value;  
            for(int i=0; i<charArray.Length; i++)
            {
                value = value.Replace(charArray[i].ToString(),string.Empty);
            }
            return value;   
        }
    }
}

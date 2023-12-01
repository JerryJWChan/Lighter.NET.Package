using System.Text;

namespace Lighter.NET.Common
{
    /// <summary>
    /// Json輔助函式
    /// </summary>
    public class CustomJsonSerializer
    {
        /// <summary>
        /// 序列化成json(輸入物件參數)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string Serialize(object? model)
        {
            /*
             * only escape for ", \, / , and control character
             * u followed by four-hex-digits should be escape to \uxxxx (但在UTF-8編碼時，可不用escape，故略)
             */

            /*
             * To Do : recursive
             */

            if (model == null) return "{}";
            Type thisType = model.GetType();

            var props = model.GetType().GetProperties();
            StringBuilder sb = new StringBuilder();
            foreach (var p in props)
            {
                string escaped = EscapeJsonValue(p.GetValue(model)?.ToString() ?? "");
                sb.Append($"\"{ToCamelCase(p.Name)}\":\"{HtmlEncodeProvider.Encode(escaped)}\",");
            }
            string json = "{" + sb.ToString().TrimEnd(',') + "}";
            return json;
        }

        /// <summary>
        /// 序列化成json(輸入陣列參數)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="modelList"></param>
        /// <returns></returns>
        public static string Serialize<TModel>(IEnumerable<TModel>? modelList) where TModel : class
        {
            /*
             * only escape for ", \, / , and control character
             * u followed by four-hex-digits should be escape to \uxxxx (但在UTF-8編碼時，可不用escape，故略)
             */

            /*
             * To Do : recursive
             */

            if (modelList == null || modelList.Count() == 0) return "[]";
            var props = typeof(TModel).GetProperties();
            StringBuilder sb = new StringBuilder();
            foreach (var m in modelList)
            {
                sb.Append('{');
                for (int i = 0; i < props.Length; i++)
                {
                    string escaped = EscapeJsonValue(props[i].GetValue(m)?.ToString() ?? "");
                    sb.Append($"\"{ToCamelCase(props[i].Name)}\":\"{HtmlEncodeProvider.Encode(escaped)}\"");
                    if(i< props.Length - 1)
                    {
                        sb.Append(',');
                    }
                }
                sb.Append("},");
            }

            string json = "[" + sb.ToString().TrimEnd(',') + "]";
            return json;
        }

        /// <summary>
        /// 將字串(變數名稱或屬性名稱)轉換成CamelCase格式
        /// </summary>
        /// <param name="original">原字串</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToCamelCase(string original)
        {
            if(string.IsNullOrEmpty(original)) throw new ArgumentNullException(nameof(original));
            if(original.Length == 1) return original.ToLower();
            return char.ToLower(original[0]) + original.Substring(1);
        }

        /// <summary>
        /// 將json特殊字元做escape編碼
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EscapeJsonValue(string value)
        {
            /*
             * only escape for ", \, / , and control character
             * u followed by four-hex-digits should be escape to \uxxxx (但在UTF-8編碼時，可不用escape，故略)
             */

            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(value)) return "";
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '"':
                        sb.Append('\"');
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '/':
                        sb.Append("\\/");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    default:
                        sb.Append(value[i]);
                        break;

                }
            }

            return sb.ToString();
        }
    }
}

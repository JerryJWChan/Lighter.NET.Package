using Microsoft.Extensions.Primitives;

namespace Lighter.NET.Common
{
    /// <summary>
    /// Http Header輔助函式
    /// </summary>
    public static class HttpHeaderExtension
    {
        /// <summary>
        /// 取得特定key的header值
        /// </summary>
        /// <param name="headers">header集合</param>
        /// <param name="key">key</param>
        /// <returns>header值或空字串</returns>
        public static string Get(this Microsoft.AspNetCore.Http.IHeaderDictionary headers,string key)
        {
            if (headers == null) return "";
            if (headers.TryGetValue(key, out StringValues value))
            {
                string strValue =  value.ToString();
                if (string.IsNullOrEmpty(strValue)) return "";
                return strValue;
            }
            else
            {
                return "";
            }
        }

    }
}

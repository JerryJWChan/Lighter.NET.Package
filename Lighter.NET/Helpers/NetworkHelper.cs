using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Primitives;
using Lighter.NET.Common;

namespace Lighter.NET.Helpers
{
    /// <summary>
    /// 網路相關輔助函式
    /// </summary>
    public class NetworkHelper
    {
        /// <summary>
        /// 取得Client IP位址
        /// </summary>
        /// <param name="http">current http context</param>
        /// <param name="ipCount">要取幾組ip(可能的)位址，預設1組，最多3組</param>
        /// <returns></returns>
        public static string GetClientIP(HttpContext http, int ipCount = 1)
        {
            /*
             * IPv4長度(max):15
             * IPv6長度(max):39
             */
            if (http == null) return "NA-1";

            if(ipCount < 1) ipCount= 1;
            if(ipCount > 3) ipCount= 3; 
            List<string> ips= new List<string>();
            //第1組
            string xffs = http.Request.Headers.Get("X-Forwarded-For");
            if (xffs != "")
            {
                var first = xffs.TrimEnd(',').Split(',').Select(x => x.Trim()).FirstOrDefault();
                if (!string.IsNullOrEmpty(first)){ ips.Add(first); }
            }
            //第2組
            if(http.Connection.RemoteIpAddress != null){
                var second = http.Connection.RemoteIpAddress.ToString();
                if (!string.IsNullOrEmpty(second)){ips.Add(second);}
            }
            //第3組
            var third = http.Request.Headers.Get("REMOTE_ADDR");
            if(third != "") {ips.Add(third);}

            if (ips.Count == 0) return "NA-2";

            string[] picked;
            if(ips.Count > ipCount) 
            {
                picked = ips.Take(ipCount).ToArray();
            }
            else
            {
                picked = ips.ToArray();
            }

            return string.Join(",", picked);
        }
    }
}

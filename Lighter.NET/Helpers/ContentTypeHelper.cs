using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lighter.NET.Helpers
{
    /// <summary>
    /// ContentType(MimeType)輔助函式
    /// </summary>
    public static class ContentTypeHelper
    {
        /// <summary>
        /// MIME type對照表
        /// </summary>
        private static Dictionary<string, string> _mimeDic = new Dictionary<string, string>();
        /// <summary>
        /// 預設通用MIME-Type, 使用此MIME-Type時，需搭配以下語法
        /// Content-Type: application/octet-stream
        /// Content-Disposition: attachment; filename="(實際的檔名)"
        /// </summary>
        private const string _DEFAULT_MIME_TYPE = "application/octet-stream";
        static ContentTypeHelper() {
            /*初始化對照表*/
            _mimeDic.Add(".pdf", "application/pdf");
            _mimeDic.Add(".doc", "application/msword");
            _mimeDic.Add(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            _mimeDic.Add(".word", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            //_mimeDic.Add(".xls", "application/vnd.ms-excel");
            _mimeDic.Add(".xls", "application/ms-excel");
            //_mimeDic.Add(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            _mimeDic.Add(".xlsx", "application/ms-excel");
            _mimeDic.Add(".excel", "pplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            _mimeDic.Add(".ppt", "application/vnd.ms-powerpoint");
            _mimeDic.Add(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            _mimeDic.Add(".ods", "application/vnd.oasis.opendocument.spreadsheet");
            _mimeDic.Add(".odt", "application/vnd.oasis.opendocument.spreadsheet");
            _mimeDic.Add(".odp", "application/vnd.oasis.opendocument.presentation");
            _mimeDic.Add(".csv", "text/csv");
            _mimeDic.Add(".jpeg", "image/jpeg");
            _mimeDic.Add(".jpg", "image/jpeg");
            _mimeDic.Add(".png", "image/png");
            _mimeDic.Add(".gif", "image/gif");
            _mimeDic.Add(".bmp", "image/bmp");
            _mimeDic.Add(".txt", "text/plain");
            _mimeDic.Add(".xml", "application/xml");
            _mimeDic.Add(".json", "application/json");
            _mimeDic.Add(".htm", "text/html");
            _mimeDic.Add(".html", "text/html");
            _mimeDic.Add(".zip", "application/zip");
            _mimeDic.Add(".7z", "application/x-7z-compressed");
            _mimeDic.Add(".rar", "application/vnd.rar");
            _mimeDic.Add(".mp4", "video/mp4");
            _mimeDic.Add(".mpeg", "video/mpeg");
        }
        /// <summary>
        /// 將傳入的副檔名對應到MIME-Type
        /// </summary>
        /// <param name="fileExtension">副檔名</param>
        /// <returns></returns>
        public static string MapToMimeType(string fileExtension)
        {
            string key = fileExtension.ToLower();
            if (key.StartsWith('.') == false) key = "." + key;
            if (_mimeDic.ContainsKey(key))
            {
                return _mimeDic[key];
            }
            else
            {
                return _DEFAULT_MIME_TYPE;
            }
        }
    }
}

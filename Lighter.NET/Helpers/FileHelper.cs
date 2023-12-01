using Lighter.NET.Common;

namespace Lighter.NET.Helpers
{
    /// <summary>
    /// 檔案處理輔助函式
    /// </summary>
    public class FileHelper
    {
        #region 檔案處理
        /// <summary>
        /// 檢核檔案(不限制檔案類型)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sizeLimit">檔案大小上限</param>
        /// <param name="errMsg">錯誤訊息</param>
        /// <param name="fileSizUnit">檔案大小計算單位</param>
        /// <returns></returns>
        public static bool Validate(Microsoft.AspNetCore.Http.IFormFile file, long sizeLimit, out string errMsg, FileSizeUnit fileSizUnit = FileSizeUnit.MB)
        {
            return Validate(file,sizeLimit,new string[0],out errMsg, fileSizUnit);
        }

        /// <summary>
        /// 檢核檔案(限制檔案類型)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sizeLimit">檔案大小上限</param>
        /// <param name="acceptFileExtensions">接受的檔案類型之副檔名</param>
        /// <param name="errMsg">錯誤訊息</param>
        /// <param name="fileSizUnit">檔案大小計算單位</param>
        /// <returns></returns>
        public static bool Validate(Microsoft.AspNetCore.Http.IFormFile file, long sizeLimit, string[] acceptFileExtensions, out string errMsg, FileSizeUnit fileSizUnit = FileSizeUnit.MB)
        {
            errMsg = "";

            if (file== null || file.Length == 0) 
            {
                errMsg = "未選取檔案或檔案內容為空";
                return false;
            }

            if(acceptFileExtensions !=null && acceptFileExtensions.Length > 0)
            {
                var cleanedExtensions = acceptFileExtensions.Select(x => x.TrimStart('.').ToLower());
                string ext = Path.GetExtension(file.FileName).TrimStart('.').ToLower();
                if(cleanedExtensions.Contains(ext) == false)
                {
                    errMsg = $"拒絕檔案類型.{ext}，只接受{ string.Join(",", acceptFileExtensions)}類型";
                    return false;
                }
            }

            if(sizeLimit <=0) {
                errMsg = "檔案大小限制參數不可為0或負數";
                return false;
            }

            long longSizeLimit = sizeLimit * (int)fileSizUnit;

            if (file.Length > longSizeLimit)
            {
                errMsg = $"超過檔案大小限制{sizeLimit} {fileSizUnit.ToString()}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 存檔
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folderPhysicalPath">存檔資料夾絕對路徑</param>
        /// <param name="filename">檔名(若有給則使用此檔名，無給則用file自帶的原檔名</param>
        /// <returns></returns>
        public static ApiResult Save(Microsoft.AspNetCore.Http.IFormFile file, string folderPhysicalPath, string filename = "")
        {
            try
            {
                if (string.IsNullOrEmpty(filename)) filename = file.FileName;
                filename = Path.GetFileName(filename); //資安(清除惡意路徑)
                string filepath = Path.Combine(folderPhysicalPath, filename);
                using (var stream = new FileStream(filepath, FileMode.OpenOrCreate))
                {
                    file.CopyTo(stream);
                }
                return new ApiSuccessResult();
            }
            catch (Exception ex)
            {
                //ToDo:log error message
                return new ApiFailResult(ex.Message);
            }
        }

        /// <summary>
        /// 存檔(非同步)
        /// </summary>
        /// <param name="source">來源檔案串流</param>
        /// <param name="folderPath">存檔資料夾絕對路徑</param>
        /// <param name="errMsg">錯誤訊息</param>
        /// <param name="filename">檔名</param>
        /// <returns></returns>
        public static async Task<ApiResult> SaveAsync(Stream source, string folderPhysicalPath, string filename)
        {
            try
            {
                //if (string.IsNullOrEmpty(filename)) filename = file.FileName;
                filename = Path.GetFileName(filename); //資安(清除惡意路徑)
                string filepath = Path.Combine(folderPhysicalPath, filename);
                using (var targetStream = new FileStream(filepath, FileMode.OpenOrCreate))
                {
                    await source.CopyToAsync(targetStream);
                }
                return new ApiSuccessResult();
            }
            catch (Exception)
            {
                //ToDo:log error message
                //資安：物回傳ex.Message，因為可能包含系統路徑
                return new ApiFailResult("save file failed. please check the path is correct and having write permission");
            }
        }

        /// <summary>
        /// 存檔(非同步)
        /// </summary>
        /// <param name="source">來源檔案byte[]</param>
        /// <param name="folderPath">存檔資料夾絕對路徑</param>
        /// <param name="errMsg">錯誤訊息</param>
        /// <param name="filename">檔名</param>
        /// <returns></returns>
        public static async Task<ApiResult> SaveAsync(byte[] source, string folderPhysicalPath, string filename)
        {
            try
            {
                if(source != null && source.Length> 0)
                {
                    filename = Path.GetFileName(filename); //資安(清除惡意路徑)
                    string filepath = Path.Combine(folderPhysicalPath, filename);
                    using (var targetStream = new FileStream(filepath, FileMode.OpenOrCreate))
                    {
                        await targetStream.WriteAsync(source,0,source.Length);
                    }
                    return new ApiSuccessResult();
                }
                else
                {
                    return new ApiFailResult("save file failed. the [source] is null or empty.");
                }
      
            }
            catch (Exception)
            {
                //ToDo:log error message
                //資安：物回傳ex.Message，因為可能包含系統路徑
                return new ApiFailResult("save file failed. please check the path is correct and having write permission");
            }
        }
        #endregion

        #region 資料夾處理
        /// <summary>
        /// 建立資料夾(若已存在則略過動作)
        /// </summary>
        /// <param name="folderPath"></param>
        public static void CreateFolder(string folderPath)
        {
            if(!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }
        }
        #endregion

        #region 檔案大小格式化
        /// <summary>
        /// 將檔案大小格式化成(GB, MB或KB)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="decimalDigits"></param>
        /// <returns></returns>
        public static string FormatFilesize(int size, int decimalDigits = 0)
        {
            /*
             * decimalDigits: 小數點位數
             */
            int kb = 1024;
            int mb = 1024 * 1024;
            int gb = 1024 * 1024 * 1024;
            double dSize = size;
            double divided;
            if (size >= gb)
            {
                divided = Math.Round(dSize / gb, decimalDigits);
                return $"{divided} GB";
            }
            else if (size >= mb)
            {
                divided = Math.Round(dSize / mb, decimalDigits);
                return $"{divided} MB";
            }
            else
            {
                divided = Math.Round(dSize / kb, decimalDigits);
                return $"{divided} KB";
            }
        }
        /// <summary>
        /// 將檔案大小格式化成(GB, MB或KB)
        /// </summary>
        /// <param name="size"></param>
        /// <param name="decimalDigits"></param>
        /// <returns></returns>
        public static string FormatFilesize(object? size, int decimalDigits = 0)
        {
            if (size == null) { return "0"; }
            string strSize = size?.ToString() ?? "";
            int intSize;
            bool parseOK = int.TryParse(strSize, out intSize);
            if (parseOK)
            {
                return FormatFilesize(intSize, decimalDigits);
            }
            else
            {
                return "數字格式不正確";
            }

        }
        #endregion
    }
}

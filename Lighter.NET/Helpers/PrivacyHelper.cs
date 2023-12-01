using Lighter.NET.Common;

namespace Lighter.NET.Helpers
{
    /// <summary>
    /// 個資保護輔助函式
    /// </summary>
    public class PrivacyHelper
    {

        /// <summary>
        /// 遮罩字串尾段
        /// </summary>
        /// <param name="originalValue">原始值</param>
        /// <param name="maskCount">遮罩字元數(最小1個字元)</param>
        /// <param name="maskCharacter">遮罩字符(預設：*)</param>
        /// <returns></returns>
        public static string MaskEnd(object? originalValue, int maskCount, string maskCharacter = "*")
        {
            if (originalValue == null) return "";
            return MaskEnd(originalValue.ToString(), maskCount, maskCharacter);
        }
        /// <summary>
        /// 遮罩字串尾段
        /// </summary>
        /// <param name="originalValue">原始字串值</param>
        /// <param name="maskCount">遮罩字元數(最小1個字元)</param>
        /// <param name="maskCharacter">遮罩字符(預設：*)</param>
        /// <returns></returns>
        public static string MaskEnd(string? originalValue, int maskCount, string maskCharacter = "*")
        {
            if (string.IsNullOrWhiteSpace(originalValue)) { return ""; }
            if (string.IsNullOrWhiteSpace(maskCharacter)) { maskCharacter = "*"; }
            if (maskCount < 1) maskCount = 1;
            string maskString = maskCharacter.Repeat(maskCount);
            if (originalValue.Length > maskString.Length)
            {
                return originalValue.Substring(0, originalValue.Length - maskString.Length) + maskString;
            }
            else
            {
                return maskString;
            }
        }


        /// <summary>
        /// 遮罩字串頭段
        /// </summary>
        /// <param name="originalValue">原始值</param>
        /// <param name="maskCount">遮罩字元數(最小1個字元)</param>
        /// <param name="maskCharacter">遮罩字符(預設：*)</param>
        /// <returns></returns>
        public static string MaskStart(object? originalValue, int maskCount, string maskCharacter = "*")
        {
            if (originalValue == null) return "";
            return MaskStart(originalValue.ToString(), maskCount, maskCharacter);
        }

        /// <summary>
        /// 遮罩字串頭段
        /// </summary>
        /// <param name="originalValue">原始字串值</param>
        /// <param name="maskCount">遮罩字元數(最小1個字元)</param>
        /// <param name="maskCharacter">遮罩字符(預設：*)</param>
        /// <returns></returns>
        public static string MaskStart(string? originalValue, int maskCount, string maskCharacter = "*")
        {
            if (string.IsNullOrWhiteSpace(originalValue)) { return ""; }
            if (string.IsNullOrWhiteSpace(maskCharacter)) { maskCharacter = "*"; }
            if (maskCount < 1) maskCount = 1;
            string maskString = maskCharacter.Repeat(maskCount);
            if (originalValue.Length > maskString.Length)
            {
                return maskString + originalValue.Substring(maskString.Length);
            }
            else
            {
                return maskString;
            }
        }

        /// <summary>
        /// 遮罩字串中段
        /// </summary>
        /// <param name="originalValue">原始值</param>
        /// <param name="startIndex">啟始位置索引(zero-based index)</param>
        /// <param name="maskCount">遮罩字元數(最小1個字元)</param>
        /// <param name="maskCharacter">遮罩字符(預設：*)</param>
        /// <returns></returns>
        public static string MaskMiddle(object? originalValue, int startIndex, int maskCount, string maskCharacter = "*")
        {
            if (originalValue == null) return "";
            return MaskMiddle(originalValue.ToString(), startIndex, maskCount, maskCharacter);
        }

        /// <summary>
        /// 遮罩字串中段
        /// </summary>
        /// <param name="originalValue">原始字串值</param>
        /// <param name="startIndex">啟始位置索引(zero-based index)</param>
        /// <param name="maskCount">遮罩字元數(最小1個字元)</param>
        /// <param name="maskCharacter">遮罩字符(預設：*)</param>
        /// <returns></returns>
        public static string MaskMiddle(string? originalValue, int startIndex, int maskCount, string maskCharacter = "*")
        {
            if (string.IsNullOrWhiteSpace(originalValue)) { return ""; }
            if (string.IsNullOrWhiteSpace(maskCharacter)) { maskCharacter = "*"; }
            if (startIndex < 0) startIndex = 0;
            if (startIndex > originalValue.Length) startIndex = originalValue.Length - 1;
            if (maskCount < 1) maskCount = 1;
            string maskString = maskCharacter.Repeat(maskCount);
            if (originalValue.Length > maskString.Length)
            {
                string start = originalValue.Substring(0, startIndex);
                string end = "";
                if (startIndex + maskCount < originalValue.Length - 1)
                {
                    end = originalValue.Substring(startIndex + maskCount);
                }
                return start + maskString + end;
            }
            else
            {
                return maskString;
            }
        }

    }
}

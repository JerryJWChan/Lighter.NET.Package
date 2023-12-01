namespace Lighter.NET.Common
{
    /// <summary>
    /// 選單項目
    /// </summary>
    public class OptionItem
    {
        /*
         * 此類別用以替代SelectItem，提供選單項目的資料來源
         * 目的是不須引用System.Web.Mvc，以提高函式庫的通用性
         */
        public OptionItem() { }
        public OptionItem(string value)
        {
            Text = value;
            Value = value;
            Selected = false;
        }
        public OptionItem(string text, string value)
        {
            Text = text;
            Value = value;
            Selected = false;
        }

        public OptionItem(string text, string value, bool selected)
        {
            Text = text;
            Value = value;
            Selected = selected;
        }


        /// <summary>
        /// 選項文字
        /// </summary>
        public string Text { get; set; } = "";
        /// <summary>
        /// 選項值
        /// </summary>
        public string Value { get; set; } = "";
        /// <summary>
        /// 是否選定
        /// </summary>
        public bool Selected { get; set; } = false;
        /// <summary>
        /// 附帶資料名
        /// </summary>
        public string? ExtraDataName { get; set; }
		/// <summary>
		/// 附帶資料值
		/// </summary>
		public string? ExtraDataValue { get; set; }

    }
}

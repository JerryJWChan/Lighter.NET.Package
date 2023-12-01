
namespace Lighter.NET.Common
{
    public static class OptionItemExtension
    {
        /// <summary>
        /// 將指定值對應到具有該值的項目文字
        /// </summary>
        /// <param name="OptionList"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string MapToText(this List<OptionItem>? optionList, string? value)
        {
            string text = "";
            if (optionList == null)
            {
                text = $"value={value}選項不存在";
            }
            else
            {
                var item = optionList.FirstOrDefault(x => x.Value == value);
                if(item != null)
                {
                    text = item.Text;
                }
                else
                {
                    text = $"value={value}選項不存在";
                }
            }
            
            return text;
        }
        /// <summary>
        /// 將指定值對應到具有該值的項目文字
        /// </summary>
        /// <param name="OptionList"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string MapToText(this List<OptionItem>? optionList, int? value)
        {
            string? strValue = (value == null) ? "" : value.ToString();
            return MapToText(optionList, strValue);
        }

        /// <summary>
        /// 設定選取項目
        /// </summary>
        /// <param name="optionList"></param>
        /// <param name="selectedValue"></param>
        public static List<OptionItem> SetSelected(this List<OptionItem>? optionList, string? selectedValue)
        {
            if (optionList == null) optionList = new List<OptionItem>();
            var item = optionList.FirstOrDefault(x => x.Value == selectedValue);
            if (item != null)
            {
                item.Selected = true;
            }
            return optionList;
        }

        /// <summary>
        /// 設定選取項目
        /// </summary>
        /// <param name="optionList"></param>
        /// <param name="selectedValue"></param>
        public static List<OptionItem> SetSelected(this List<OptionItem>? optionList, int? selectedValue)
        {
            string? strValue = (selectedValue == null) ? "" : selectedValue.ToString();
            return SetSelected(optionList, strValue);
        }

        /// <summary>
        /// 設定選項排序(by值，順序)
        /// </summary>
        /// <param name="optionList"></param>
        public static List<OptionItem> OrderByValue(this List<OptionItem>? optionList)
        {
            if (optionList == null) optionList = new List<OptionItem>();
            optionList = optionList.OrderBy(x=>x.Value).ToList();   
            return optionList;
        }

        /// <summary>
        /// 設定選項排序(by值，反序)
        /// </summary>
        /// <param name="optionList"></param>
        public static List<OptionItem> OrderByValueDesc(this List<OptionItem>? optionList)
        {
            if (optionList == null) optionList = new List<OptionItem>();
            optionList = optionList.OrderByDescending(x => x.Value).ToList();
            return optionList;
        }
        /// <summary>
        /// 加入選項
        /// </summary>
        /// <param name="optionList"></param>
        /// <param name="text">選項文字</param>
        /// <param name="value">選項值</param>
        /// <returns></returns>
        public static List<OptionItem> AddOption(this List<OptionItem>? optionList,string text ,string value)
        {
            if (optionList == null) optionList = new List<OptionItem>();
            optionList.Add(new OptionItem(text,value));
            return optionList;
        }

        /// <summary>
        /// 加入選項
        /// </summary>
        /// <param name="optionList"></param>
        /// <param name="text">選項文字</param>
        /// <param name="value">選項值</param>
        /// <returns></returns>
        public static List<OptionItem> AddOption(this List<OptionItem>? optionList, string text, int value)
        {
            if (optionList == null) optionList = new List<OptionItem>();
            optionList.Add(new OptionItem(text, value.ToString()));
            return optionList;
        }

        /// <summary>
        /// 加入空白(值)選項
        /// </summary>
        /// <param name="optionList"></param>
        /// <param name="emptyOptionText">空白(值)選項文字</param>
        /// <param name="value">選項值(預設：空字串)</param>
        /// <returns></returns>
        public static List<OptionItem> AddEmptyOption (this List<OptionItem>? optionList,string emptyOptionText,string value="")
        {
            if (optionList == null) optionList = new List<OptionItem>();
            optionList.Insert(0, new OptionItem(emptyOptionText, value));
            return optionList;
        }

        /// <summary>
        /// 加入「全部」選項
        /// </summary>
        /// <param name="optionList"></param>
        /// <param name="emptyOptionText">選項文字</param>
        /// <param name="value">選項值</param>
        /// <returns></returns>
        public static List<OptionItem> AddAllOption(this List<OptionItem>? optionList, string allOptionText = "全部", string value = "")
        {
            if (optionList == null) optionList = new List<OptionItem>();
            optionList.Insert(0, new OptionItem(allOptionText, value));
            return optionList;
        }
    }
}

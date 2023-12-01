namespace Lighter.NET.Common
{
    public class YesNoOptions
    {
		public static List<OptionItem>YesNo(string yesValue , string noValue)  
        {
            return YesNo(false,"",yesValue,noValue);
        }

		public static List<OptionItem> YesNo(bool addEmptyOption=false, string emptyOptionText="", string yesValue = "1", string noValue = "0")
        {
            var options = new List<OptionItem>() {
                new OptionItem("Yes",yesValue),
                new OptionItem("No",noValue)
            };
            if(addEmptyOption)
            {
                options.Insert(0, new OptionItem(emptyOptionText, ""));
            }
            return options;
        }

		public static List<OptionItem> 是否(string yesValue, string noValue)
        {
            return 是否(false,"",yesValue,noValue);
		}

		public static List<OptionItem> 是否(bool addEmptyOption = false, string emptyOptionText = "",string yesValue="1", string noValue="0")
        {
            var options = new List<OptionItem>();
            var culture = LangHelper.GetCultureName();
            switch (culture)
            {
                case CultureName.Current:
                case CultureName.CT:
                default:
                    options.Add(new OptionItem("是", yesValue));
                    options.Add(new OptionItem("否", noValue));
                    break;
                case CultureName.CS:
                    options.Add(new OptionItem("是", yesValue));
                    options.Add(new OptionItem("否", noValue));
                    break;
                case CultureName.EN:
                    options.Add(new OptionItem("Yes", yesValue));
                    options.Add(new OptionItem("No", noValue));
                    break;
            }

            if (addEmptyOption)
            {
                options.Insert(0, new OptionItem(emptyOptionText, ""));
            }
            return options;
        }

		public static List<OptionItem> 啟用停用(string yesValue, string noValue)
        {
            return 啟用停用(false,"", yesValue, noValue);
		}

		public static List<OptionItem> 啟用停用(bool addEmptyOption = false, string emptyOptionText = "",string yesValue = "1", string noValue = "0")
        {
            var options = new List<OptionItem>();
            var culture = LangHelper.GetCultureName();
            switch (culture)
            {
                case CultureName.Current:
                case CultureName.CT:
                default:
                    options.Add(new OptionItem("啟用", yesValue));
                    options.Add(new OptionItem("停用", noValue));
                    break;
                case CultureName.CS:
                    options.Add(new OptionItem("启用", yesValue));
                    options.Add(new OptionItem("停用", noValue));
                    break;
                case CultureName.EN:
                    options.Add(new OptionItem("Enable", yesValue));
                    options.Add(new OptionItem("Disable", noValue));
                    break;
            }

            if (addEmptyOption)
            {
                options.Insert(0, new OptionItem(emptyOptionText, ""));
            }
            return options;
        }
    }
}

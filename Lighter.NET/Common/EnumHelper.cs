using System.Collections.Generic;
using System.Collections;

namespace Lighter.NET.Common
{
    public class EnumHelper
    {
        /// <summary>
        /// 確保解析並將字串轉換成Enum值
        /// </summary>
        /// <typeparam name="TEnum">Enum型別</typeparam>
        /// <param name="value">字串值</param>
        /// <returns></returns>
        public static TEnum SafeParse<TEnum>(object? objValue) where TEnum : struct, Enum
        {
            string value = objValue?.ToString() ?? "";
            if (string.IsNullOrEmpty(value)) return default(TEnum);

            bool result = Enum.TryParse<TEnum>(value, out TEnum enumItem);
            if (result == false) return enumItem; //enumItem
            /*
             * 針對result==true的情況
             * 若傳入的value是數字且不在TEnum的正常值範圍內時, result仍會是true
             * 故須再判斷IsDefined
             */

            if (Enum.IsDefined(typeof(TEnum), enumItem))
            {
                return enumItem;
            }
            else
            {
                return default(TEnum);
            }
        }
        /// <summary>
        /// 確保解析並將字串轉換成Enum值
        /// </summary>
        /// <typeparam name="TEnum">Enum型別</typeparam>
        /// <param name="value">字串值</param>
        /// <returns></returns>
        public static TEnum SafeParse<TEnum>(string? value) where TEnum : struct, Enum
        {
            if(string.IsNullOrEmpty(value)) return default(TEnum);

            bool result = Enum.TryParse<TEnum>(value, out TEnum enumItem);
            if (result == false) return enumItem; //enumItem
            /*
             * 針對result==true的情況
             * 若傳入的value是數字且不在TEnum的正常值範圍內時, result仍會是true
             * 故須再判斷IsDefined
             */

            if (Enum.IsDefined(typeof(TEnum), enumItem))
            {
                return enumItem;
            }
            else
            {
                return default(TEnum);
            }
        }
        /// <summary>
        /// 確保將整數值轉換成有效的指定型別Enum值
        /// </summary>
        /// <typeparam name="TEnum">Enum型別</typeparam>
        /// <param name="value">整數值</param>
        /// <returns></returns>
        public static TEnum SafeParse<TEnum>(int value) where TEnum : struct, Enum
        {
            TEnum enumItem = (TEnum)Enum.ToObject(typeof(TEnum), value);
            if (Enum.IsDefined(typeof(TEnum), enumItem))
            {
                return enumItem;
            }
            else
            {
                return default(TEnum);
            }           
        }

        /// <summary>
        /// 將Enum轉換成選項清單
        /// </summary>
        /// <param name="useStringValueArritbuteAsText">是否採用StringValueAttribute所指定的值(字串)作為選項文字(預設：false)</param>
        /// <param name="excludeUndefinedItem">是否排除Undefined或「未定義」的項目(預設:false)</param>
        /// <returns></returns>
        public static List<OptionItem> ConvertToOptionList<TEnum>(bool useStringValueArritbuteAsText = false, bool excludeUndefinedItem = false) where TEnum : struct, Enum
        {
            Type enumType = typeof(TEnum);
            if(!enumType.IsEnum)
            {
                throw new ArgumentException("enumType must be an type of Enum");
            }

            List<OptionItem> optionList;

            if (useStringValueArritbuteAsText)
            {
                optionList = Enum.GetValues(enumType).Cast<int>()
                             .Select(v => new OptionItem(EnumHelper.SafeParse<TEnum>(v).StringValue(), v.ToString()))
                             .ToList();
            }
            else
            {
                optionList = Enum.GetValues(enumType).Cast<int>()
                            .Select(v => new OptionItem(Enum.GetName(enumType, v) ?? "", v.ToString()))
                            .ToList();
            }

            if (excludeUndefinedItem)
            {
                optionList.Remove(x=>x.Text.ToLower() == "undefined" || x.Text == "未定義");
            }

            return optionList;  
        }

		/// <summary>
		/// 將Enum轉換成選項清單
		/// </summary>
		/// <typeparam name="TEnum"></typeparam>
		/// <param name="convertArgs">轉換參數</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static List<OptionItem> ConvertToOptionList<TEnum>(EnumConvertToOptionsArgs convertArgs) where TEnum : struct, Enum
		{
			Type enumType = typeof(TEnum);
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType must be an type of Enum");
			}

            var enumValueList = Enum.GetValues(enumType).Cast<int>();
            List<OptionItem> optionList = new List<OptionItem>();
            foreach (var v in enumValueList)
            {
                var enumItem = EnumHelper.SafeParse<TEnum>(v);
				var optionItem = new OptionItem();
				if (convertArgs.useLangAttributeAsText)
                {
                    optionItem.Text = enumItem.Lang().Current;
				}
				else if (convertArgs.useStringValueAttritbuteAsText)
                {
                    optionItem.Text = enumItem.StringValue();
				}
                else
                {
					optionItem.Text = enumItem.ToString();
				}

                if (convertArgs.useStringValueAttributeAsValue)
                {
                    optionItem.Value = enumItem.StringValue();
                }
                else
                {
					optionItem.Value = v.ToString();
				}

                if (convertArgs.useLangAttributeAsExtraData)
                {
                    optionItem.ExtraDataValue = enumItem.Lang().All();
                    optionItem.ExtraDataName = convertArgs.extraDataName;
                }
                optionList.Add(optionItem);
			}

			if (convertArgs.excludeUndefinedItem)
			{
				optionList.Remove(x => x.Text.ToLower() == "undefined" || x.Text == "未定義");
			}

			return optionList;
		}

        /// <summary>
        /// 取得TEnum型別的全部列舉項目
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<TEnum> GetEnumItemList<TEnum>() where TEnum : struct, Enum
        {
            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("enumType must be an type of Enum");
            }
            var enumItemList = Enum.GetValues(enumType).Cast<int>().Select(v=> EnumHelper.SafeParse<TEnum>(v)).ToList();
            return enumItemList;
        }
	}
}

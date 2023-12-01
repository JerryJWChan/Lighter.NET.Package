using System.Collections.Concurrent;
using System.Reflection;

namespace Lighter.NET.Common
{
	/// <summary>
	/// Enum延伸函式
	/// </summary>
	public static  class EnumExtension
	{
		#region Enum對照字串值
		/*此函式參考自Josip Miskovic，但將原使用DescriptionAttribute，改寫成自定義EnumStringValueAttribute*/
		private static readonly
		ConcurrentDictionary<string, string> _enumStringValueCache = new ConcurrentDictionary<string, string>();

		/// <summary>
		/// 列舉項目EnumStringValueAttribute標註所設定的字串值
		/// </summary>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		public static string StringValue(this Enum enumValue)
		{
			var key = $"{enumValue.GetType().FullName}.{enumValue}";

			var stringValue = _enumStringValueCache.GetOrAdd(key, x =>
			{
				string strValue;
				var attr = enumValue
					.GetType()
					.GetField(enumValue.ToString())?.GetCustomAttribute(typeof(EnumStringValueAttribute));
				if (attr != null)
				{
					strValue = ((EnumStringValueAttribute)attr).Value;
				}
				else
				{
					strValue = $"{key} has no EnumStringValueAttribute declaration.";
				}

				return strValue;
			});

			return stringValue;
		}

		#endregion

		#region Enum項目語系定義
		private static readonly
				ConcurrentDictionary<string, Lang> _enumLangCache = new ConcurrentDictionary<string, Lang>();


		/// <summary>
		/// 列舉項目LangAttribute標註所設定的語系對照
		/// </summary>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		public static Lang Lang(this Enum enumValue)
		{
			var key = $"{enumValue.GetType().FullName}.{enumValue}";

			var langItem = _enumLangCache.GetOrAdd(key, x =>
			{
				Lang lang;
				var attr = enumValue
					.GetType()
					.GetField(enumValue.ToString())?.GetCustomAttribute(typeof(LangAttribute));
				if (attr != null)
				{
					lang = ((LangAttribute)attr).Lang;
				}
				else
				{
					lang = new Lang() { CT = enumValue.ToString(), EN = enumValue.ToString(), CS = enumValue.ToString() };
				}

				return lang;
			});

			return langItem;
		}
		#endregion

		/// <summary>
		/// 將Enum值轉換成另一個Enum型別的值
		/// </summary>
		/// <param name="v">轉換前的Enum值</param>
		/// <typeparam name="TEnumResult">轉換後的Enum型別</typeparam>
		/// <returns>轉換後的Enum值</returns>
		public static TEnumResult ConvertTo<TEnumResult>(this Enum enumValue) where TEnumResult : struct, Enum
		{
			bool result = Enum.TryParse<TEnumResult>(enumValue.ToString(), out TEnumResult resultValue);
			if (result == false) return resultValue; //此處enumValue會是TEnum的預設值
			/*
             * 針對result==true的情況
             * 若傳入的enumValue是數字且不在TEnumResult的正常值範圍內時, result仍會是true
             * 故須再判斷IsDefined
             */

			if (Enum.IsDefined(typeof(TEnumResult), resultValue))
			{
				return resultValue;
			}
			else
			{
				return default(TEnumResult);
			}
		}

	}
}

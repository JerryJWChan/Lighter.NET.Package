namespace Lighter.NET.Common
{
    /// <summary>
    /// 語系文字對照項
    /// </summary>
    public class Lang
    {
        /// <summary>
        /// 列舉項目現在語系文字
        /// </summary>
        public string Current
        {
            get
            {
                return _GetCurrent();
            }
        }
        /// <summary>
        /// 繁體中文
        /// </summary>
        public string CT { get; set; } = "(未定義)";
        /// <summary>
        /// 英文
        /// </summary>
        public string EN { get; set; } = "(未定義)";
        /// <summary>
        /// 簡體中文
        /// </summary>
        public string CS { get; set; } = "(未定義)";
		private string _GetCurrent()
		{
			var cultureName = LangHelper.GetCultureName();
			switch (cultureName)
			{
				case CultureName.Current:
				case CultureName.CT:
				default:
					return CT;
				case CultureName.CS:
					return CS;
				case CultureName.EN:
					return EN;
			}
		}

		/// <summary>
		/// 全部語系文字(以分隔字元串接)(順序：CT,EN,CS)
		/// </summary>
		/// <param name="delimeter">字元串接(預設：逗號)</param>
		/// <returns></returns>
		public string All(char delimeter = ',')
        {
            return $"{CT}{delimeter}{EN}{delimeter}{CS}";
        }
	}
}

namespace Lighter.NET.Common
{
    /// <summary>
    /// 學年度
    /// </summary>
    public class AcademicYear
    {
        /// <summary>
        /// 學年值
        /// </summary>
        public int Value { get; private set; }
        /// <summary>
        /// 西元年
        /// </summary>
        public int WesternYear
        {
            get
            {
                return Value + 1911;
            }
        }
        /// <summary>
        /// 當前的學年
        /// </summary>
        public AcademicYear():this(DateTime.Today){}
        /// <summary>
        /// 某日期的學年
        /// </summary>
        /// <param name="theDate"></param>
        public AcademicYear(DateTime theDate)
        {
            int year = theDate.Year - 1911;
            int month = theDate.Month;
            int sem = month >= 8 || month <= 1 ? 1 : 2;
            year = sem == 1 ? year : year - 1;
            Value = year;
        }

        /// <summary>
        /// 某學年
        /// </summary>
        /// <param name="year"></param>
        public AcademicYear(int year) 
        {
            Value = year;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// 前一個學年
        /// </summary>
        /// <returns></returns>
        public AcademicYear Previous()
        {
            var previous = new AcademicYear(Value - 1);
            return previous;
        }

        /// <summary>
        /// 下一個學年
        /// </summary>
        /// <returns></returns>
        public AcademicYear Next()
        {
            var next = new AcademicYear(Value + 1);
            return next;
        }

        /// <summary>
        /// 產生學年度下拉選項
        /// </summary>
        /// <param name="beforeYearCount">往前幾個學年</param>
        /// <param name="afterYearCount">往後幾個學年</param>
        /// <param name="format">文字格式(例如："{0}學年度")</param>
        /// <param name="isCurrentSelected">是否預選當前學年</param>
        /// <returns></returns>
        public static List<OptionItem> CreateOptionItems(int beforeYearCount, int afterYearCount = 0, string format="",bool isCurrentSelected=true)
        {
            int start, end;
            bool selected = false;
            if (beforeYearCount <= 0) beforeYearCount = 3;
            if (afterYearCount <= 0) afterYearCount = 0;
            int current = new AcademicYear().Value;
            start = current - beforeYearCount;
            end = current + afterYearCount;
            List<OptionItem> list = new List<OptionItem>();
            if(format == "") format= "{0}";
            for(int i = start; i<=end; i++)
            {
                if (isCurrentSelected)
                {
                    selected = i == current;
                }

                list.Add(new OptionItem(string.Format(format, i),i.ToString(),selected));
            }
            return list;
        }
    }
}

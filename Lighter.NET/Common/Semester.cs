namespace Lighter.NET.Common
{
    public class Semester
    {
        /// <summary>
        /// 學期值
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// 當前學期
        /// </summary>
        public Semester() {
            int m = DateTime.Today.Month;
            if(m >= 8 || m <= 1)
            {
                Value = 1;
            }
            else
            {
                Value = 2;
            }
        }
        /// <summary>
        /// 學期
        /// </summary>
        /// <param name="value">學期值</param>
        /// <param name="text">學期文字(例如：上學期/下學期，第1學期/第2學期)</param>
        public Semester(int value) 
        { 
            if(value <= 0 || value > 2)
            {
                throw new ArgumentOutOfRangeException("The value argument must be 1 or 2");
            }
            Value = value; 
        }

        /// <summary>
        /// 下一個學期
        /// </summary>
        /// <returns></returns>
        public Semester Next()
        {
            int current = Value;
            current++;
            if(current > 2) current = 1;
            return new Semester(current);
        }

        /// <summary>
        /// 前一個學期
        /// </summary>
        /// <returns></returns>
        public Semester Previous()
        {
            int current = Value;
            current--;
            if (current < 1) current = 2;
            return new Semester(current);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// 將學期1,2,3...轉成A,B,C
        /// </summary>
        /// <returns></returns>
        public string ToABC()
        {
            int ascii = 65 + Value - 1;
            return ((char)ascii).ToString();
        }

        /// <summary>
        /// 產生學期下拉選項
        /// </summary>
        /// <param name="format">文字格式(例如："第{0}學期")</param>
        /// <param name="isCurrentSelected">是否預選當前期</param>
        /// <returns></returns>
        public static List<OptionItem> CreateOptionItems(string format = "", bool isCurrentSelected = true)
        {
            bool selected = false;
            int current = new Semester().Value;
            List<OptionItem> list = new List<OptionItem>();
            if (format == "") format = "{0}";
            for (int i = 1; i <= 2; i++)
            {
                if (isCurrentSelected)
                {
                    selected = i == current;
                }

                list.Add(new OptionItem(string.Format(format, i), i.ToString(), selected));
            }

            return list;
        }

    }
}

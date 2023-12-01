namespace Lighter.NET.Common
{
    /// <summary>
    /// 學年度-學期
    /// </summary>
    public class AcademicYearSemester
    {
        /// <summary>
        /// 學年(物件)
        /// </summary>
        public AcademicYear AcademicYear { get; set; }
        /// <summary>
        /// 學期(物件)
        /// </summary>
        public Semester Semester { get; set; }
        /// <summary>
        /// 學年(數字)
        /// </summary>
        public int Acy 
        {
            get
            {
                return AcademicYear.Value;
            } 
        }
        /// <summary>
        /// 學期(數字)
        /// </summary>
        public int Sem
        {
            get
            {
                return Semester.Value;
            }
        }
        /// <summary>
        /// 是否當前學年-學期
        /// </summary>
        public bool IsCurrent
        {
            get
            {
                return (Acy == new AcademicYear().Value && Sem == new Semester().Value);
            }
        }
        /// <summary>
        /// 當期學年度-學期
        /// </summary>
        public AcademicYearSemester()
        {
            AcademicYear = new AcademicYear();
            Semester = new Semester();
        }

        /// <summary>
        /// 特定學年度-學期
        /// </summary>
        /// <param name="acy">學年</param>
        /// <param name="sem">學期</param>
        public AcademicYearSemester(int acy, int sem)
        {
            AcademicYear = new AcademicYear(acy);
            Semester = new Semester(sem);
        }
        /// <summary>
        /// 往前推一個學期
        /// </summary>
        /// <returns></returns>
        public AcademicYearSemester PreviousSemester()
        {
            int acy = AcademicYear.Value;
            int sem = Semester.Previous().Value;
            if (sem == 2) acy--; //跨至前一個學年
            return new AcademicYearSemester(acy,sem);
        }
        /// <summary>
        /// 往後推一個學期
        /// </summary>
        /// <returns></returns>
        public AcademicYearSemester NextSemester()
        {
            int acy = AcademicYear.Value;
            int sem = Semester.Next().Value;
            if (sem == 1) acy++; //跨至下一個學年
            return new AcademicYearSemester(acy, sem);
        }

        /// <summary>
        /// 產生[學年度-學期]下拉選項
        /// </summary>
        /// <param name="beforeYearCount">往前幾個學年</param>
        /// <param name="afterYearCount">往後幾個後年</param>
        /// <param name="format">文字格式(例如："{0}學年度,第{1}學期")</param>
        /// <param name="isCurrentSelected">是否預選當前學年度-學期</param>
        /// <param name="autoAddSecondSemester">自動補足第2學期</param>
        /// <returns></returns>
        public static List<OptionItem> CreateOptionItems(int beforeYearCount, int afterYearCount = 0, string format = "", bool isCurrentSelected = true, bool autoAddSecondSemester=false)
        {
            bool selected = false;
            if (beforeYearCount <= 0) beforeYearCount = 3;
            if (afterYearCount <= 0) afterYearCount = 0;
            //當前學年
            int currentAcy = new AcademicYear().Value;
            //當前學期
            int currentSem = new Semester().Value;
            //起始學年
            int startYear = currentAcy - beforeYearCount;
            int yearCount = beforeYearCount + 1 + afterYearCount;
            //學期數
            int semCount = yearCount * 2;
            //自動補足第2學期處理(若autoAddSecondSemester==false，且afterYearCount == 0，且當前學期是第1學期，則不show出第2學期)
            if(autoAddSecondSemester == false && afterYearCount == 0 && currentSem == 1)
            {
                semCount--;
            }

            List<OptionItem> list = new List<OptionItem>();
            if (format == "") format = "{0}學年度,第{1}學期";
            //起始學年-學期
            var acySem = new AcademicYearSemester(startYear, 1);
            for (int i = 0; i < semCount; i++)
            {
                if (isCurrentSelected)
                {
                    selected = acySem.IsCurrent;
                }

                list.Add(new OptionItem(string.Format(format, acySem.Acy,acySem.Sem), $"{acySem.Acy},{acySem.Sem}", selected));
                acySem = acySem.NextSemester();
            }
            return list;
        }
    }
}

namespace Lighter.NET.Common
{
    /// <summary>
    /// 進度Model
    /// </summary>
    public class ProgressModel:StatusModel
    {
        private int _current;
        private DateTime _currentTime;
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartTime { get;set;}  
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime CurrentTime { get; }
        /// <summary>
        /// 總作業數
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 當前數
        /// </summary>
        public int Current 
        {
            get { return _current; }
            set 
            { 
                _current = value; 
                _currentTime = DateTime.Now;
            }
        }
        /// <summary>
        /// 進度百分比(%)
        /// </summary>
        public double Percent
        {
            get
            {
                if(Total==0) return 0;  
                var p  = Math.Floor(((double)Current/ Total)*100);
                return p;
            }
        }
        /// <summary>
        /// 耗時(ms)
        /// </summary>
        public double Duration
        {
            get 
            {
                return _currentTime.Subtract(StartTime).TotalMilliseconds;
            }
        }
        /// <summary>
        /// 剩餘時間
        /// </summary>
        public double Remain
        {
            get 
            { 
                if(Current==0) return -1;
                return (Duration / Current) * (Total - Current);
            }
        }
        /// <summary>
        /// 是否已完成(已進行到最後一項)
        /// </summary>
        public bool Complete
        {
            get 
            {
                return Current >= Total;
            }
        }
        public ProgressModel()
        {
            StartTime = DateTime.Now;
            _currentTime = DateTime.Now;
        }
        /// <summary>
        /// 複制當前狀態下的ProgressModel
        /// </summary>
        /// <returns></returns>
        public ProgressModel Clone() {
            ProgressModel cloned = new ProgressModel() {
                Caption = this.Caption,
                Current = this.Current,
                Total = this.Total,
                JobName = this.JobName,
                StartTime = this.StartTime,
                Status = this.Status,
                Text = this.Text,
                Type = this.Type,
                _current = this.Current,
                _currentTime = this.CurrentTime
            };
            return cloned;
        }
    }
}

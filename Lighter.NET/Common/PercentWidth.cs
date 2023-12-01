namespace Lighter.NET.Common
{
    /// <summary>
    /// 百分比寬度
    /// </summary>
    public class PercentWidth
    {
        private double _value = 0;
        public PercentWidth(double widthPercent)
        {
            _value= widthPercent;
        }
        public PercentWidth(string widthPercent)
        {
            if(!string.IsNullOrEmpty(widthPercent))
            {
                if (widthPercent.EndsWith('%'))
                {
                    widthPercent = widthPercent.TrimEnd('%');
                    _value= Convert.ToDouble(widthPercent);
                }
            }
        }

        /// <summary>
        /// 寬度值(單位：%)
        /// </summary>
        public double Value { 
            get
            {
                return _value;
            } 
        }

        /// <summary>
        /// 百分比表示式
        /// </summary>
        public string PercentExpression
        {
            get
            {
                return $"{_value}%";
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (!(obj is PercentWidth)) return false;
            var other = obj as PercentWidth;
            if (other == null) return false;
            return this.Value == other.Value;
        }

        public override string ToString()
        {
            return $"{_value}%";
        }

        public static bool operator ==(PercentWidth? w1, PercentWidth? w2)
        {
            if (w1 is null) return (w2 is null);
            return w1.Equals(w2);
        }
        public static bool operator !=(PercentWidth? w1, PercentWidth? w2)
        {
            if (w1 is null) return !(w2 is null);
            return !w1.Equals(w2);
        }

        public static PercentWidth? operator +(PercentWidth? w1, PercentWidth? w2)
        {
            if(w1 == null && w2 ==null) return null;
            if(w1 != null && w2 ==null) return w1;
            if(w2 != null && w1 ==null) return w2;
            return new PercentWidth(w1!.Value + w2!.Value);
        }

        public static PercentWidth? operator -(PercentWidth? w1, PercentWidth? w2)
        {
            if (w1 == null) throw (new ArgumentNullException("the left operant of the substraction operation cannot be null"));
            if (w2 == null) return w1;
            if(w1.Value < w2.Value) return new PercentWidth(0);
            return new PercentWidth(w1.Value-w2.Value);
        }

        public static implicit operator PercentWidth (string percentWidth)
        {
            return new PercentWidth(percentWidth);
        }
    }
}

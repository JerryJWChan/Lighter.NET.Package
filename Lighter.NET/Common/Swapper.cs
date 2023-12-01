namespace Lighter.NET.Common
{
    /// <summary>
    /// 變數(或數值)對調器
    /// </summary>
    public class Swapper<T>
    {
        private T _value1;
        private T _value2;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value1">對調前的變數(或數值)1</param>
        /// <param name="value2">對調前的變數(或數值)2</param>
        public Swapper(T value1, T value2)
        {
            T temp = value1;
            value1 = value2;
            value2 = temp;
            _value1 = value1;
            _value2 = value2;
        }
        /// <summary>
        /// 對調後的變數(或數值)1
        /// </summary>
        public T Value1 { get { return _value1; } }
        /// <summary>
        /// 對調後的變數(或數值)2
        /// </summary>
        public T Value2 { get { return _value2;} }
        
    }
}

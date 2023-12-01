namespace Lighter.NET.Common
{
    /// <summary>
    /// 尺寸(長x寬：整數)
    /// </summary>
    public class Size
    {

        public static Size _240x180 { get; } = new Size(240,180);
        public static Size _360x270 { get; } = new Size(360, 270);
        public static Size _480x360 { get; } = new Size(480, 360);
        public static Size _640x480 { get; } = new Size(640, 480);
        public static Size _960x720 { get; } = new Size(960, 720);
        public static Size _1024x768 { get; } = new Size(1024, 768);
        public static Size _1280x720 { get; } = new Size(1280, 720);
        public static Size _1280x960 { get; } = new Size(1280, 960);
        public int X { get; set; }
        public int Y { get; set; }
        public Size() { }
        public Size(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 轉成直立尺立
        /// </summary>
        /// <returns></returns>
        public Size Protrait()
        {
            var swap = new Swapper<int>(X, Y);
            X = swap.Value1;
            Y = swap.Value2;
            return this;
        }
    }

    /// <summary>
    /// 尺寸(長x寬：浮點數)
    /// </summary>
    public class SizeF
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}

using Lighter.NET.Common;

namespace Lighter.NET.UiComponents.Icon
{
    /// <summary>
    /// 生成svg圖示的html
    /// </summary>
    public class SvgIcon : UiElementBase
    {
        public override UiElementType ElementType => UiElementType.SvgIcon;
        /// <summary>
        /// 預設SVG圖示寬度
        /// </summary>
        public const int DEFAULT_SVG_WIDTH = 24;
        /// <summary>
        /// 預設SVG圖示高度
        /// </summary>
        public const int DEFAULT_SVG_HEIGHT = 24;
        /// <summary>
        /// 單位「組體」圖示線條邊框寬度
        /// </summary>
        public const double UNIT_BOLD_STROKE_WIDTH = 0.2;
        /// <summary>
        /// 對應到svg symbol定義的id屬性
        /// </summary>
        public virtual string SymbolId { get; set; } = "";
        /// <summary>
        /// svg symbol定義寬度(預設24px)
        /// </summary>
        public virtual int SymbolWidth { get; set; } = DEFAULT_SVG_WIDTH;
        /// <summary>
        /// svg symbol定義高度(預設24px)
        /// </summary>
        public virtual int SymbolHeight { get; set; } = DEFAULT_SVG_HEIGHT;
        /// <summary>
        /// Svg標準版本(1.1, 2.0, ...)(預設：1.1版)
        /// </summary>
        public virtual string Version { get; set; } = "1.1";
        /// <summary>
        /// 寬度(預設：0表示自動依scale=1計算)
        /// </summary>
        public virtual int Width { get; set; } = 0;
        /// <summary>
        /// 高度(預設：0表示自動依scale=1計算)
        /// </summary>
        public virtual int Height { get; set; } = 0;
        /// <summary>
        /// 尺寸縮放比例(預設原圖大小)
        /// </summary>
        public virtual double Scale { get; set; } = 1.0;
        /// <summary>
        /// 填入顏色(預設空值：表示自動繼承容器元素的顏色)
        /// </summary>
        public string Fill { get; set; } = "";
        /// <summary>
        /// 邊框顏色(預設：無邊框)
        /// </summary>
        public string Stroke { get; set; } = "";
        /// <summary>
        /// 邊框粗細
        /// </summary>
        public double StrokeWidth
        {
            get { return UNIT_BOLD_STROKE_WIDTH * BoldLevel; }
        }
        /// <summary>
        /// 線條邊框粗細等級(預設0:無邊框)
        /// </summary>
        public int BoldLevel { get; set; } = 0;
        /// <summary>
        /// 是否顯示粗體圖示
        /// </summary>
        public bool IsBold 
        {
            get
            {
                return (BoldLevel > 0);
            }
        }

        public SvgIcon() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolId">參照的svg symbol定義的id屬性</param>
        public SvgIcon(string symbolId)
        {
            SymbolId = symbolId;
        }

        /// <summary>
        /// 設定縮放倍數
        /// </summary>
        /// <param name="scale">縮放倍數</param>
        /// <returns></returns>
        public SvgIcon SetScale(double scale)
        {
            if (scale > 0) { Scale = scale; }
            return this;
        }

        /// <summary>
        /// 設定填入顏色
        /// </summary>
        /// <param name="fillColor">顏色</param>
        /// <returns></returns>
        public SvgIcon SetColor(string fillColor)
        {
            if (!string.IsNullOrEmpty(fillColor)) { Fill = fillColor; }
            return this;
        }

        /// <summary>
        /// 設定粗體
        /// </summary>
        /// <param name="boldLevel">粗體等級(數字越大線條邊框越粗)</param>
        /// <param name="bolderColor">邊框顏色</param>
        /// <returns></returns>
        public SvgIcon SetBold(int boldLevel = 1, string bolderColor = "")
        {
            if (boldLevel < 0) boldLevel = 0;
            BoldLevel = boldLevel;
            if(!string.IsNullOrEmpty(bolderColor)) { Stroke= bolderColor; }
            return this;
        }

        public override string Render(IUiElement? setting)
        {
            ApplySetting(setting);

            int width = CalWidth();
            int height = CalHeight();
            string fillAttr = MakeFillAttribute();
            string strokeAttr = MakeStrokeAttribute();
            string startTag = $"<svg width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {SymbolWidth} {SymbolHeight}\" {fillAttr} {strokeAttr}>";
            string useTag = $"<use href=\"#{SymbolId.TrimStart('#')}\" />";
            string endTag = $"</svg>";
            string html = $"{startTag}{useTag}{endTag}";
            return html;
        }

        /// <summary>
        /// 計算圖示Width屬性值
        /// </summary>
        /// <returns></returns>
        private int CalWidth()
        {
            int width = DEFAULT_SVG_WIDTH;
            /*有設定SvgScale屬性，則優先以scale計算*/
            if (Scale > 0)
            {
                if (Width == 0)
                {
                    width = Convert.ToInt32(SymbolWidth * Scale);
                }
                else
                {
                    width = Width;
                }
            }
            return width;
        }

        /// <summary>
        /// 計算圖示Height屬性值
        /// </summary>
        /// <returns></returns>
        private int CalHeight()
        {
            int height = DEFAULT_SVG_HEIGHT;
            /*有設定SvgScale屬性，則優先以scale計算*/
            if (Scale > 0)
            {
                if (Height == 0)
                {
                    height = Convert.ToInt32(SymbolHeight * Scale);
                }
                else
                {
                    height = Height;
                }
            }
            return height;
        }

        /// <summary>
        /// 產生stroke屬性
        /// </summary>
        /// <returns></returns>
        private string MakeStrokeAttribute()
        {
            string strokeAttr = "";
            if (!string.IsNullOrEmpty(Stroke))
            {
                strokeAttr += $"stroke=\"{Stroke}\"";
            }

            if (BoldLevel > 0)
            {
                strokeAttr += $" stroke-width=\"{StrokeWidth}\"";
            }

            return strokeAttr;
        }

        /// <summary>
        /// 產生fill屬性
        /// </summary>
        /// <returns></returns>
        private string MakeFillAttribute()
        {
            string fillAttr = "";
            if (!string.IsNullOrEmpty(Fill))
            {
                fillAttr += $"fill=\"{Fill}\"";
            }

            return fillAttr;
        }
    }
}

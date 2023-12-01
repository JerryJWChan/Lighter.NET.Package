using Lighter.NET.Common;
using Lighter.NET.UiComponents.Icon;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// Html tag 產生器
    /// </summary>
    public class TagMaker
    {
		/// <summary>
		/// Html tag 產生器
		/// </summary>
		public TagMaker() { }

        #region Render Tag

        /// <summary>
        /// 生成指定的Html Tag
        /// </summary>
        /// <typeparam name="TTag">Tag型別</typeparam>
        /// <param name="tagSetter">html tag的參數設定</param>
        /// <returns></returns>
        public IHtmlContent Tag<TTag>(Action<TTag>? tagSetter) where TTag : IUiElement, new()
        {
            TTag tag = new TTag();

            if (tagSetter != null)
            {
                tagSetter(tag);
            }
            return Tag(tag, default);
        }

        /// <summary>
        /// 將指定的屬性，依指定的格式，生成指定的Html Tag
        /// </summary>
        /// <typeparam name="TTag">Tag型別</typeparam>
        /// <param name="selector">Model屬性選擇子</param>
        /// <param name="format">顯示格式</param>
        /// <param name="tagSetter">html tag的參數設定</param>
        /// <returns></returns>
        public IHtmlContent Tag<TTag>(FormatEnum format, Action<TTag>? tagSetter = null) where TTag : IUiElement, new()
        {
            TTag tag = new TTag();
            if (tagSetter != null)
            {
                tagSetter(tag);
            }
            string formatStr = format.StringValue();
            return Tag(tag, default, formatStr);
        }

        /// <summary>
        /// 生成指定的Html Tag，若是「選項類」的元素，則直接套用傳入的選項參數，若是其他類的元素，則將model的值對照成選項的顯示文字後，指定給html tag的value
        /// </summary>
        /// <typeparam name="TTag">Tag型別</typeparam>
        /// <param name="selector">Model屬性選擇子</param>
        /// <param name="optionItems">選項資料</param>
        /// <param name="tagSetter">html tag的參數設定</param>
        /// <returns></returns>
        public IHtmlContent Tag<TTag>(List<OptionItem>? optionItems, Action<TTag>? tagSetter = null) where TTag : IUiElement, new()
        {
            TTag tag = new TTag();
            if (tagSetter != null)
            {
                tagSetter(tag);
            }

            if (optionItems == null) optionItems = new List<OptionItem>();
            if (tag is IOptionElement)
            {
                var selectTag = tag as IOptionElement;
                selectTag!.Options = optionItems;
                return Tag(tag, default);
            }
            else
            {
                throw new Exception("optionItems參數只適用於有實作IOptionElement的tag元素");
            }
        }


        /// <summary>
        /// 生成html tag
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="metaProp">model屬性資訊</param>
        /// <param name="tag">html tag物件</param>
        /// <param name="setting">tag設定參數</param>
        /// <param name="format">值顯示格式</param>
        /// <returns></returns>
        public IHtmlContent Tag<TTag>(TTag tag, TTag? setting, string format = "") where TTag : IUiElement, new()
        {

            if (!string.IsNullOrEmpty(tag.Value))
            {
                if (format != "")
                {
                    tag.Value = string.Format($"{{0:{format}}}", tag.Value);
                }
            }

            string html = tag.Render(setting);
            return new HtmlString(html);
        }

        #endregion

        #region Render Icon
        /// <summary>
        /// 生成svg圖示
        /// </summary>
        /// <param name="svgIcon"></param>
        /// <param name="fillColor">線條色彩</param>
        /// <param name="scale">縮放等級(1.0表示原寸，小於1.0表示縮小倍數，大於1.0表示放大倍數)</param>
        /// <param name="isBold">是否組體</param>
        /// <returns></returns>
        public IHtmlContent Icon(SvgIcon svgIcon, string fillColor = "white", double scale = 1.0, bool isBold = false)
        {
            svgIcon.Fill = fillColor;
            if (scale > 0 && scale != 1.0)
            {
                svgIcon.Scale = scale;
            }
            if (isBold)
            {
                svgIcon.Stroke = fillColor;
            }
            return new HtmlString(svgIcon.Render());
        }

        #endregion

    }
}

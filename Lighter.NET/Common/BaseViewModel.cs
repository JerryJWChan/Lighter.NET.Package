using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lighter.NET.Common
{
    /// <summary>
    /// ViewModel基底類別(用於傳遞參數給Layout)
    /// </summary>
    public class BaseViewModel
    {
        /// <summary>
        /// 視窗標題
        /// </summary>
        public string WindowTitle { get; set; } = "";
        /// <summary>
        /// 頁面標題
        /// </summary>
        public string PageTitle { get; set; } = "";
        /// <summary>
        /// 網頁關鍵字(for無障礙 and SEO)
        /// </summary>
        public string PageKeywords { get; set; } = "";
        /// <summary>
        /// 網頁描述(for無障礙 and SEO)
        /// </summary>
        public string PageDescription { get; set; } = "";
        /// <summary>
        /// 操作說明網址
        /// </summary>
        public string HelpUrl { get; set; } = "";
        /// <summary>
        /// 套用頂層Layout(_Layout)(預設：true)
        /// </summary>
        public bool UseRootLayout { get; set; } = true;
        /// <summary>
        /// 頁面自訂選單(li > ul > a 階層式)
        /// </summary>
        public IHtmlContent? CustomMenu { get; set; }
        /// <summary>
        /// 頁面自訂Link引用(for CSS or CDN)
        /// </summary>
        public List<HtmlLinkElement> CustomLinks { get; set; } = new List<HtmlLinkElement>();
        /// <summary>
        /// 上方頁面自訂Script引用(for Javascript)
        /// </summary>
        public List<HtmlScriptElement> CustomScripts_Top { get; set; } = new List<HtmlScriptElement>();
        /// <summary>
        /// 下方頁面自訂Script引用(for Javascript)
        /// </summary>
        public List<HtmlScriptElement> CustomScripts_Bottom { get; set; } = new List<HtmlScriptElement>();
        /// <summary>
        /// 表單參數
        /// </summary>
        public FormActionArgs? FormActionArgs { get; set; }
        /// <summary>
        /// 要傳遞給Layout的參數物件
        /// </summary>
        public object? LayoutArgs { get; set; }

        /// <summary>
        /// 加入頁面自訂Link引用url
        /// </summary>
        /// <param name="url">Link引用url(例如：CSS檔所在相對路徑)</param>
        /// <returns></returns>
        public BaseViewModel AddCustomLink(string url, string rel="stylesheet")
        {
            CustomLinks.Add(new HtmlLinkElement() { href=url, rel=rel});
            return this;
        }

        /// <summary>
        /// 加入頁面自訂Link引用
        /// </summary>
        /// <param name="linkElement">Link引用元素</param>
        /// <returns></returns>
        public BaseViewModel AddCustomLink(HtmlLinkElement linkElement)
        {
            CustomLinks.Add(linkElement);
            return this;
        }

        /// <summary>
        /// 加入頁面自訂Script引用url
        /// </summary>
        /// <param name="url">Script引用url(例如：JS檔所在相對路徑)</param>
        /// <param name="placement">Script引用放置區域(Top(上方)是指放置在html head區塊; Bottom(下方)是指放置在html body結尾處)</param>
        /// <returns></returns>
        public BaseViewModel AddCustomScript(string url, Placement placement = Placement.Top)
        {
            if(placement != Placement.Top) placement = Placement.Bottom; //只要不是指定在上方的，一律都放置在下方
            if(placement == Placement.Top)
            {
                CustomScripts_Top.Add(new HtmlScriptElement() { src = url });
            }
            if(placement == Placement.Bottom)
            {
                CustomScripts_Bottom.Add(new HtmlScriptElement() { src = url });
            }
            return this;
        }

        /// <summary>
        /// 加入頁面自訂Script引用
        /// </summary>
        /// <param name="scriptElement">Script引用元素</param>
        /// <param name="placement">Script引用放置區域(Top(上方)是指放置在html head區塊; Bottom(下方)是指放置在html body結尾處)</param>
        /// <returns></returns>
        public BaseViewModel AddCustomScript(HtmlScriptElement scriptElement, Placement placement = Placement.Top)
        {
            if (placement != Placement.Top) placement = Placement.Bottom; //只要不是指定在上方的，一律都放置在下方
            if (placement == Placement.Top)
            {
                CustomScripts_Top.Add(scriptElement);
            }
            if (placement == Placement.Bottom)
            {
                CustomScripts_Bottom.Add(scriptElement);
            }
            return this;
        }

        /// <summary>
        /// 從傳入的ViewModel中取得BaseViewModel，若ViewModel非BaseViewModel的子類，則new一個回傳
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BaseViewModel Get(object? viewModel)
        {
            BaseViewModel? baseModel = null;
            if (viewModel != null && viewModel.GetType().IsClass)
            {
                baseModel = viewModel as BaseViewModel;
            }
            if (baseModel == null) baseModel = new BaseViewModel();
            return baseModel;
        }
    }

}

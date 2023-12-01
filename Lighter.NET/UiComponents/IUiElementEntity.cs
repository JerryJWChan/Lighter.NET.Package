using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// UI元素介面(定義要在UI(例如MVC的View)介面中render出來的UI元素的基本規格)，只包含屬性
    /// </summary>
    public interface IUiElementEntity
    {
        #region Properties[屬性]
        /// <summary>
        /// 查詢項類別(要生成哪一種html <input> tag)
        /// </summary>
        UiElementType ElementType { get; }
        /// <summary>
        /// 元素(即其.Value屬性)的資料型別
        /// </summary>
        Type? DataType { get; set; }
        /// <summary>
        /// 元素值顯示格式
        /// </summary>
        string DataFormat { get; set; }
        /// <summary>
        /// 生成html tag時是否包含id屬性，預設true
        /// </summary>
        bool WithId { get; set; }
        /// <summary>
        /// 對應至 html: id屬性(預設同Name)
        /// (若是多選的輸入項(例如CheckBoxList)，每個選項的Name相同，但html tag的id不可重複，故於實作render時「選項」只用Name不使用id,
        /// 或是將id加上後綴詞後(例如：[Name]_table, [Name]_group, [Name]_container...)，給容器tag使用)
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 對應至 html: name屬性
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 對應至 html:value屬性
        /// </summary>
        string Value { get; set; }
        /// <summary>
        /// 是否表單欄位元素
        /// </summary>
        bool IsFormFieldElement { get; set; }
        /// <summary>
        /// 是否樣版欄位(無id, name屬性，有data-field-name, data-type屬性)
        /// </summary>
        bool IsTemplateField { get; set; }
        /// <summary>
        /// 是否必填欄位
        /// </summary>
        bool IsRequired { get; set; }
        /// <summary>
        /// 是否唯讀
        /// </summary>
        bool IsReadOnly { get; set; }
        /// <summary>
        /// 對應至 html: class屬性
        /// </summary>
        string CssClass { get; set; }
        /// <summary>
        /// CSS inline style
        /// </summary>
        string Style { get; set; }
        /// <summary>
        /// 除了基本id,name,class,style,onEvent事件handler以外的其他html element屬性
        /// </summary>
        List<AttributeItem> Attributes { get; set; }
        /// <summary>
        /// 啟用HtmlEncode
        /// </summary>
        bool EnableHtmlEncode { get; set; }
        /// <summary>
        /// 元素值轉換器
        /// </summary>
        Func<object?, object>? ValueConverter { get; set; }
        /// <summary>
        /// Model屬性的描述資料
        /// </summary>
        MetaProperty? ModelMetaProperty { get; set; }

        #endregion
    }
}

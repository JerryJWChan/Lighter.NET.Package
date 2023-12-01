using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;
using Lighter.NET.UiComponents;
using Lighter.NET.UiComponents.Icon;

namespace Lighter.NET.Common
{
    /// <summary>
    /// Model打包器，提供將指定的Model屬性，Render成指定的HtmlTag的功能
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class RenderableModel<TModel> where TModel : IMetaModel, new()
    {
        private IStringLocalizer<TModel>? _localizer;
        /// <summary>
        /// Model的全部「帶值」屬性資訊
        /// </summary>
        public List<MetaProperty> MetaProperties { get; } = new List<MetaProperty>();

        /// <summary>
        /// 要生成Html的Model
        /// </summary>
        public TModel Model { get; set; }
        /// <summary>
        /// 生成html tag時包含id屬性(預設true:包含)
        /// </summary>
        public bool WithId { get; set; } = true;
        /// <summary>
        /// 生成html tag時包含Name屬性(預設true:包含)
        /// </summary>
        public bool WithName { get; set; } = true;
        /// <summary>
        /// Label文字大小
        /// </summary>
        public CssClassSize LabelTextSize { get; set; } = CssClassSize.Undefined;
        /// <summary>
        /// 輸入框文字大小
        /// </summary>
        public CssClassSize InputTextSize { get; set; } = CssClassSize.Undefined;

        #region 初始化
        /// <summary>
        /// Model打包器，提供將指定的Model屬性，Render成指定的HtmlTag的功能
        /// </summary>
        /// <param name="model"></param>
        public RenderableModel(TModel? model)
        {
            if (model == null) { model = new TModel(); }
            Model = model;
            try
            {
                ////多語系字串轉換器
                //var localizeOptions = Options.Create(new LocalizationOptions() { ResourcesPath = "Resources" });
                //var nullLogger = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory();
                //var factory = new ResourceManagerStringLocalizerFactory(localizeOptions, nullLogger);
                //_localizer = new StringLocalizer<TModel>(factory);
                _localizer = LangHelper.GetLocalizer<TModel>();
            }
            catch (Exception ex)
            {
                Console.Write($"RenderableModel() create StringLocalizer failed. {ex.Message}");
            }

            InitMetaProperties(model);
        }

        /// <summary>
        /// Model打包器，提供將指定的Model屬性，Render成指定的HtmlTag的功能
        /// </summary>
        /// <param name="model"></param>
        /// <param name="localizer">多語系字串轉換器</param>
        public RenderableModel(TModel? model, IStringLocalizer<TModel> localizer)
        {
            if (model == null) { model = new TModel(); }
            Model = model;
            _localizer= localizer;
            InitMetaProperties(model);

        }

        /// <summary>
        /// 初始化MetaProperties
        /// </summary>
        private void InitMetaProperties(TModel model)
        {
            /*將Model的PropertyInfo和屬性值備妥*/
            Type thisType = model.GetType(); //取執行階段型別
            var props = thisType.GetProperties();
            foreach (var p in props)
            {
                MetaProperties.Add(new MetaProperty(p, p.GetValue(model)));
            }
        }

        #endregion

        #region Value
       /// <summary>
        /// 指定的欄位是否有值
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool HasValue(Expression<Func<TModel, object?>> selector)
        {
            string propName = selector.GetLambdaPropertyName();
            if (propName == "") throw new ArgumentException($"[{selector.ToString()}]語法錯誤");

            var metaProp = MetaProperties.FirstOrDefault(x => x.PropertyInfo.Name == propName);
            if (metaProp == null) throw new ArgumentException($"[{selector.ToString()}]無此欄位");

            bool hasValue = metaProp.Value != null 
                            && string.IsNullOrEmpty(metaProp.Value?.ToString()??"") == false;
            return hasValue;
        }

        /// <summary>
        /// 取得指定的欄位值
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string Value(Expression<Func<TModel, object?>> selector)
        {
            string propName = selector.GetLambdaPropertyName();
            if (propName == "") throw new ArgumentException($"[{selector.ToString()}]語法錯誤");

            var metaProp = MetaProperties.FirstOrDefault(x => x.PropertyInfo.Name == propName);
            if (metaProp == null) throw new ArgumentException($"[{selector.ToString()}]無此欄位");

            return metaProp.Value?.ToString() ?? "";
        }

        #endregion
 
        #region Render Tag

        /// <summary>
        /// 將指定的屬性，Render成指定的HtmlTag
        /// </summary>
        /// <typeparam name="TTag">繼承IUiElement的HtmlTag類別</typeparam>
        /// <param name="selector">屬性選擇子</param>
        public IHtmlContent Tag<TTag>(Expression<Func<TModel, object?>> selector) where TTag : IUiElement, new()
        {
            TTag tag = new TTag();
            TTag? setting = default(TTag);
            return Tag(selector, tag, setting);
        }

        /// <summary>
        /// 將指定的屬性，生成指定的Html Tag
        /// </summary>
        /// <typeparam name="TTag">Tag型別</typeparam>
        /// <param name="selector">Model屬性選擇子</param>
        /// <param name="tagSetter">html tag的參數設定</param>
        /// <returns></returns>
        public IHtmlContent Tag<TTag>(Expression<Func<TModel, object?>> selector,Action<TTag> tagSetter) where TTag : IUiElement, new()
        {
            TTag tag = new TTag();
            
            if(tagSetter != null)
            {
                tagSetter(tag);
            }
            return Tag(selector, tag, default);
        }

        /// <summary>
        /// 將指定的屬性，依指定的格式，生成指定的Html Tag
        /// </summary>
        /// <typeparam name="TTag">Tag型別</typeparam>
        /// <param name="selector">Model屬性選擇子</param>
        /// <param name="format">顯示格式</param>
        /// <param name="tagSetter">html tag的參數設定</param>
        /// <returns></returns>
        public IHtmlContent Tag<TTag>(Expression<Func<TModel, object?>> selector, FormatEnum format, Action<TTag>? tagSetter = null) where TTag : IUiElement, new()
        {
            string propName = selector.GetLambdaPropertyName();
            if (propName == "") return new HtmlString($"[{selector.ToString()}]語法錯誤");

            var metaProp = MetaProperties.FirstOrDefault(x => x.PropertyInfo.Name == propName);
            if (metaProp == null) return new HtmlString($"[{selector.ToString()}]無此欄位");

            TTag tag = new TTag();
            if (tagSetter != null)
            {
                tagSetter(tag);
            }
            string formatStr = format.StringValue();
            return Tag(metaProp, tag, default, formatStr);
        }

        /// <summary>
        /// 生成指定的Html Tag，若是「選項類」的元素，則直接套用傳入的選項參數，若是其他類的元素，則將model的值對照成選項的顯示文字後，指定給html tag的value
        /// </summary>
        /// <typeparam name="TTag">Tag型別</typeparam>
        /// <param name="selector">Model屬性選擇子</param>
        /// <param name="optionItems">選項資料</param>
        /// <param name="tagSetter">html tag的參數設定</param>
        /// <returns></returns>
        public IHtmlContent Tag<TTag>(Expression<Func<TModel, object?>> selector, List<OptionItem>? optionItems, Action<TTag>? tagSetter = null) where TTag : IUiElement, new()
        {
            TTag tag = new TTag();
            if(tagSetter != null)
            {
                tagSetter(tag);
            }

            if(optionItems == null) optionItems =  new List<OptionItem>();
            if (tag is IOptionElement)
            {
                var selectTag = tag as IOptionElement;
                selectTag!.Options = optionItems;
                return Tag(selector, tag, default);
            }
            else
            {
                string propName = selector.GetLambdaPropertyName();
                if (propName == "") return new HtmlString($"[{selector.ToString()}]語法錯誤");

                var metaProp = MetaProperties.FirstOrDefault(x => x.PropertyInfo.Name == propName);
                if (metaProp == null) return new HtmlString($"[{selector.ToString()}]無此欄位");

                tag.Value = optionItems.MapToText(metaProp.Value?.ToString()??"");
                return Tag(metaProp, tag, default);

            }
        }

        /// <summary>
        /// 將指定的屬性，Render成指定的HtmlTag
        /// </summary>
        /// <param name="selector">屬性選擇子</param>
        /// <param name="tag">繼承IUiElement的HtmlTag物件</param>
        /// <param name="setting">參數設定，傳入與TTag相同型別的物件，其中有標註[SettingProperty]的屬性值會被套用到生成的HtmlTag</param>
        /// <param name="value">指定HtmlTag value(預設採用Model的屬性值)</param>
        /// <param name="required">是否必填欄位</param>
        /// <param name="isReadonly">是否唯讀欄位</param>
        public IHtmlContent Tag<TTag>(Expression<Func<TModel, object?>> selector, TTag tag, TTag? setting) where TTag : IUiElement, new()
        {
            string propName = selector.GetLambdaPropertyName();
            if (propName == "") return new HtmlString($"[{selector.ToString()}]語法錯誤");

            var metaProp = MetaProperties.FirstOrDefault(x => x.PropertyInfo.Name == propName);
            if (metaProp == null) return new HtmlString($"[{selector.ToString()}]無此欄位");

            return Tag(metaProp, tag, setting);
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
        public IHtmlContent Tag<TTag>(MetaProperty metaProp, TTag tag, TTag? setting, string format = "") where TTag : IUiElement, new()
        {
           //套入資料欄位屬性
            if(WithName && string.IsNullOrEmpty(tag.Name))
            {
                tag.Name = metaProp.PropertyInfo.Name;
            }
            if(WithId && string.IsNullOrEmpty(tag.Id))
            {
                tag.Id = tag.Name;
            }
            if(string.IsNullOrEmpty(tag.Value))
            {
                if(format == "")
                {
                    tag.Value = metaProp.Value?.ToString() ?? "";
                }
                else
                {
                    tag.Value = string.Format($"{{0:{format}}}", metaProp.Value);
                }
            }
            
            tag.DataType = metaProp.PropertyInfo.PropertyType;
            tag.ModelMetaProperty = metaProp;

            //若未指定Required屬性，則以Model中的定義
            //某些情況下，雖在Model中未定義Required,但在特定的填表程序中卻是必填欄位
            if (tag.IsRequired == false)
            {
                tag.IsRequired = metaProp.IsRequired;
            }

            //針對Label和LabelS Tag處理語系轉換
            if(tag.ElementType == UiElementType.Label)
            {
                if(_localizer != null)
                {
                    var label = tag as Label;
                    if(label != null)
                    {
                        string displayName = label.GetDisplayName();
                        string localizedText = _localizer.GetString(displayName);
                        label.LocalizedText= localizedText;
                    }
                }
            }

            //針對Text Tag處理語系轉換
            if (tag.ElementType == UiElementType.TextBox)
            {
                if (_localizer != null)
                {
                    var text = tag as Text;
                    if (text != null)
                    {
                        string placeHolderText = text.GetPropertyDescription();
                        string localizedText = _localizer.GetString(placeHolderText);
                        text.LocalizedPlaceholderText = localizedText;
                    }
                }
            }

            //針對TextArea Tag處理語系轉換
            if (tag.ElementType == UiElementType.TextArea)
            {
                if (_localizer != null)
                {
                    var textArea = tag as TextArea;
                    if (textArea != null)
                    {
                        string placeHolderText = textArea.GetPropertyDescription();
                        string localizedText = _localizer.GetString(placeHolderText);
                        textArea.LocalizedPlaceholderText = localizedText;
                    }
                }
            }

            if(tag.ElementType == UiElementType.Label)
            {
                if(LabelTextSize != CssClassSize.Undefined)
                {
                    tag.AddClass($"font-{LabelTextSize.ToString().ToLower()}");
                }
            }
            else
            {
                if (InputTextSize != CssClassSize.Undefined)
                {
                    tag.AddClass($"font-{InputTextSize.ToString().ToLower()}");
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
        public IHtmlContent Icon(SvgIcon svgIcon, string fillColor="white", double scale = 1.0,bool isBold=false)
        {
            svgIcon.Fill = fillColor;
            if(scale > 0 && scale != 1.0)
            {
                svgIcon.Scale = scale;
            }
            if (isBold)
            {
                svgIcon.Stroke = fillColor;
            }
            return new HtmlString(svgIcon.Render());
        }

        /// <summary>
        /// 生成svg圖示
        /// </summary>
        /// <param name="selector">屬性選擇器(用以指定狀態值)</param>
        /// <param name="statefulSvgIcon">可呈現不同狀態的svg icon</param>
        /// <param name="fillColor">線條色彩</param>
        /// <param name="scale">縮放等級(1.0表示原寸，小於1.0表示縮小倍數，大於1.0表示放大倍數)</param>
        /// <param name="isBold">是否組體</param>
        /// <returns></returns>
        public IHtmlContent Icon(Expression<Func<TModel, object?>> selector, StatefulSvgIconBase statefulSvgIcon, string fillColor = "white", double scale = 1.0, bool isBold = false)
        {
            if(statefulSvgIcon == null) return new HtmlString($"[statefulSvgIcon]參數為null");
            string propName = selector.GetLambdaPropertyName();
            if (propName == "") return new HtmlString($"[{selector.ToString()}]語法錯誤");

            var metaProp = MetaProperties.FirstOrDefault(x => x.PropertyInfo.Name == propName);
            if (metaProp == null) return new HtmlString($"[{selector.ToString()}]無此欄位");

            statefulSvgIcon.Fill = fillColor;
            if (scale > 0 && scale != 1.0)
            {
                statefulSvgIcon.Scale = scale;
            }
            if (isBold)
            {
                statefulSvgIcon.Stroke = fillColor;
            }
            statefulSvgIcon.SetState(metaProp?.Value); //給入狀態值
            return new HtmlString(statefulSvgIcon.Render());
        }
        #endregion


    }
}

namespace Lighter.NET.UiComponents
{
    using System.Reflection;
    using Lighter.NET.Common;
	using Lighter.NET.Helpers;
	/// <summary>
	/// UiElement基礎類別
	/// </summary>
	public abstract class UiElementBase : IUiElement
    {
        #region Constant
        /// <summary>
        /// 資料欄位名稱之data-屬性名稱
        /// </summary>
        public const string DATA_FIELD_ATTR_NAME = "data-field";
        /// <summary>
        /// 資料(列)鍵值(欄位值)之data-屬性名稱
        /// </summary>
        public const string DATA_KEY_ATTR_NAME = "data-key";
		/// <summary>
		/// 資料(列)鍵值(欄位名)之data-屬性名稱
		/// </summary>
		public const string DATA_KEY_FIELD_ATTR_NAME = "data-key-field";
		/// <summary>
		/// 操作命令之data-屬性名稱
		/// </summary>
		public const string DATA_COMMAND_ATTR_NAME = "data-command";
        /// <summary>
        /// 側邊面版時，是否可隱藏欄位之data-屬性名稱
        /// </summary>
        public const string DATA_CAN_HIDE_ATTR_NAME = "data-canHide";
        /// <summary>
        /// 欄位的RWD分級(1~6，數字越大，只顯示在越大的螢幕尺寸。若0，表示不分級)
        /// </summary>
        public const string DATA_COLUMN_RWD_LEVEL_ATTR_NAME = "data-column-rwd-level";
		/// <summary>
		/// 必填欄位之data-屬性名稱
		/// </summary>
		public const string DATA_REQUIRED_ATTR_NAME = "data-required";
		/// <summary>
		/// 唯一值欄位之data-屬性名稱
		/// </summary>
		public const string DATA_UNIQUE_ATTR_NAME = "data-unique";
        #endregion

        #region Static
        /// <summary>
        /// 元素基本預設屬性
        /// </summary>
        public static UiElementDefault Default = new UiElementDefault();
        /// <summary>
        /// 設定元素基本預設屬性
        /// </summary>
        /// <param name="defaultSetter">預設值設定</param>
        public static void SetDefault(Action<UiElementDefault> defaultSetter)
        {
            var uiDefault = new UiElementDefault();
            if(defaultSetter != null)
            {
                defaultSetter(uiDefault);
            }
            Default = uiDefault;
        }
        #endregion

        public abstract UiElementType ElementType { get; }
        public Type? DataType { get; set; }
        public string DataFormat { get; set; } = "";
        public bool WithId { get; set; } = true;
        /// <summary>
        /// 生成的html tag是否包含name屬性
        /// </summary>
        public bool WidthName { get; set; } = true;
        public string Id { get; set; } = "";
        public string Name { get;set; } = "";
        /// <summary>
        /// 是否表單欄位(預設值：false)
        /// </summary>
        public virtual bool IsFormFieldElement { get; set; } = false;
        /// <summary>
        /// 是否樣版欄位(預設值：false)
        /// </summary>
        public bool IsTemplateField { get; set; } = false;

        /// <summary>
        /// 生成Html tag 時的id屬性表示式
        /// </summary>
        protected string IDAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(Id)) return "";
                if (WithId == false) return "";
                if (IsTemplateField) return "";
                return $"id=\"{Id}\"";
            }
        }

        /// <summary>
        /// 生成Html tag 時的name屬性表示式
        /// </summary>
        protected string NameAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return "";
                if (WidthName == false) return "";
                if (IsTemplateField) return "";
                return $"name=\"{Name}\"";
            }
        }

        /// <summary>
        /// 生成Html tag 時的value屬性表示式
        /// </summary>
        protected string ValueAttribute
        {
            get
            {
                return $"value=\"{Value??""}\"";
            }
        }

        /// <summary>
        /// 生成Html tag 時的class屬性表示式
        /// </summary>
        protected string ClassAttribute
        {
            get
            {
                return $"class=\"{CssClass ?? ""}\"";
            }
        }

        /// <summary>
        /// 生成Html tag 時的disabled屬性表示式
        /// </summary>
        protected string DisabledAttribute
        {
            get
            {
                if (IsDisabled)
                {
                    return $"disabled=\"true\"";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 生成Html tag 時的樣版欄位特有的data-*屬性表示式
        /// </summary>
        /// <returns></returns>
        protected string TemplateDataAttribute()
        {
            if (IsTemplateField == false) return "";
            return $" {DATA_FIELD_ATTR_NAME}=\"{Name}\" data-type=\"{DataType?.Name?.ToLower()??""}\"";
        }

        /// <summary>
        /// 必填欄位data-required屬性標記
        /// </summary>
        /// <returns></returns>
        protected string RequiredDataAttribute()
        {
            if (!IsRequired) return "";
            return DATA_REQUIRED_ATTR_NAME;  //flag格式(無需值)
        }

        /// <summary>
        /// 值
        /// </summary>
        [SettingProperty]
        public string Value { get; set; } = "";

        /// <summary>
        /// 是否必要欄位(預設值：false)
        /// </summary>
        [SettingProperty]
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// 是否唯讀(預設值：false)
        /// </summary>
        [SettingProperty]
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// 是否disabled(預設值：false)
        /// </summary>
        [SettingProperty]
        public bool IsDisabled { get; set; } = false;

        [SettingProperty]
        public string CssClass { get;set; } = "";

        [SettingProperty]
        public string Style { get;set; } = "";
        /// <summary>
        /// 對應html元素之zindex屬性
        /// </summary>
        public string ZIndex { get; set; } = "";

        [SettingProperty]
        public List<AttributeItem> Attributes { get;set; } = new List<AttributeItem>();
        /// <summary>
        /// UI元素(以Html5 tag為主)預設使用HtmlEncode以防止XSS安全問題
        /// </summary>
        public bool EnableHtmlEncode { get; set; } = true;
        public Func<object?,object>? ValueConverter { get; set; }
        public MetaProperty? ModelMetaProperty { get; set; }
        /// <summary>
        /// 加入Html屬性
        /// </summary>
        /// <param name="attributes"></param>
        public void AddAttributes(IEnumerable<AttributeItem> attributes)
        {
            if(attributes != null && attributes.Count() > 0)
            {
                foreach (var attr in attributes)
                {
                    AddAttribute(attr.Name, attr.Value);
                }
            }
        }
        public IUiElement AddAttribute(string name, string? value = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            name = name.ToLower();
            //class, style, readonly : 另外處理
            if (name=="class")
            {
                _ = AddClass(value??"");
            }
            else if (name=="style")
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Style.Length > 0) Style += ";";
                    Style += value;
                }
            }else if (name == "readonly")
            {
                CharBool cb = new CharBool(value);
                IsReadOnly = cb.IsTrue;
            }
            else
            {
                var found = Attributes.FirstOrDefault(x => x.Name == name);
                if (found != null)
                {
                    found.Value = value;
                }
                else
                {
                    Attributes.Add(new AttributeItem() { Name=name,Value=value});
                }
            }
            return this;
        }

        public IUiElement AddDataAttribute(string name, string? value = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            /*DataAttribute的區別只在於屬性名稱必須是data-開頭*/
            if (value == null) value = "";
            string nameHead = "";
			if (name.Length >= 5)
            {
				nameHead = name.Substring(0, 5).ToLower();
			}
             
            if (nameHead != "data-") name = "data-" + name;
            return AddAttribute(name, value);
        }

        public IUiElement ClearClass()
        {
            CssClass = "";
            return this;
        }

        public IUiElement AddClass(string cssClassList)
        {
            cssClassList = cssClassList.Trim();
            if(string.IsNullOrEmpty(cssClassList)) return this;

            var cssArr = cssClassList.Trim().Split(" ").Select(x=>x.Trim());
            foreach (var css in cssArr)
            {
                if(CssClass.Contains(css)) continue;
                if (CssClass.Length > 0) CssClass += " ";
                CssClass += css;
            }

            return this;
        }

        public IUiElement ReadOnly()
        {
            IsReadOnly= true;
            return this;
        }

        public IUiElement Required()
        {
            IsRequired = true;
            return this;
        }

        /// <summary>
        /// 設為隱藏
        /// </summary>
        /// <returns></returns>
        public IUiElement Hide()
        {
            this.AddClass("hide");
            return this;
        }

        public IUiElement Editable(string cssClass = "editable")
        {
            _ = AddClass(cssClass);
            return this;
        }

        /// <summary>
        /// 使成為template tag(無id,name屬性，有data-field-name, data-type屬性)
        /// </summary>
        /// <returns></returns>
        public IUiElement AsTemplate()
        {
            IsTemplateField = true;
            return this;
        }

        public string Render()
        {
            return Render(setting:null);
        }
        public string Render(string value)
        {
            this.Value = value;
            return Render(setting: null);
        }
        public abstract string Render(IUiElement? setting=null);
        public virtual string RenderInnerHTML(IUiElement? setting = null) 
        {
            return "";
        }
        /// <summary>
        /// HtmlEncode
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string HtmlEncode(string content)
        {
            return HtmlEncodeProvider.Encode(content);
        }

        /// <summary>
        /// 套用參數設定(優先序：1)
        /// </summary>
        /// <param name="setting">同型別的物件參數</param>
        public void ApplySetting(IUiElement? setting)
        {
            /*
             * 取出setting參數物件中有標註SettingPropertyAttribute的屬性值，
             * 套用到此Ui元件中
             */
            if (setting == null) return;
            var props = setting.GetType().GetProperties().Where(p => p.IsDefined(typeof(SettingPropertyAttribute)))?.ToArray();

            if (props != null && props.Length > 0)
            {
                ObjectHelper.Assign(this, setting, props, ignoreDefaultValue: true);
            }
        }

        /// <summary>
        /// 產生全部的html屬性表示式
        /// </summary>
        /// <returns></returns>
        protected string MakeAllAttributesExpression()
        {
            if(Attributes.Count > 0)
            {
                var transformed = Attributes.Select(x => GetAttributeExpression(x)).ToArray();
                return string.Join(" ", transformed);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 取得html屬性表示式
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        private string GetAttributeExpression(AttributeItem attr)
        {
            if (attr == null) return "";
            if(attr.Value == null) return attr.Name;
            return $"{attr.Name}=\"{attr.Value}\"";
        }
    }
}

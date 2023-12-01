using Lighter.NET.Common;
namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 生成label的html tag
    /// </summary>
    public class Label : UiElementBase
    {
        public override UiElementType ElementType => UiElementType.Label;
        /// <summary>
        /// 不顯示(必填欄位)星號
        /// </summary>
        [SettingProperty]
        public bool NoStar { get; set; } = false;
        /// <summary>
        /// 是否包含DisplayAttribute的Description
        /// </summary>
        public bool IncludeDescription { get; set; } = false;
        /// <summary>
        /// Description要套用的Css Class
        /// </summary>
        public string DescriptionCssClass { get; set; } = "";
        /// <summary>
        /// 採用完整的Model的DisplayName屬性值(不使用ShortName)
        /// </summary>
        [SettingProperty]
        public bool UseFullDisplayName { get; set; } = false;
        /// <summary>
        /// 經過語系轉換的Label文字
        /// </summary>
        [SettingProperty]
        public string LocalizedText { get; set; } = "";
        public override string Render( IUiElement? setting = null)
        {
            ApplySetting(setting); 
            string displayName = GetDisplayName();

            string requiredStar = GetRequiredStar();
            string cssClass = this.CssClass;
            if (requiredStar != "") cssClass += $" required";

            string html = $"<label for=\"{this.Name}\" class=\"{cssClass}\">{requiredStar}{displayName}</label>";
            return html;
        }

        public override string RenderInnerHTML(IUiElement? setting = null)
        {
            return GetDisplayName();
        }

        /// <summary>
        /// 取得必填欄位星號標示
        /// </summary>
        /// <param name="metaProp"></param>
        /// <returns></returns>
        protected virtual string GetRequiredStar()
        {
            return (IsRequired)? "★" : ""; 
        }

        /// <summary>
        /// 取得顯示名稱，預設順序：ShortName > FullName > Name
        /// </summary>
        /// <param name="metaProp"></param>
        /// <returns></returns>
        public virtual string GetDisplayName()
        {
            if (!string.IsNullOrEmpty(LocalizedText)) return LocalizedText;
            if (ModelMetaProperty == null || ModelMetaProperty.Display==null) return this.Name;
            /*預設順序：ShortName > FullName > Name */
            string displayName = "";
            var displayAttr = ModelMetaProperty.Display;

			if (!UseFullDisplayName) { 
                displayName = displayAttr.ShortName ?? "";
            }
            if (displayName == "") displayName = displayAttr.Name ?? "";
            
            displayName =  (displayName == "") ? this.Name : displayName;
            if (IncludeDescription)
            {
                string desc = displayAttr.Description??"";
                desc = desc.TrimStart(new char[] { '(' });
                desc = desc.TrimEnd(new char[] { ')' });
				if(!string.IsNullOrEmpty(desc))
                {
                    string descCssClass = "";
                    if (!string.IsNullOrEmpty(DescriptionCssClass)) descCssClass = $"class=\"DescriptionCssClass\"";
                    displayName = $"{displayName}<span {descCssClass}>({desc})</span>";
                }
			}

            return displayName;
        }
    }
}

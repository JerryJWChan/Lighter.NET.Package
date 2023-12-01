using System.Text;
using Lighter.NET.Common;
namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 資料分頁按鈕列(此元素物件必須配合lighter.js使用)
    /// </summary>
    public class PagingBar : UiElementBase
    {
        public override UiElementType ElementType => UiElementType.PaginationBar;
        public PagingSetting PagingSetting { get; set; } = new PagingSetting();
        /// <summary>
        /// 前端換頁事件javascript處理函式
        /// </summary>
        public string ChangePageHandler { get; set; } = "";
        /// <summary>
        /// 前端換頁事件要呼叫的後端Action的Url
        /// </summary>
        public string ChangePageActionUrl { get; set; } = "";
        /// <summary>
        /// 綁定的目標元素id(例如：表格id或div容器id)
        /// </summary>
        public string BindingTargetId { get; set; } = "";
        /// <summary>
        /// 資料分頁按鈕列(此元素物件必須配合lighter.js使用)
        /// </summary>
        public PagingBar()
        {
            CssClass = "paging-bar";
        }
        public override string Render(IUiElement? setting = null)
        {
            if (string.IsNullOrEmpty(Id)) Id = "pagingBar"; //預設Id
            string cssClassAttr = $" class=\"{CssClass}\"";
            string innerHTML = RenderInnerHTML();

            if(this.PagingSetting.PageCount == 0)
            {
                cssClassAttr += " hide";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<div {IDAttribute} {cssClassAttr}>");
            sb.AppendLine(innerHTML);
            sb.AppendLine("</div>");
            //搭配Lighter js object，用以綁定換頁事件
            sb.AppendLine("<script>");
            //將PagingSetting轉成json格式，用作前端的$PagingBar.set()函式的參數
            string pagingSettingJson = CustomJsonSerializer.Serialize(PagingSetting);
            string handlerFunction = $@"(pagingSetting, changePageActionUrl, bindingTargetId)=>{{{ChangePageHandler}(pagingSetting, changePageActionUrl, bindingTargetId);}}";
            sb.AppendLine($"let _{Id} = $PagingBar('#{Id}').set({pagingSettingJson},{handlerFunction},'{ChangePageActionUrl}','{BindingTargetId}');");
            sb.AppendLine("</script>");
            return sb.ToString();
        }

        public override string RenderInnerHTML(IUiElement? setting = null)
        {
            //參數檢核
            string errMsg;
            bool isValid = SettingValidation(out errMsg);
            if(!isValid)
            {
                return $"<span class=\"msg-error\">{errMsg}</span>";
            }

            ApplySetting(setting);
            StringBuilder sb = new StringBuilder();
            //5頁(含)以下
            if(PagingSetting.PageCount <= 5)
            {
                if(PagingSetting.PageCount >=1)
                {
                    sb.AppendLine($"<button type=\"button\" class=\"first current-page\" value=\"1\">1</button>\n");
                }
                if (PagingSetting.PageCount >= 2)
                {
                    sb.AppendLine($"<button type=\"button\" class=\"previous\" value=\"2\">2</button>\n");
                }
                if (PagingSetting.PageCount >= 3)
                {
                    sb.AppendLine($"<button type=\"button\" class=\"middle\" value=\"3\">3</button>\n");
                }
                if (PagingSetting.PageCount >= 4)
                {
                    sb.AppendLine($"<button type=\"button\" class=\"next\" value=\"4\">4</button>\n");
                }
                if (PagingSetting.PageCount == 5)
                {
                    sb.AppendLine($"<button type=\"button\" class=\"last\" value=\"5\">5</button>\n");
                }

                return sb.ToString();
            }

            //5頁以上
            sb.AppendLine($"<button type=\"button\" class=\"first\" value=\"1\">1</button>\n");
            sb.AppendLine($"<span class=\"more-before hide\">....</span>\n");
            sb.AppendLine($"<button type=\"button\" class=\"previous\" value=\"2\">2</button>\n");
            sb.AppendLine($"<button type=\"button\" class=\"middle\" value=\"3\">3</button>\n");
            sb.AppendLine($"<button type=\"button\" class=\"next\" value=\"4\">4</button>\n");
            sb.AppendLine($"<span class=\"more-after\">....</span>\n");
            sb.AppendLine($"<button type=\"button\" class=\"last\" value=\"{PagingSetting.PageCount}\">{PagingSetting.PageCount}</button>\n");
            return sb.ToString();
        }

        /// <summary>
        /// 參數檢核
        /// </summary>
        private bool SettingValidation(out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrEmpty(Id))
            {
                errMsg += "PaginationBar元素之Id屬性不可為空值";
            }
            if (PagingSetting == null)
            {
                if (errMsg != "")  errMsg += ";";
                errMsg = "PagingSetting 參數不可為null";
            }

            if(ChangePageHandler == "")
            {
                if (errMsg != "") errMsg += ";";
                errMsg = "未設定ChangePageHandler參數";
            }
            return errMsg == "";
        }
    }
}

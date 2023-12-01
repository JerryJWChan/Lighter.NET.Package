using Lighter.NET.Common;
using System.Text;

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 步驟(step-by-step)狀態列
    /// </summary>
    public class StepBar : UiElementBase
    {
        private int _currentStep = 1;
        /// <summary>
        /// 總步驟數
        /// </summary>
        public int StepCount { get; set; } = 1;
        /// <summary>
        /// 當前步驟數
        /// </summary>
        public int CurrentStep
        {
            get { return _currentStep; }
            set 
            { 
                if(value > StepCount)
                {
                    throw new ArgumentException($"The CurrentStep={value} cannot exceed total StepCount={StepCount}");
                }
                _currentStep = value; 
            }
        }
        /// <summary>
        /// 步驟數字顯示尺寸大小(以S/M/L/XL...表示)
        /// </summary>
        public CssClassSize Size { get; set; } = CssClassSize.M;
        public override UiElementType ElementType => UiElementType.StepBar;

        public override string Render(IUiElement? setting = null)
        {
            string sizeCssClass = $"size-{Size.ToString().ToLower()}";

            string stepNumberHtml = StepNumberHtml();
            string tagHtml = $@"
    <div class=""flex-box flex-center"">
        <div class=""step-bar flex-center-v {sizeCssClass} {CssClass}"" {IDAttribute} data-current-step=""{CurrentStep}"">
            {stepNumberHtml}
        </div>
    </div>
";
            return tagHtml;
        }

        /// <summary>
        /// 步驟數字Html
        /// </summary>
        /// <returns></returns>
        private string StepNumberHtml()
        {
            StringBuilder sb  = new StringBuilder();    
            if (CurrentStep < 1) CurrentStep = 1;
            string currentCssClass = "";
            for (int i = 1; i <= StepCount; i++)
            {
                currentCssClass = (i == CurrentStep) ? "current" : "";
                sb.AppendLine($"<div class=\"step-number {currentCssClass}\" data-step=\"{i}\">{i}</div>");
                if (i < StepCount)
                {
                    sb.AppendLine($"<hr class=\"step-line\"/>");
                }
            }
            return sb.ToString();
        }
    }
}

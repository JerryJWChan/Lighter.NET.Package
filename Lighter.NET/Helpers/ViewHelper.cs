using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;


namespace Lighter.NET.Helpers
{
    public static class ViewHelper
    {
        /// <summary>
        /// 取得Partial view 生成的html
        /// </summary>
        /// <param name="controller">action所屬的controller</param>
        /// <param name="partialViewName">partial view名稱(含相對路徑)</param>
        /// <param name="model">要傳給view的model</param>
        /// <returns></returns>
        public static async Task<string> GetPartialViewHtml(Controller controller, string partialViewName, object model)
        {
            try
            {
                IViewEngine? vEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult? vResult = vEngine?.FindView(controller.ControllerContext, partialViewName, false);
  
                if(vResult == null || vResult.View == null)
                {
                    return $"the partialViewName[{partialViewName}] is not found.";
                }

                using (StringWriter writer = new StringWriter())
                {
                    ViewContext vContext = new ViewContext(
                        controller.ControllerContext, 
                        vResult.View,
                        controller.ViewData,
                        controller.TempData, 
                        writer, 
                        new HtmlHelperOptions()
                        );
                    vContext.ViewData.Model = model;
                    
                    await vResult.View.RenderAsync(vContext);
                    return writer.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        /// <summary>
        /// 讀取任意檔案內容做為ParialView的Content
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetContentFromFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (FileNotFoundException)
            {
                return $"file not found. filePath={filePath}";
            }
            catch(Exception ex)
            {
                //To do:log error message
                Console.WriteLine(ex.Message);
                return $"載入檔案內容發生錯誤，filePath={filePath}";
            }
        }

    }
}

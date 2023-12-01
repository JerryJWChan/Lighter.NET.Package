using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace Lighter.NET.Common
{
    /// <summary>
    /// ViewModel打包容器，提供View所需的「強型別」model(可多個)和基本狀態資訊打包
    /// </summary>
    public class ViewModelWrapper:BaseViewModel
    {
        private Dictionary<string,object?> _store = new Dictionary<string,object?>();
        /// <summary>
        /// 訊息
        /// </summary>
        public IList<MessageModel> Messages = new List<MessageModel>();
        /// <summary>
        /// DataModel的檢核錯誤
        /// </summary>
        public IList<ModelError> ModelErrors = new List<ModelError>();
        /// <summary>
        /// 連結按鈕定義(提供可在View上生成連結或按鈕，例如：返回上一頁、返回首頁)
        /// </summary>
        public IList<HyperLinkDefinition> HyperLinks= new List<HyperLinkDefinition>();
        /// <summary>
        /// 有訊息
        /// </summary>
        public bool HasMessage
        {
            get { return (Messages != null) && (Messages.Count > 0); }
        }
        /// <summary>
        /// 有錯誤
        /// </summary>
        public bool HasError
        {
            get
            {
                return (Messages != null && Messages.Any(x=>x.Type == MessageType.Error)) || HasModelError;
            }
        }
        /// <summary>
        /// 有資料檢核錯誤
        /// </summary>
        public bool HasModelError
        {
            get
            {
                return (ModelErrors != null) && ModelErrors.Count > 0;
            }
        }

        /// <summary>
        /// 有連結按鈕(例如：回上一頁)
        /// </summary>
        public bool HasHyperLink 
        { 
            get 
            {
                return HyperLinks.Count > 0;
            } 
        }
        /// <summary>
        /// VMWrapper中的Model是否合格(非null且無檢核錯誤)
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool isValid = (HasModelError == false) && (HasError == false);
                return isValid;
            }
        }
        /// <summary>
        /// 是否有model是null
        /// </summary>
        public bool HasNullModel
        {
            get
            {
                return _store.Values.Any(x => x is null);
            }
        }
        /// <summary>
        /// ViewModel打包容器，提供View所需的「強型別」model(可多個)和基本狀態資訊打包
        /// </summary>
        public ViewModelWrapper() { }

        #region 連結按鈕預設Css
        public static string HyperLinkCssClassBlue { get; set; } = "button outline-blue";
        public static string HyperLinkCssClassRed { get; set; } = "button outline-red";
        public static string HyperLinkCssClassGreen { get; set; } = "button outline-green";
        public static string HyperLinkCssClassYellow { get; set; } = "button outline-yellow";
        public static string HyperLinkCssClassGray { get; set; } = "button outline-gray";
        public static string HyperLinkCssClassBrown { get; set; } = "button outline-brown";
        #endregion

        #region Model與參數
        /// <summary>
        /// 加入model，同一型別原則上只可加入一次，相同型別加入多次時，須傳入modelName做為區分，否則就覆蓋前一個
        /// </summary>
        /// <typeparam name="T">model型別</typeparam>
        /// <param name="model">model</param>
        /// <param name="modelName">model名稱(用以區分相同型別的model)</param>
        /// <returns></returns>
        public ViewModelWrapper AddModel<T>(T model, string modelName = "")
        {
            _Add(model,modelName);
            return this;
        }

        /// <summary>
        /// 加入選項清單
        /// </summary>
        /// <typeparam name="TModel">要用到此選項清單的DataModel type</typeparam>
        /// <param name="columSelector">要對應的欄位選擇器</param>
        /// <param name="optionList">選項清單</param>
        /// <returns></returns>
        public ViewModelWrapper AddOptions<TModel>(Expression<Func<TModel, object?>> columnSelector, List<OptionItem> optionList)
        {
            var columnName = columnSelector.GetLambdaPropertyName();
            string key = typeof(TModel).ToString() + columnName;
            _Add(optionList,key);
            return this;
        }

        /// <summary>
        /// 加入選項清單
        /// </summary>
        /// <param name="optionList">選項清單</param>
        /// <param name="optionName">選項名稱(慣例：傳入與下拉選單的id屬性相同的名稱)</param>
        /// <returns></returns>
        public ViewModelWrapper AddOptions(List<OptionItem> optionList, string optionName)
        {
            _Add(optionList,optionName);
            return this;
        }

        /// <summary>
        /// 取出指定型別的Model(只限class型別)
        /// </summary>
        /// <typeparam name="TModel">model型別</typeparam>
        /// <param name="modelName">model別名(用以區分相同型別的不同model)</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TModel? GetModel<TModel>(string modelName = "")
        {
            Type modelType = typeof(TModel);
            string key = $"{modelType.FullName??modelType.Name}_{modelName}";
            if (_store.ContainsKey(key)==false)
            {
                throw new TypeAccessException($"GetModel()型別或modelName參數錯誤,The VMWrapper does not contains any model with the key of the type and modelName combination [{key}]. Make sure the model of this type has been added to the VMWrapper in the controller action method.");
            }
                
            var modelValue = _store[key];
            return (modelValue != null) ? (TModel?)modelValue : default;

            ////(1)reference-type
            //if (modelType.IsClass)
            //{
            //    return (modelValue != null) ? (TModel?)modelValue : default;
            //}
            ////(2)enum
            //else if (modelType.IsEnum)
            //{
            //    return SafeParseEnum<TModel>(Convert.ToInt32(modelValue));
            //    //if (modelValue is int)
            //    //{
                    
            //    //}
            //    //else
            //    //{

            //    //}
            //}
            ////(3)value-type
            //else if (modelType.IsValueType)
            //{
            //    if(modelValue is IConvertible)
            //    {
            //        try
            //        {
            //            var objValue = Convert.ChangeType(modelValue, modelType, CultureInfo.InvariantCulture);
            //            if(objValue == null)
            //            {
            //                throw new TypeAccessException($"Convert to {modelType.FullName} failed for item key {key}."); 
            //            }
            //            return (TModel)objValue;
            //        }
            //        catch (Exception ex)
            //        {
            //            throw new Exception($"Convert to {modelType.FullName} failed for item key {key}. Error message:{ex.Message}");
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception($"Convert to {modelType.FullName} failed for item key {key} because the underlining value does not implement Iconvertible interface");
            //    }
            //}
            //else
            //{
            //    throw new Exception($"GetModel<{modelType.FullName}>() failed for item key {key}. The actual type of the item value is not supported.");
            //}

        }

     
        /// <summary>
        /// 取得選項清單
        /// </summary>
        /// <typeparam name="TModel">要用到此選項清單的DataModel type</typeparam>
        /// <param name="columnSelector">要對應的欄位選擇器</param>
        /// <returns></returns>
        public List<OptionItem> GetOptions<TModel>(Expression<Func<TModel, object?>> columnSelector)
        {
            var columnName = columnSelector.GetLambdaPropertyName();
            string key = typeof(TModel).ToString() + columnName;
            var optionList = GetModel<List<OptionItem>>(key);
            if (optionList == null) optionList = new List<OptionItem>();
            return optionList;
        }

        /// <summary>
        /// 取得選項清單(List<OptionItem>型態)
        /// </summary>
        /// <param name="optionName">選項名稱(慣例：傳入與下拉選單的id屬性相同的名稱)</param>
        /// <returns></returns>
        public List<OptionItem> GetOptions(string optionName)
        {
            var optionList = GetModel<List<OptionItem>>(optionName);
            if(optionList == null) optionList= new List<OptionItem>();
            return optionList;
        }

        #endregion

        #region 訊息
        /// <summary>
        /// 設定訊息
        /// </summary>
        /// <param name="msgType">訊息種類</param>
        /// <param name="msgCaption">訊息標題</param>/// 
        /// <param name="msgText">訊息文字</param>
        public void AddMessage(MessageType msgType, string msgCaption, string msgText = "",bool isPopup=false)
        {
            Messages.Add(new MessageModel() { Type = msgType, Caption= msgCaption, Text = msgText, IsPopup = isPopup });
        }

        /// <summary>
        /// 加入檢核錯誤
        /// </summary>
        /// <param name="propertyName">屬性(欄位)名稱</param>
        /// <param name="message">檢核錯誤訊息</param>
        public void AddModelError(string propertyName,string message)
        {
            var item = ModelErrors.FirstOrDefault(x => x.PropertyName == propertyName);
            if (item != null)
            {
                if (!(item.Message.EndsWith(',') || item.Message.EndsWith('.'))) item.Message += ",";
                item.Message += message;
            }
            else
            {
                ModelErrors.Add(new ModelError() {PropertyName = propertyName, Message = message });
            }
        }

        /// <summary>
        /// 加入檢核錯誤(多筆)
        /// </summary>
        /// <param name="modelErrors"></param>
        public void AddModelError(IList<ModelError> modelErrors)
        {
            if(modelErrors == null || modelErrors.Count == 0) return;
            foreach (ModelError modelError in modelErrors)
            {
                ModelErrors.Add(modelError);
            }
        }

        /// <summary>
        /// 加入檢核錯誤(不指定屬性(欄位)名稱，例如model本身是null或其他錯誤)
        /// </summary>
        /// <param name="message"></param>
        public void AddModelError(string message)
        {
            //自動產生一個不重複的替代屬性名稱
            string msgName = $"#{ModelErrors.Count}";
            AddModelError(msgName, message);
        }

        /// <summary>
        /// 取得訊息文字
        /// </summary>
        /// <param name="seperator">訊息分隔字串(例如逗號","或html換行標記"</br>")</param>
        /// <param name="includeModelError">是否包含DataModel Error</param>
        /// <returns></returns>
        public string GetMessage(string seperator = ",",bool includeModelError = false)
        {
            if ((Messages == null || Messages.Count == 0) && includeModelError==false)  return "";
            string msg = "";
            if(Messages!.Count > 0 ) {
                msg += string.Join(seperator, Messages.Select(x => $"{x.Caption}:{x.Text}").ToArray());
            }
            
            if (includeModelError & ModelErrors.Count > 0)
            {
                msg += string.Join(seperator, ModelErrors.Select(x => $"{x.PropertyName}:{x.Message}" ).ToArray());
            }
  
            return msg.TrimStart(':');
        }

        /// <summary>
        /// 取得訊息列表
        /// </summary>
        /// <param name="includeModelError">是否包含Model Error</param>
        /// <returns></returns>
        public IList<MessageModel> GetMessages(bool includeModelError = false)
        {
            IList<MessageModel> msgList = Messages;
            if (includeModelError)
            {
                if (ModelErrors.Count > 0)
                {
                    foreach (var err in ModelErrors)
                    {
                        msgList.Add(new MessageModel()
                        {
                            Type = MessageType.Error,
                            Caption = "",
                            Text = $"{err.PropertyName},{err.Message}"
                        });
                    }
                }
            }
            return msgList;
        }

        /// <summary>
        /// 取得訊息的Json(供client的ShowMessage()用)
        /// </summary>
        /// <param name="includeModelError">是否包含Model Error</param>
        /// <returns></returns>
        public string GetMessageJson(bool includeModelError = false)
        {

            IList<MessageModel> msgList = GetMessages(includeModelError);

            //string json = JsonHelper.Serialize(msgList);
            //var converter = new Newtonsoft.Json.Converters.StringEnumConverter();
            //string json = JsonConvert.SerializeObject(msgList,converter);

            JsonSerializerOptions options= new JsonSerializerOptions();
            //避免JsonResult中的中文字被編碼成UCN (Unicode Character Name)
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            //json換行顯示
            options.WriteIndented = true;
            //使用camelCase(首字小寫)慣例
            options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            //將enum序列化成字串，而非數字
            options.Converters.Add(new JsonStringEnumConverter());

            string json = JsonSerializer.Serialize(msgList, options);
            return json;
           
        }
        #endregion

        #region 基本檢核
        /// <summary>
        /// 檢核屬性是否null
        /// </summary>
        /// <param name="modelProperty">model的屬性</param>
        /// <param name="modelErrorMessage">檢核訊息</param>
        /// <param name="propertyName">屬性名稱(若略則自動截取)</param>
        public void CheckNull(object? modelProperty,string modelErrorMessage , [CallerArgumentExpression("modelProperty")] string propertyName = "")
        {
            if(modelProperty == null)
            {
				AddModelError(GetOnlyPropertyName(propertyName),modelErrorMessage);
			}
        }

		/// <summary>
		/// 檢核屬性是否null或空值
		/// </summary>
		/// <param name="modelProperty">model的屬性</param>
		/// <param name="modelErrorMessage">檢核訊息</param>
		/// <param name="propertyName">屬性名稱(若略則自動截取)</param>
		public void CheckNullOrEmpty(object? modelProperty, string modelErrorMessage = "", [CallerArgumentExpression("modelProperty")] string propertyName = "")
		{
			if (modelProperty == null || string.IsNullOrEmpty(modelProperty.ToString()))
			{
                if (modelErrorMessage == "") modelErrorMessage = "未填寫";
				AddModelError(GetOnlyPropertyName(propertyName), modelErrorMessage);
			}
		}

        /// <summary>
        /// 檢核任一項屬性是否有值
        /// </summary>
        /// <param name="modelProperty1">model的屬性1</param>
        /// <param name="modelProperty2">model的屬性2</param>
        /// <param name="modelErrorMessage">檢核訊息</param>
        /// <param name="propertyName">屬性名稱(若略則自動截取)</param>
        public bool CheckAnyHasValue(object? modelProperty1, object? modelProperty2, string modelErrorMessage = "", [CallerArgumentExpression("modelProperty1")] string propertyName1 = "", [CallerArgumentExpression("modelProperty2")] string propertyName2 = "")
        {
            bool prop1Empty = false;
            bool prop2Empty = false;
            if (modelProperty1 == null || string.IsNullOrEmpty(modelProperty1.ToString()))
            {
                prop1Empty= true;
            }
            if (modelProperty2 == null || string.IsNullOrEmpty(modelProperty2.ToString()))
            {
                prop2Empty = true;
            }

            bool success = !(prop1Empty && prop2Empty);
            if(!success)
            {
                if (modelErrorMessage == "") modelErrorMessage = "至少須填寫其中一項";
                AddModelError(GetOnlyPropertyName(propertyName1), modelErrorMessage);
                AddModelError(GetOnlyPropertyName(propertyName2), modelErrorMessage);
            }

            return success;
        }

        #endregion

        #region 加入連結(按鈕)

        /// <summary>
        /// 加入連結
        /// </summary>
        /// <param name="hyperLink">連結定義</param>
        /// <returns></returns>
        public ViewModelWrapper AddHyperLink(HyperLinkDefinition? hyperLink)
        {
            if(hyperLink != null)
            {
                HyperLinks.Add(hyperLink);
            }
            
            return this;
        }

        /// <summary>
        /// 加入連結(呈現藍色)
        /// </summary>
        /// <param name="text">連結文字</param>
        /// <param name="url">網址</param>
        /// <param name="target">目標視窗</param>
        /// <param name="cssClass">css class(多項時以空白分隔)</param>
        /// <returns></returns>
        public ViewModelWrapper AddHyperLinkBlue(string text, string url, HyperLinkTargetType target = HyperLinkTargetType.Self)
        {
            HyperLinks.Add(new HyperLinkDefinition()
            {
                Text = text,
                Url = url,
                Target = target,
                CssClass = HyperLinkCssClassBlue
            });
            return this;
        }

        /// <summary>
        /// 加入連結(呈現藍色)
        /// </summary>
        /// <param name="text">連結文字</param>
        /// <param name="url">網址</param>
        /// <param name="target">目標視窗</param>
        /// <param name="cssClass">css class(多項時以空白分隔)</param>
        /// <returns></returns>
        public ViewModelWrapper AddHyperLinkRed(string text, string url, HyperLinkTargetType target = HyperLinkTargetType.Self)
        {
            HyperLinks.Add(new HyperLinkDefinition()
            {
                Text = text,
                Url = url,
                Target = target,
                CssClass = HyperLinkCssClassRed
            });
            return this;
        }

        /// <summary>
        /// 取得Controller設定的連結按鈕的Html
        /// </summary>
        /// <returns></returns>
        public string GetHyperLinksHtml()
        {
            if (HyperLinks.Count == 0) return "";
            string html = "";
            foreach (var link in HyperLinks)
            {
                html += $"<a href=\"{link.Url}\" target=\"_{link.Target.ToString().ToLower()}\" class=\"{link.CssClass}\">{link.Text}</a>";
            }
            return html;
        }
        #endregion

        #region Protect/Private functions

        /// <summary>
        /// 取得只有property name(去掉class name)
        /// </summary>
        /// <param name="proName">自動截取的傳入屬性名稱</param>
        /// <returns></returns>
        protected string GetOnlyPropertyName(string propName)
        {
            if(propName.IndexOf('.') >= 0)
            {
                return propName.Split('.').Last();
            }
            else
            {
                return propName;
            }
        }

        /// <summary>
        /// 加入model，同一型別原則上只可加入一次，相同型別加入多次時，須傳入modelName做為區分，否則就覆蓋前一個
        /// </summary>
        /// <typeparam name="T">model型別</typeparam>
        /// <param name="model">model</param>
        /// <param name="modelName">model名稱(用以區分相同型別的model)</param>
        protected void _Add<T>(T model, string modelName = "")
        {
            Type modelType= typeof(T);
            string key = $"{modelType.FullName?? modelType.Name}_{modelName}";
            if (_store.ContainsKey(key))
            {
                _store[key] = model;
                //model覆蓋時，給出警告訊息
                AddMessage(MessageType.Warning, "", $"AddModel()或AddOptions()動作時，[{typeof(T).Name}]型別的model發生覆蓋的情形，請確認相同型的model有使用欄位選擇子或modelName參數加以區分");
            }
            else
            {
                _store.Add(key, model);
            }
        }

        /// <summary>
        /// 將物件轉換成Enum值，並確保轉換後的值包含於enum的宣告項目之中
        /// </summary>
        /// <typeparam name="TEnum">Enum型別</typeparam>
        /// <param name="value">字串值</param>
        /// <returns></returns>
        protected TEnum SafeParseEnum<TEnum>(object? value)
        {
            if(value == null)
            {
                throw new ArgumentNullException("SafeParseEnum() failed. The value argument is null which is not supported.");
            }

            TEnum result;
            try
            {
                result = (TEnum)value;

                /*
                 * 若傳入的value是數字且不在TEnum的正常值範圍內時, result仍會有值
                 * 故須再判斷IsDefined
                 */

                if (Enum.IsDefined(typeof(TEnum), result))
                {
                    return result;
                }
                else
                {
                    throw new Exception($"SafeParseEnum() failed. The value argument is out of the range of the {typeof(TEnum).FullName} enum declaration.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"SafeParseEnum() failed. Conversion of the value {value} to {typeof(TEnum).FullName} failed. Error message:{ex.Message}");
            }

        }

        #endregion

        #region Testing Dev Tool
        /// <summary>
        /// 測試TModel全部屬性的model error(給出假的檢核錯誤訊息)，
        /// 以便檢查前端validation hint訊息的顯示情形是否正確
        /// </summary>
        public void TestAllModelError<TModel>()
        {
            var props = typeof(TModel).GetProperties();
            foreach (var p in props)
            {
                AddModelError(p.Name, $"{p.Name}資料錯誤，請重新填寫。");
            }
        }
        #endregion
    }

}

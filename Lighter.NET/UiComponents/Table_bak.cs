namespace Lighter.NET.UiComponents
{
    using System;
    using System.Data;
    using System.Text;
    using System.Xml;
    using Lighter.NET.Common;

    /// <summary>
    /// 生成table的html tag
    /// </summary>
    public class Table_bak : UiElementBase
    {
        public override UiElementType ElementType => UiElementType.Table;
        /// <summary>
        /// 生成的table的client端id
        /// </summary>
        public string TableClientId
        {
            get { return $"{Id ?? "tb_id_undefined"}";  }
        }
        /// <summary>
        /// 生成的容器元素的client端id
        /// </summary>
        public string ContainerClientId
        {
            get { return $"{TableClientId}_container"; }
        }
        /// <summary>
        /// 對應至 html: class屬性
        /// </summary>
        public new string CssClass { get; set; } = "bright";  //預設值bright須搭配lighter.css才有效
        /// <summary>
        /// 欄位值或元素值轉換器(加入時，用key值對應欄位名稱，或元素名稱)
        /// </summary>
        public Dictionary<string, Func<object, object>> ValueConverters { get; } = new Dictionary<string, Func<object, object>>();
        /// <summary>
        /// 欄位套用的css class(加入時，用key值對應欄位名稱，或元素名稱)
        /// </summary>
        public Dictionary<string, string> ColumnCssClass { get; } = new Dictionary<string, string>();
        /// <summary>
        /// 不使用HtmlEncode的欄位名稱
        /// </summary>
        public List<string> NoHtmlEncodeFields { get; } = new List<string>();
        /// <summary>
        /// 啟用單/偶列不用css style
        /// </summary>
        public bool EnableAlternativeRowStyle { get;set; }  = true;
        /// <summary>
        /// 啟用整列選取(Note:須搭配使用lighter.js和lighter.css)
        /// </summary>
        public bool EnableRowSelect { get; set; } = false;
        /// <summary>
        /// 無資料時是否顯示欄位標頭
        /// </summary>
        public bool ShowHeaderWhenNoData { get; set; } = false;
        /// <summary>
        /// 加上序號欄位
        /// </summary>
        public bool AddSerialNo { get; set; } = false;
        /// <summary>
        /// 加上編輯按鈕欄位
        /// </summary>
        public bool AddEditButton { get; set; } = false;
        /// <summary>
        /// 加上刪除按鈕欄位
        /// </summary>
        public bool AddDeleteButton { get; set; } = false;
        /// <summary>
        /// 表格資料來源
        /// </summary>
        public DataTable? DataSource { get; set; }
        /// <summary>
        /// 欄位標頭文字(若未指定，預設使用DataTable的ComlumnText)
        /// </summary>
        public string[] ColumnHeaders { get; set; } = new string[0];
        /// <summary>
        /// 響應式欄位要套用的css class(依欄位順序)(Note:須搭配使用lighter.css才有效)
        /// </summary>
        public string[] RWDColumnCssClass { get; set; } = new string[0];
        /// <summary>
        /// 欄位文字水平對齊css class(指定一項，則套用到每一欄，若指定多項，則每一項對應一欄)
        /// </summary>
        public TextAlign[] TextAlign { get; set; } = new TextAlign[0];
        /// <summary>
        /// 無資料時要顯示的文字
        /// </summary>
        public string NoDataText { get; set; } = "(No Data)";
        /// <summary>
        /// 欄位顯示文字長度上限(超過以...表示)
        /// </summary>
        public int MaxTextLength { get; set; } = 0;
        /// <summary>
        /// 序號欄位標頭
        /// </summary>
        public string SerialNoHeaderText { get; set; } = "序號";
        /// <summary>
        /// 操作按鈕欄位標頭
        /// </summary>
        public string CommandButtonHeaderText { get; set; } = "";
        /// <summary>
        /// 鍵值欄位名稱(有多個鍵值欄位時，以逗號分隔)
        /// </summary>
        public string KeyFieldName { get; set; } = "";
        /// <summary>
        /// 列選取事件處理函式(client端的javascript)名稱
        /// </summary>
        public string RowSelectEventHandler { get; set; } = "";
        /// <summary>
        /// 編輯按鈕內容(文字或html)
        /// </summary>
        public string EditButtonContent { get; set; } = "編輯";
        /// <summary>
        /// 刪除按鈕內容(文字或html)
        /// </summary>
        public string DeleteButtonContent { get; set; } = "刪除";
        /// <summary>
        /// 列操作命令(例如edit,delete...)事件處理函式(client端的javascript)名稱
        /// </summary>
        public string RowCommandEventHandler { get; set; } = "";

        public override string Render(IUiElement? setting = null)
        {
            /*
             * 生成Html結構
             * <div id="xxx_table_containter">
             *      (pager:如果有的話)
             *      <table id="xxx_table">
             *          (表格內容)
             *      </table>
             *      <script>
             *          (註冊row的focus事件，做為RowSelect事件，使觸發RowSelectEventHandler)
             *      </script>
             * </div>
             * (上面的xxx表示此物件的ID屬性)
             */

            //ApplySetting(setting); (不需要)
            //ApplyMetaProperty(metaProp); (不需要)

            //參數檢核
            SettingValidation();

            string cssClassAttr = $" class=\"{CssClass}\"";
            string innerHTML = RenderInnerHTML();
            string script = PrepareRowEventScript();

            StringBuilder sb= new StringBuilder();
            sb.AppendLine($"<div id=\"{ContainerClientId}\" class=\"table_container\">");
            sb.AppendLine($"<table id=\"{TableClientId}\"{cssClassAttr}>");
            sb.AppendLine(innerHTML);
            sb.AppendLine("</table>");
            sb.AppendLine(script);
            sb.AppendLine("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// 生成innerHTML
        /// </summary>
        /// <returns></returns>
        public override string RenderInnerHTML(IUiElement? setting = null)
        {
            StringBuilder sb = new StringBuilder();
            ApplySetting(setting);
            //欄位標頭列
            if ((DataSource != null && DataSource.Rows.Count > 0) || ShowHeaderWhenNoData)
            {
                sb.Append(RenderColumeHeader());
            }
            //資料列
            sb.Append(RenderRows());

            return sb.ToString();
        }

        /// <summary>
        /// 生成表格欄位標頭列Html
        /// </summary>
        /// <returns></returns>
        public string RenderColumeHeader()
        {
            //表格資料
            DataTable dt;
            if (DataSource != null)
            {
                dt = DataSource;
            }
            else
            {
                dt = new DataTable();
            }
            string headerRow = MakeHeaderRowHtml(dt);
            return headerRow;
        }

        /// <summary>
        /// 生成表格列Html
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string RenderRows()
        {
            StringBuilder sb = new StringBuilder();

            //表格資料
            DataTable dt;
            if (DataSource != null)
            {
                dt = DataSource;
            }
            else
            {
                dt = new DataTable();
            }

            //查無資料
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0)
            {
                string emptyRow = $"<tr><td style=\"width:100%;text-align:center\">{NoDataText}</td></tr>";
                sb.AppendLine(emptyRow);
                return sb.ToString();
            }

            int colCount = dt.Columns.Count;
            //鍵值欄位index
            int[] keyFieldIndexs = FindKeyFieldIndex(dt);
            if (keyFieldIndexs.Length == 0 && (EnableRowSelect || AddEditButton || AddDeleteButton))
            {
                throw new ArgumentException($"KeyFieldName [{KeyFieldName}] is not defined in the datasource column or field name ");
            }
            //鍵值data-attribute
            string dataKeyAttr = "";
            string dataKeyValue = "";
            string rowHtml; //串接用
            //string tdCssClass = ""; 
            string tabIndexAttr = "";//row tab-index for focusable
            string alterRowCss = EnableAlternativeRowStyle ? $" class=\"alter-row\"" : ""; //單偶數列css style
            //string[] cssArr = MakeTextAlignCssClass(dt); //文字對齊css class
            string colValue = ""; //欄位值
            var converters = PrepareConverters(dt); //欄位值轉換器
            var encodeFlags = PrepareHtmlEncodeFlags(dt);
            var colCssClass = PrepareColumnCssClass(dt); //欄位css class

            if (EnableRowSelect)
            {
                tabIndexAttr = $" tabindex=\"{new Random().Next(10000, 20000)}\"";
            }

            #region 資料列
            sb.Append($"<tbody{alterRowCss}>");

            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                rowHtml = "";
                DataRow row = dt.Rows[rowIndex];

                //鍵值屬性
                dataKeyValue = "";
                if (keyFieldIndexs.Length > 0)
                {
                    for (int keyIndex = 0; keyIndex < keyFieldIndexs.Length; keyIndex++)
                    {
                        if (keyIndex > 0) dataKeyValue += ",";
                        dataKeyValue += row[keyFieldIndexs[keyIndex]]?.ToString() ?? "";
                    }
                }

                dataKeyAttr = $" {DATA_KEY_ATTR_NAME}=\"{HtmlEncode(dataKeyValue)}\"";

                //序號
                if (AddSerialNo)
                {
                    rowHtml += $"<td class=\"center\">{rowIndex + 1}</td>";
                }

                //資料欄位
                for (int j = 0; j < colCount; j++)
                {
                    colValue = converters[j](row[j])?.ToString() ?? "";
                    if (encodeFlags[j]) colValue = HtmlEncode(colValue);
                    rowHtml += $"<td{colCssClass[j]}>{colValue}</td>";
                }

                //命令按鈕
                if (AddEditButton || AddDeleteButton)
                {
                    rowHtml += "<td class=\"tb-row-cmd min-width-100\">";
                    //命令屬性
                    string cmdAttr = "";
                    if (AddEditButton)
                    {
                        //編輯按鈕
                        cmdAttr = $" {DATA_COMMAND_ATTR_NAME}=\"edit\"";
                        rowHtml += $"<button class=\"center edit\"{dataKeyAttr} {cmdAttr}>{EditButtonContent}</button>";

                    }
                    if (AddDeleteButton)
                    {
                        //刪除按鈕
                        cmdAttr = $" {DATA_COMMAND_ATTR_NAME}=\"delete\"";
                        rowHtml += $"<button class=\"center delete\"{dataKeyAttr} {cmdAttr}>{DeleteButtonContent}</button>";
                    }
                    rowHtml += "</td>";
                }

                sb.Append($"<tr{tabIndexAttr}{dataKeyAttr}>{rowHtml}</tr>\n");
            }
            sb.Append("</tbody>");
            #endregion

            return sb.ToString();
        }

        /// <summary>
        /// 參數檢核
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void SettingValidation()
        {
            //(1)ID為必要參數
            if (string.IsNullOrEmpty(Id))
            {
                throw new ArgumentException("缺少必要參數ID");
            }
            //(2)若EnableRowSelect=true,則必須指定KeyFieldName和RowSelectEventHandler
            if (EnableRowSelect && string.IsNullOrEmpty(KeyFieldName))
            {
                throw new ArgumentException("若EnableRowSelect=true,則必須指定KeyFieldName");
            }
            if (EnableRowSelect && string.IsNullOrEmpty(RowSelectEventHandler))
            {
                throw new ArgumentException("若EnableRowSelect=true,則必須指定RowSelectEventHandler");
            }

            //(3)若AddEditButton=true,則必須指定KeyFieldName和RowCommandEventHandler
            if (AddEditButton && string.IsNullOrEmpty(KeyFieldName))
            {
                throw new ArgumentException("若AddEditButton=true,則必須指定KeyFieldName");
            }
            if (AddEditButton && string.IsNullOrEmpty(RowCommandEventHandler))
            {
                throw new ArgumentException("若AddEditButton=true,則必須指定RowCommandEventHandler");
            }
        }

        /// <summary>
        /// 找出鍵值欄位索引
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private int[] FindKeyFieldIndex(DataTable dt)
        {
            if(dt == null || dt.Columns.Count == 0) return new int[] { };
            if(string.IsNullOrEmpty(KeyFieldName)) return new int[] { };
            var keyNames = KeyFieldName.Split(',');
            List<int> foundIndexs = new List<int>();
            foreach (var keyName in keyNames)
            {
                int index = -1;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].ColumnName == keyName.Trim())
                    {
                        index = i; break;
                    }
                }
                if(index > -1) 
                {
                    foundIndexs.Add(index);
                }
            }

            return foundIndexs.ToArray();
        }

        /// <summary>
        /// 產生欄位標頭列
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string MakeHeaderRowHtml(DataTable dt)
        {
            if (dt == null) return "";
            int colCount = dt.Columns.Count;
            
            StringBuilder sb = new StringBuilder();
            sb.Append("<thead>");
            sb.Append("<tr>");
            string headerText = "";
            string rwdClass = ""; //響應式
            if (colCount > 0)
            {
                //序號標頭
                if (AddSerialNo)
                {
                    sb.Append($"<th {DATA_FIELD_ATTR_NAME}=\"auto_serial_no\">{SerialNoHeaderText}</th>");
                }
                for (int i = 0; i < colCount; i++)
                {
                    rwdClass = "";
                    if (i < RWDColumnCssClass.Length)
                    {
                        rwdClass = $" class=\"{RWDColumnCssClass[i]}\"";
                    }

                    //欄位標頭
                    if (i < ColumnHeaders.Length)
                    {
                        headerText = ColumnHeaders[i];
                    }
                    else
                    {
                        headerText = dt.Columns[i].ColumnName;
                    }
                    sb.Append($"<th{rwdClass} {DATA_FIELD_ATTR_NAME}=\"{dt.Columns[i].ColumnName}\">{headerText}</th>");
                }
                //操作按鈕標頭
                if (AddEditButton || AddDeleteButton)
                {
                    sb.Append($"<th>{CommandButtonHeaderText}</th>");
                }
            }
            else
            {
                sb.Append($"<th>&nbsp;</th>\n");
            }
      
            sb.Append("</tr>");
            sb.Append("</thead>");
            string html = sb.ToString();
            return html;
        }

        /// <summary>
        /// 觸發RowSelect事件和DoubleClick事件javascript
        /// </summary>
        /// <returns></returns>
        private string PrepareRowEventScript()
        {
            /*例用focus事件模擬列選取事件*/
            /*DoubleClick事件因為手機支援性不足，可改用click+focus來模擬*/

            string rowSelectHandler = RowSelectEventHandler.Replace("()", "");
            string rowCommandHandler = RowCommandEventHandler.Replace("()", "");

            StringBuilder sb =  new StringBuilder();
            sb.AppendLine("<script>");
            if (EnableRowSelect) {
                sb.AppendLine($"RegisterTableRowSelectedEvent('{this.TableClientId}',{rowSelectHandler});");
            }
            
            if(AddEditButton)
            {
                sb.AppendLine($"RegisterTableRowCommandEvent('{this.TableClientId}',{rowCommandHandler});");
            }
            sb.AppendLine("</script>");
            string script = sb.ToString();
            return script;
        }

        /// <summary>
        /// 備妥欄位文字對齊css class 陣列
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string[] PrepareTextAlignCssClass(DataTable dt)
        {
            if (dt == null||dt.Columns.Count==0) return new string[] {""};
            if (TextAlign == null || TextAlign.Length == 0) {
                string[] cssArr = "left".RepeatToArray(dt.Columns.Count);
                return cssArr;
            }
            if(TextAlign.Length == 1)
            {
                string cssClass = TextAlign[0].ToString()!.ToLower();
                string[] cssArr = cssClass.RepeatToArray(dt.Columns.Count);
                return cssArr;
            }
            else
            {
                string[] cssArr = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if(i < TextAlign.Length)
                    {
                        cssArr[i] = TextAlign[i].ToString()!.ToLower();
                    }
                    else
                    {
                        cssArr[i] = "";
                    }
                }
                return cssArr;
            }
        }

        /// <summary>
        /// 備妥轉換器
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private Func<object, object>[] PrepareConverters(DataTable dt)
        {
            if (dt == null || dt.Columns.Count == 0) return new Func<object, object>[] { x => x };  //虛的，無作用
            var converters = new Func<object, object>[dt.Columns.Count];
            for (int i = 0; i < converters.Length; i++)
            {
                if (ValueConverters.ContainsKey(dt.Columns[i].ColumnName))
                {
                    converters[i] = ValueConverters[dt.Columns[i].ColumnName];
                }
                else
                {
                    converters[i] = x => x;
                }
            }
            return converters;
        }

        /// <summary>
        /// 備妥HtmlEncode flag (true:要encode, false:不用encode)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool[] PrepareHtmlEncodeFlags(DataTable dt)
        {
            if (dt == null || dt.Columns.Count == 0) return new bool[0];  //虛的，無作用
            var flags = new bool[dt.Columns.Count];
            for (int i = 0; i < flags.Length; i++)
            {
                if (NoHtmlEncodeFields != null && NoHtmlEncodeFields.Count > 0 && NoHtmlEncodeFields.Contains(dt.Columns[i].ColumnName))
                {
                    flags[i] = false;    
                }
                else
                {
                    flags[i] = true;
                }
            }
            return flags;
        }

        /// <summary>
        /// 備妥欄位RWD css class
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string[] PrepareRWDCssClass(DataTable dt)
        {
            if (dt == null || dt.Columns.Count == 0) return new string[0];  //虛的，無作用
            var colArr = new string[dt.Columns.Count];
            for (int i = 0; i < colArr.Length; i++)
            {
                if(RWDColumnCssClass!=null && i< RWDColumnCssClass.Length)
                {
                    colArr[i] = RWDColumnCssClass[i];
                }
                else
                {
                    colArr[i] = "";
                }
            }
            return colArr;
        }

        /// <summary>
        /// 備妥欄位要套用的css class
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string[] PrepareColumnCssClass(DataTable dt)
        {
            if (dt == null || dt.Columns.Count == 0) return new string[0];  //虛的，無作用
            string[] alignArr = PrepareTextAlignCssClass(dt); //對齊
            string[] rwdArr = PrepareRWDCssClass(dt); //RWD
            var colArr = new string[dt.Columns.Count];
            for (int i = 0; i < colArr.Length; i++)
            {
                string? cssClass;
                _ = ColumnCssClass.TryGetValue(dt.Columns[i].ColumnName, out cssClass);
                if (string.IsNullOrEmpty(cssClass)) cssClass = "";

                colArr[i] = $" class=\"{alignArr[i]} {rwdArr[i]} {cssClass}\"";
            }
            return colArr; 
        }
    }
}

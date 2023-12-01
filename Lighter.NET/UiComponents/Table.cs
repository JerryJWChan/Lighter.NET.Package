namespace Lighter.NET.UiComponents
{
    using System;
    using System.Data;
    using System.Text;
    using System.Xml;
    using Lighter.NET.Common;

    /// <summary>
    /// 生成table的html tag(資料源:DataTable)
    /// </summary>
    public class Table: UiElementBase
    {
        public override UiElementType ElementType => UiElementType.Table;
        /// <summary>
        /// 表格資料來源(DataTable)
        /// </summary>
        public DataTable? DataSource { get; set; }
        /// <summary>
        /// 欄位定義
        /// </summary>
        public List<Column>? Columns { get; set; }
        /// <summary>
        /// 命令按鈕定義(例如：編輯、刪除…)
        /// </summary>
        public List<RowCommand>? RowCommands { get; set; } 
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
        /// 對應至 table的class屬性
        /// </summary>
        public new string CssClass { get; set; } = "bright";  //預設值bright須搭配lighter.css才有效
		/// <summary>
		/// 欄位標頭列的class屬性
		/// </summary>
		public string HeaderCssClass { get; set; } = "";
		/// <summary>
		/// 對應至 tr的class屬性
		/// </summary>
		public string RowCssClass { get; set; } = "";
		/// <summary>
		/// 對應至 td的class屬性
		/// </summary>
		public string CellCssClass { get; set; } = "padding-y-m"; //預設值padding-y-m須搭配lighter.css才有效
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
        /// 欄位文字水平對齊css class(若欄位沒有個別指定，則統一套用此class)，預設值：Center
        /// </summary>
        public TextAlign TextAlign { get; set; } = TextAlign.Center;
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
        /// 列選取事件處理函式(client端的javascript)名稱
        /// </summary>
        public string RowSelectEventHandler { get; set; } = "";

        /// <summary>
        /// 列操作命令(例如edit,delete...)事件處理函式(client端的javascript)名稱
        /// </summary>
        public string RowCommandEventHandler { get; set; } = "";
        /// <summary>
        /// 表格欄位標頭的HTML(若有給值則直接顯示此HTML，若未給值，則自動以欄位定義來生成欄位標頭)
        /// (內含thead > tr > th)
        /// </summary>
        public string TableHeadHTML { get; set; } = "";
        /// <summary>
        /// RWD切割點(當表格寬度縮小至側邊欄狀態時，大於此切割點的部分欄位可隱藏)
        /// </summary>
        public Rwd RwdCutPoint { get; set; } = Rwd.xs;
        /// <summary>
        /// 表格種類(預設:DataRowList)
        /// </summary>
        public TableType TableType { get; set; } = TableType.DataRowList;
        /// <summary>
        /// 生成table的html tag(資料源:DataTable)
        /// </summary>
        public Table() { }
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
            ValidateSetting();

            string cssClassAttr = $"class=\"{CssClass}\"";
            string rwdCutPointAttr = $"data-rwd-cutLevel=\"{((int)RwdCutPoint)}\"";
            string tableTypeAttr = $"data-table-type=\"{TableType.ToString()}\"";
            string innerHTML = RenderInnerHTML();
            string script = PrepareRowEventScript();

            StringBuilder sb= new StringBuilder();
            sb.AppendLine($"<div id=\"{ContainerClientId}\" class=\"table-container\">");
            sb.AppendLine($"<table id=\"{TableClientId}\" {cssClassAttr} {rwdCutPointAttr} {tableTypeAttr}>");
            sb.AppendLine(innerHTML);
            sb.AppendLine("</table>");

            //查無資料
            string noDataVisibility = (DataSource == null || DataSource.Columns.Count == 0 || DataSource.Rows.Count == 0) ? "" : "hide";
            string emptyRow = $"<div style=\"width:100%;text-align:center\" class=\" margin-top-xl {noDataVisibility}\" data-no-data>{NoDataText}</div>";
            sb.AppendLine(emptyRow);

            sb.AppendLine("</div>");
            //javascript
            sb.AppendLine(script);
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
            if(!string.IsNullOrEmpty(TableHeadHTML)) return TableHeadHTML;

            if (Columns == null || Columns.Count == 0) return "";

            int colCount = Columns.Count;

            StringBuilder sb = new StringBuilder();
            sb.Append("<thead>");
            sb.Append("<tr>");
            string headerText = "";
            string headerClass = ""; //header的css class
            string rwdAttr = "";     //data-column-rwd-level屬性
            string visibleStyle = ""; //可視性style
			string requiredAttr = ""; //是否必填 data-required屬性
			string uniqueAttr = ""; //是否唯一值 data-unique屬性

			if (colCount > 0)
            {
                //序號標頭
                if (AddSerialNo)
                {
                    sb.Append($"<th {DATA_FIELD_ATTR_NAME}=\"auto_serial_no\">{SerialNoHeaderText}</th>");
                }

                foreach (var col in Columns)
                {
                    if (string.IsNullOrEmpty(col.RwdCssClass)) col.RwdCssClass = $"rwd-{col.RwdLevel}";

                    //CSS
                    headerClass = $" class=\"center {this.HeaderCssClass} {col.HeaderCssClass} {col.RwdCssClass}\"";

                    //RWD
                    rwdAttr = $" {DATA_COLUMN_RWD_LEVEL_ATTR_NAME}=\"{((int)col.RwdLevel)}\"";

                    //欄位標頭
                    if (!string.IsNullOrEmpty(col.HeaderText))
                    {
                        headerText = col.HeaderText;
                    }
                    else
                    {
                        headerText = col.FieldName;
                    }

                    //可視性
                    visibleStyle = "";
                    if (col.Visible == false)
                    {
                        visibleStyle = "display:none;";
                    }

					requiredAttr = ""; //是否必填 data-required屬性
					if (col.IsRequired)
					{
						requiredAttr = $"{DATA_REQUIRED_ATTR_NAME}=\"true\"";
					}

					uniqueAttr = ""; //是否唯一值 data-unique屬性
					if (col.IsUnique)
					{
						uniqueAttr = $"{DATA_UNIQUE_ATTR_NAME}=\"true\"";
					}

					sb.Append($"<th{headerClass} {DATA_FIELD_ATTR_NAME}=\"{col.FieldName}\" {RequiredDataAttribute()}  {requiredAttr} {uniqueAttr} {rwdAttr} {DATA_CAN_HIDE_ATTR_NAME}=\"{col.CanHide.ToString().ToLower()}\" style=\"{visibleStyle}\">{headerText}</th>");
                }

                //操作按鈕標頭
                if (RowCommands != null && RowCommands.Count > 0)
                {
                    sb.Append($"<th>{CommandButtonHeaderText ?? "&nbsp;"}</th>");
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
        /// 生成表格列Html
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string RenderRows()
        {

			//鍵值data-attribute
			string dataKeyAttr = "";
			string dataKeyValue = "";
			string rowHtml; //串接用
							//string tdCssClass = ""; 
			string tabIndexAttr = "";//row tab-index for focusable
			string alterRowCss = EnableAlternativeRowStyle ? $" class=\"alter-row\"" : ""; //單偶數列css style
			string colValue = ""; //欄位值
			string textAlignCssClass = ""; //文字對齊 css class
			string textAlignStyle = ""; //文字對齊 style
			string visibleStyle = ""; //可視性style
			string whenCssClass = ""; //條件式 css class
			string whenCssKey = ""; ////條件式 css class key值

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
                return $"<tbody data-row-count=\"0\" {alterRowCss}></tbody>";
            }

            //檢核參數設定
            ValidateSetting();

            int colCount = dt.Columns.Count;
            //鍵值欄位index
            int[] keyFieldIndexs = FindKeyFieldIndex(dt);
			StringBuilder sb = new StringBuilder();

			if (EnableRowSelect)
            {
                tabIndexAttr = $" tabindex=\"{new Random().Next(10000, 20000)}\"";
            }

            #region 資料列
            sb.Append($"<tbody data-row-count=\"{dt.Rows.Count}\" {alterRowCss}>");

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
                    rowHtml += $"<td class=\"center rwd-xs\">{rowIndex + 1}</td>";
                }

                //資料欄位
                foreach (var col in Columns!)
                {
                    //文字對齊
                    textAlignCssClass = "";
                    textAlignStyle = "";
                    if (col.TextAlign != TextAlign.None) //欄位個別定義
                    {
                        textAlignCssClass = col.TextAlign.ToString().ToLower();
                        textAlignStyle = $"text-align:{col.TextAlign.ToString().ToLower()};";
                    }
                    else if (this.TextAlign != TextAlign.None) //table屬性定義
                    {
                        textAlignCssClass = this.TextAlign.ToString().ToLower();
                        textAlignStyle = $"text-align:{col.TextAlign.ToString().ToLower()};";
                    }

                    //條件式Css Class
                    whenCssKey = row[col.FieldName]?.ToString()??"";
                    if (col.WhenCssClass.ContainsKey(whenCssKey))
                    {
                        whenCssClass = col.WhenCssClass[whenCssKey];
                    }
                    else
                    {
                        whenCssClass = "";
                    }

                    //值轉換
                    if (col.Converter != null)
                    {
                        colValue = col.Converter(row[col.FieldName])?.ToString() ?? "";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(col.Format))
                        {
                            //有格式
                            colValue = GetFormattedValue(row[col.FieldName], col.Format);
                        }
                        else
                        {
                            //無格式
                            colValue = row[col.FieldName]?.ToString() ?? "";
                        }
                    }

                    //HtmlEncode
                    if (col.HtmlEncode)
                    {
                        colValue = HtmlEncode(colValue);
                    }

                    visibleStyle = "";
                    //可視性
                    if (col.Visible == false)
                    {
                        visibleStyle = "display:none;";
                    }

                    rowHtml += $"<td class=\"{textAlignCssClass} {col.FieldCssClass} {col.RwdCssClass} {whenCssClass}\" style=\"{visibleStyle}{textAlignStyle}\">{colValue}</td>";
                }

                //命令按鈕
                if(RowCommands != null && RowCommands.Count > 0)
                {
                    rowHtml += "<td class=\"tb-row-cmd min-width-100 center\">";
                    //命令屬性
                    foreach (var cmd in RowCommands)
                    {
                        rowHtml += $"<button class=\"{cmd.CssClass} rwd-xs\"{dataKeyAttr} {DATA_COMMAND_ATTR_NAME}=\"{cmd.CommandName}\">{cmd.CommandText}</button>";
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
        private void ValidateSetting()
        {
            //確保欄位定義
            EnsureColumns();
            
            //(1)ID為必要參數
            if (string.IsNullOrEmpty(Id))
            {
                throw new ArgumentException("缺少必要參數ID");
            }
            
            //(2)檢核欄位設定
            if (Columns == null || Columns.Count == 0)
            {
                throw new MissingMemberException("The Columns property of Table is not set.");
            }

            //(3)若EnableRowSelect=true,則Columns定義中必須指定KeyField和RowSelectEventHandler
            if (EnableRowSelect && HasKeyFields() == false)
            {
                throw new ArgumentException("若EnableRowSelect=true,則Columns定義中必須至少指定一個KeyField");
            }
            if (EnableRowSelect && string.IsNullOrEmpty(RowSelectEventHandler))
            {
                throw new ArgumentException("若EnableRowSelect=true,則必須指定RowSelectEventHandler");
            }

            //(4)若有設定RowCommands(例如：編輯、刪除),則必須指定KeyFieldName和RowCommandEventHandler
            if (RowCommands != null && RowCommands.Count > 0 && HasKeyFields() == false)
            {
                throw new ArgumentException("若有定義RowCommands屬性,則Columns定義中必須至少指定一個KeyField");
            }
            if (RowCommands != null && RowCommands.Count > 0 && string.IsNullOrEmpty(RowCommandEventHandler))
            {
                throw new ArgumentException("若有定義RowCommands屬性,則必須指定RowCommandEventHandler");
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
            if(HasKeyFields()==false) return new int[] { };
            //if(string.IsNullOrEmpty(KeyFieldName)) return new int[] { };
            //var keyNames = KeyFieldName.Split(',');
            var keyNames = Columns?.Where(x=>x.IsKeyField)?.Select(x=>x.FieldName)?.ToArray()
                           ?? new string[] { };
            if(keyNames.Length == 0) return new int[] { };

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
        /// 是否有定義鍵值欄位
        /// </summary>
        /// <returns></returns>
        private bool HasKeyFields()
        {
            var keyField = Columns?.Where(x => x.IsKeyField).FirstOrDefault();
            return keyField != null;
        }

        /// <summary>
        /// 取得格式化後的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetFormattedValue(object? value, string format)
        {
            if (value == null) return "";
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;
                return dateTime.ToString(format);
            }
            else if (value is double)
            {
                var dbValue = (double)value;
                return dbValue.ToString(format);
            }
            else if (value is decimal)
            {
                var decValue = (decimal)value;
                return decValue.ToString(format);
            }
            else if (value is float)
            {
                var flValue = (float)value;
                return flValue.ToString(format);
            }
            else if (value is int)
            {
                var intValue = (int)value;
                return intValue.ToString(format);
            }
            else
            {
                return value?.ToString() ?? "";
            }

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
            if (string.IsNullOrEmpty(rowSelectHandler)) rowSelectHandler = "null";
            if (string.IsNullOrEmpty(rowCommandHandler)) rowCommandHandler = "null";

            StringBuilder sb =  new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine($"$Table('{this.TableClientId}',{rowSelectHandler},{rowCommandHandler})");
            //if (EnableRowSelect) {
            //    sb.AppendLine($"RegisterTableRowSelectedEvent('{this.TableClientId}',{rowSelectHandler});");
            //}

            //if(RowCommands != null && RowCommands.Count > 0)
            //{
            //    sb.AppendLine($"RegisterTableRowCommandEvent('{this.TableClientId}',{rowCommandHandler});");
            //}
            sb.AppendLine("</script>");
            string script = sb.ToString();
            return script;
        }

        /// <summary>
        /// 確保欄位定義
        /// </summary>
        /// <returns></returns>
        private void EnsureColumns()
        {
            if(Columns != null && Columns.Count> 0) return;
            Columns = new List<Column>();
            if (DataSource != null && DataSource.Columns.Count > 0)
            {
                foreach (DataColumn col in DataSource.Columns)
                {
                    Columns.Add(new Column(col.ColumnName,col.ColumnName));
                }
            }
        }
    }
}

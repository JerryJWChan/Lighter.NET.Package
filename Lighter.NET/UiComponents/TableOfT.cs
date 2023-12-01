namespace Lighter.NET.UiComponents
{
    using System;
    using System.Data;
    using System.Text;
    using System.Xml;
    using Lighter.NET.Common;
    using Microsoft.Extensions.Localization;

    /// <summary>
    /// 生成table的html tag(資料源:List<DataModel>)
    /// </summary>
    public class Table<TDataModel>: UiElementBase where TDataModel : class
    {
        private IStringLocalizer<TDataModel>? _localizer;
        public override UiElementType ElementType => UiElementType.Table;
        /// <summary>
        /// 表格資料來源(List<TDataModel>)
        /// </summary>
        public List<TDataModel>? DataSource { get; set; }
        /// <summary>
        /// 欄位定義
        /// </summary>
        public List<Column<TDataModel>> Columns { get; set; }  = new List<Column<TDataModel>>();
        /// <summary>
        /// 表格列是否可編輯(預設值：false)
        /// </summary>
        public bool CanEdit { get; set; } = false;
        /// <summary>
        /// 表格列是否可刪除(預設值：false)
        /// </summary>
        public bool CanDelete { get; set; } = false;
        /// <summary>
        /// 表格列是否可新增(預設值：false)
        /// </summary>
        public bool CanAdd { get; set; } = false;
        /// <summary>
        /// 儲存變更的action name(含路徑)
        /// </summary>
        public string SubmitActionName { get; set; } = "";
		/// <summary>
		/// 唯一性的欄位群組(以逗號分隔欄位名稱構成一組)
		/// </summary>
		public string? UniqueFieldsGroup { get; set; }   
        /// <summary>
        /// 命令按鈕定義(例如：編輯、刪除…)
        /// </summary>
        public List<RowCommand>? RowCommands { get; set; }
        /// <summary>
        /// 命令按鈕欄位的CssClass
        /// </summary>
        public string RowCommandsCssClass { get; set; } = "";
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
        /// 欄位標頭列的class屬性
        /// </summary>
        public string HeaderCssClass { get; set; } = "";
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
        /// 自動加上序號欄位
        /// </summary>
        public bool AddSerialNo { get; set; } = false;

        /// <summary>
        /// 自動序號欄位寬度(預設值：30px)
        /// </summary>
        public string SerialNoColumnWidth { get; set; } = "30px";

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
        /// 表格列取得focus所需的tab index(亂數指定一個)
        /// </summary>
        public int RowTabIndex { get; set; } = new Random().Next(10000, 20000);
        /// <summary>
        /// 文字不換行
        /// </summary>
        public bool TextNoWrap { get; set; } = false;
        /// <summary>
        /// 文字過長時以...表示
        /// </summary>
        public bool TextEllipsis { get; set; } = false;

        /// <summary>
        /// 隱藏欄位標頭列(預設值：false)
        /// </summary>
        public bool HideColumnHeader { get; set; } = false;

        /// <summary>
        /// 生成時，是否將DataSource的資料集包含在client的Lighter TableObject物件中(預設值：false)
        /// </summary>
        public bool IncludeClientData { get; set; } = false;
        /// <summary>
        /// 生成table的html tag(資料源:List<DataModel>)
        /// </summary>
        public Table() 
        {
			CssClass = "bright";  //預設值bright須搭配lighter.css才有效
			try
            {
			   //多語系字串轉換器(for欄位標頭)
				_localizer = LangHelper.GetLocalizer<TDataModel>();
            }
            catch (Exception ex)
            {
                Console.Write($"RenderableModel() create StringLocalizer failed. {ex.Message}");
            }
        }

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
             *          (若CanEdit, CanDelete, CanAdd，初始化table附屬的dataList)
             *      </script>
             * </div>
             * (上面的xxx表示此物件的ID屬性)
             */

            //ApplySetting(setting); (不需要)
            //ApplyMetaProperty(metaProp); (不需要)

            //參數檢核
            ValidateSetting();

            if (TextNoWrap) { AddClass("text-no-wrap"); }
            if (TextEllipsis) { AddClass("text-ellipsis"); }

            string cssClassAttr = $"class=\"{CssClass}\"";
            string rwdCutPointAttr = $"data-rwd-cutLevel=\"{((int)RwdCutPoint)}\"";
            string tableTypeAttr = $"data-table-type=\"{TableType.ToString()}\"";
            string tabIndexAttr = $" data-row-tabindex=\"{this.RowTabIndex}\"";

			string innerHTML = RenderInnerHTML();
            string script = PrepareScript();

			StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<div id=\"{ContainerClientId}\" class=\"table-container\">");
            sb.AppendLine($"<table id=\"{TableClientId}\" {cssClassAttr} {rwdCutPointAttr} {tableTypeAttr} {tabIndexAttr}>");
            sb.AppendLine(innerHTML);
            sb.AppendLine("</table>");
            //查無資料
            string noDataVisibility = (DataSource == null || DataSource.Count == 0)?"": "hide";
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
            if ((DataSource != null && DataSource.Count > 0) || ShowHeaderWhenNoData)
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
            if (!string.IsNullOrEmpty(TableHeadHTML)) return TableHeadHTML;

            if (Columns == null || Columns.Count == 0) return "";

            var props = typeof(TDataModel).GetProperties(); 

            int colCount = Columns.Count;

            string headerVisibility = HideColumnHeader ? $" class=\"hide\"" : "";

            StringBuilder sb = new StringBuilder();
            sb.Append($"<thead{headerVisibility}>");
            sb.Append("<tr>");
            string headerText = "";
            string headerClass = ""; //header的css class
            string rwdAttr = "";     //data-column-rwd-level屬性
            string visibleStyle = ""; //可視性style
            string widthStyle = "";   //欄寬style
            string requiredAttr = ""; //是否必填 data-required屬性
            string uniqueAttr = ""; //是否唯一值 data-unique屬性
                                      
            CalculateColumnWidth(Columns);//計算欄位寬度

			if (colCount > 0)
            {
                //序號標頭
                if (AddSerialNo)
                {
                    //寬度
                    widthStyle = $"width:{SerialNoColumnWidth} !important; min-width:{SerialNoColumnWidth} !important; max-width:{SerialNoColumnWidth} !important;";
                    sb.Append($"<th {DATA_FIELD_ATTR_NAME}=\"auto_serial_no\" style=\"{widthStyle}\">{SerialNoHeaderText}</th>");
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

                    //多語系
                    if (headerText == col.FieldName && _localizer != null)
                    {
                        var prop = props.FirstOrDefault(p=>p.Name== col.FieldName);
                        if(prop != null)
                        {
                            var metaProp = new MetaProperty(prop, null);
                            headerText = metaProp?.Display?.ShortName ?? metaProp?.Display?.Name ?? headerText;
						}
                        headerText = _localizer.GetString(headerText);
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

                    //寬度
                    widthStyle = $"width:{col.Width_caculated};";

					sb.Append($"<th{headerClass} {DATA_FIELD_ATTR_NAME}=\"{col.FieldName}\" {requiredAttr} {uniqueAttr} {rwdAttr} {DATA_CAN_HIDE_ATTR_NAME}=\"{col.CanHide.ToString().ToLower()}\" style=\"{widthStyle}{visibleStyle}\">{headerText}</th>");
                }

                //操作按鈕標頭
                if (RowCommands != null && RowCommands.Count > 0)
                {
                    string rowCommandColumnCssClassAttr = string.IsNullOrEmpty(RowCommandsCssClass) ? "" : $" class=\"RowCommandsCssClass\"";
                    sb.Append($"<th{rowCommandColumnCssClassAttr}>{CommandButtonHeaderText ?? "&nbsp;"}</th>");
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
            string dataKeyFieldAttr = "";
            string dataKeyFields = "";
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
			string dataFieldAttr = ""; //資料欄位名稱屬性
			
            //查無資料
			if (DataSource == null || DataSource.Count == 0)
            {
                return $"<tbody data-row-count=\"0\" {alterRowCss}></tbody>";
            }

            //檢核參數設定
            ValidateSetting();

            int rowCount = DataSource?.Count ?? 0;
            int colCount = Columns?.Count??0;
            //鍵值欄位取值函式陣列
            var keyValueGetters = GetKeyFieldValueGetters();
			//健值欄位名(若多組以逗號分隔)
			dataKeyFields = GetKeyFieldNames();
            if (dataKeyFields != "") dataKeyFieldAttr = $"{DATA_KEY_FIELD_ATTR_NAME}=\"{dataKeyFields}\"";

			StringBuilder sb = new StringBuilder();

			if (EnableRowSelect || CanEdit || CanAdd || CanDelete)
            {
                tabIndexAttr = $" tabindex=\"{this.RowTabIndex}\"";
            }

            #region 資料列
            sb.Append($"<tbody{alterRowCss}>");
            TDataModel model;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                model = DataSource![rowIndex];
                rowHtml = "";

                //鍵值欄位值(若多組以逗號分隔)
                dataKeyValue = "";

                if (keyValueGetters.Length > 0)
                {
                    for (int keyIndex = 0; keyIndex < keyValueGetters.Length; keyIndex++)
                    {
                        if (keyIndex > 0) dataKeyValue += ",";
                        dataKeyValue += keyValueGetters[keyIndex](model)?.ToString() ?? "";
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
                    //欄位名稱屬性
                    dataFieldAttr = (CanEdit||CanAdd||CanDelete) ? $"{DATA_FIELD_ATTR_NAME}=\"{col.FieldName}\"" : "";
                    
                    //文字對齊
                    textAlignCssClass = "";
                    textAlignStyle = "";
                    if (col.TextAlign != TextAlign.None) //欄位個別定義
                    {
                        textAlignCssClass = col.TextAlign.ToString().ToLower();
                        textAlignStyle = $"text-align:{col.TextAlign.ToString().ToLower()};";
                    }else if (this.TextAlign != TextAlign.None) //table屬性定義
                    {
                        textAlignCssClass = this.TextAlign.ToString().ToLower();
                        textAlignStyle = $"text-align:{col.TextAlign.ToString().ToLower()};";
                    }

                    //條件式Css Class
                    whenCssKey = col.ValueGetter(model)?.ToString() ?? "";
                    if (col.WhenCssClass.ContainsKey(whenCssKey))
                    {
                        whenCssClass = col.WhenCssClass[whenCssKey];
                    }
                    else
                    {
                        whenCssClass = "";
                    }

                    //欄位內容轉換
                    if(col.ContentConverter != null)
                    {
                        colValue = col.ContentConverter(col.ValueGetter(model), model)?.ToString()??"";
                    }
                    //值轉換
                    else if (col.Converter != null)
                    {
                        colValue = col.Converter(col.ValueGetter(model))?.ToString() ?? "";
                    }
                    else
                    {

                        if(col.TextGetter!= null) //有另外指定顯示用欄位
                        {
							if (!string.IsNullOrEmpty(col.Format))
							{
								//有格式
								colValue = GetFormattedValue(col.TextGetter(model), col.Format);
							}
							else
							{
								//無格式
								colValue = col.TextGetter(model)?.ToString() ?? "";
							}
						}
                        else
                        {
                            if (!string.IsNullOrEmpty(col.Format))
                            {
                                //有格式
                                colValue = GetFormattedValue(col.ValueGetter(model),col.Format);
                            }
                            else
                            {
                                //無格式
                                colValue = col.ValueGetter(model)?.ToString() ?? "";
                            }
                        }
      
                    }

                    //HtmlEncode
                    if (col.HtmlEncode)
                    {
                        colValue = HtmlEncode(colValue);
                    }

                    visibleStyle = "";
                    //可視性
                    if(col.Visible == false)
                    {
                        visibleStyle = "display:none;";
                    }

                    rowHtml += $"<td class=\"{textAlignCssClass} {col.FieldCssClass} {col.RwdCssClass} {whenCssClass}\" {dataFieldAttr} style=\"{visibleStyle}{textAlignStyle}\">{colValue}</td>";
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

                sb.Append($"<tr {tabIndexAttr} {dataKeyAttr} {dataKeyFieldAttr}>{rowHtml}</tr>\n");
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
            //(1)ID為必要參數
            if (WithId && string.IsNullOrEmpty(Id))
            {
                throw new ArgumentException("缺少必要參數ID");
            }
            //(2)檢核欄位設定
            if (Columns == null || Columns.Count == 0)
            {
                throw new MissingMemberException("The Columns property of Table<TDatModel> is not set.");
            }

            //(3)若EnableRowSelect=true,則Columns定義中必須指定KeyField和RowSelectEventHandler
            if (EnableRowSelect && HasKeyFields()==false)
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

            //(5)若有設定CanEdit=true,則必須設定SubmitActionName
            if(CanEdit && string.IsNullOrEmpty(SubmitActionName))
            {
                throw new ArgumentException("若有設定CanEdit=true,則必須設定SubmitActionName(儲存變更時要呼叫的action，包含路徑)");
            }

            //(6)若有設定CanAdd=true,則必須設定SubmitActionName
            if (CanAdd && string.IsNullOrEmpty(SubmitActionName))
            {
                throw new ArgumentException("若有設定CanAdd=true,則必須設定SubmitActionName(儲存變更時要呼叫的action，包含路徑)");
            }
        }

        /// <summary>
        /// 是否有定義鍵值欄位
        /// </summary>
        /// <returns></returns>
        private bool HasKeyFields()
        {
            var keyField = Columns?.Where(x=>x.IsKeyField).FirstOrDefault();
            return keyField != null;
        }

        /// <summary>
        /// 鍵值欄位(可能多欄)取值函式陣列
        /// </summary>
        /// <returns></returns>
        private Func<TDataModel, object?>[] GetKeyFieldValueGetters()
        {
            if(DataSource==null || DataSource.Count == 0 ) return new Func<TDataModel, object?>[0];
            var keyValueGetters = Columns?.Where(x => x.IsKeyField)?.Select(x => x.ValueGetter)?.ToArray() 
                                                                                              ?? new Func<TDataModel, object?>[0];
            return keyValueGetters;
        }

        /// <summary>
        /// 取得鍵值欄位名稱(若多組以逗號分隔)
        /// </summary>
        /// <returns></returns>
        private string GetKeyFieldNames()
        {
            if (DataSource == null || DataSource.Count == 0) return "";
            var fieldArr = Columns?.Where(x=>x.IsKeyField)?.Select(x=>x.FieldName)?.ToArray()??new string[0];
            if (fieldArr.Length == 0) return "";
            if(fieldArr.Length == 1)
            {
				return fieldArr[0];
			}
            else if (fieldArr.Length > 1)
            {
                return string.Join(",", fieldArr);
            }
            else
            {
                return "";
            }

		}

        /// <summary>
        /// 取得格式化後的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetFormattedValue(object? value, string format)
        {
            if (value == null) return "";
            if(value is DateTime)
            {
                var dateTime = (DateTime)value;
                return dateTime.ToString(format);
            }else if (value is double)
            {
                var dbValue = (double)value;
                return dbValue.ToString(format);
            }else if(value is decimal)
            {
                var decValue = (decimal)value;
                return decValue.ToString(format);
            }else if(value is float)
            {
                var flValue = (float)value;
                return flValue.ToString(format);
            }else if (value is int)
            {
                var intValue = (int)value;
                return intValue.ToString(format);
            }
            else
            {
                return value?.ToString()??"";
            }

        }

        /// <summary>
        /// 產生table運作所需的javascript
        /// </summary>
        /// <returns></returns>
        private string PrepareScript()
        {
			/*
			 * (註冊row的focus事件，做為RowSelect事件，使觸發RowSelectEventHandler)
             *(若CanEdit,CanAdd,CanDelete，初始化table附屬的dataList)
             */

			/*例用focus事件模擬列選取事件*/
			/*DoubleClick事件因為手機支援性不足，可改用click+focus來模擬*/

			string rowSelectHandler = RowSelectEventHandler.Replace("()", "");
            string rowCommandHandler = RowCommandEventHandler.Replace("()", "");
            if (string.IsNullOrEmpty(rowSelectHandler)) rowSelectHandler = "null";
            if (string.IsNullOrEmpty(rowCommandHandler)) rowCommandHandler = "null";

            string tableVar = $"__{this.TableClientId}_temp";

            StringBuilder sb =  new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine($"let {tableVar} = $Table('{this.TableClientId}',{rowSelectHandler},{rowCommandHandler},{this.CanEdit.ToString().ToLower()})");
            if (CanAdd)
            {
                sb.AppendLine($"{tableVar}.canAdd = true;");
            }

            if (CanDelete) {
                sb.AppendLine($"{tableVar}.canDelete = true;");
            }

            //if (EnableRowSelect) {
            //    sb.AppendLine($"RegisterTableRowSelectedEvent('{this.TableClientId}',{rowSelectHandler});");
            //}

            //if(RowCommands != null && RowCommands.Count > 0)
            //{
            //    sb.AppendLine($"RegisterTableRowCommandEvent('{this.TableClientId}',{rowCommandHandler});");
            //}

            if (CanEdit || CanAdd || CanDelete)
            {
                if(this.DataSource != null && this.DataSource.Count > 0)
                {
					sb.AppendLine($"{tableVar}.initializeDataList('{System.Text.Json.JsonSerializer.Serialize(this.DataSource)}','{this.SubmitActionName}');");
				}
                else
                {
					sb.AppendLine($"{tableVar}.initializeDataList(undefined,'{this.SubmitActionName}');");
				}
            }

            if (!string.IsNullOrEmpty(UniqueFieldsGroup))
            {
				sb.AppendLine($"{tableVar}.addUniqueFieldsGroup('{UniqueFieldsGroup}');");
			}

            sb.AppendLine("</script>");
            string script = sb.ToString();
            return script;
        }

		/// <summary>
		/// 計算欄位寬度
		/// </summary>
		/// <param name="columns">欄位定義集</param>
		/// <returns></returns>
		private void CalculateColumnWidth(List<Column<TDataModel>> columns)
		{

			if (columns == null || columns.Count == 0)
			{
                return;
			}

			//依比例計算欄位寬度
			//有定義的欄位寬度比例
			var columnRatios = columns.Select(x => x.Width_ratio);
			//有定義的固定欄寬合計
			double fixWidthTotal = columns.Sum(x => x.Width_px);
			//比例合計
			double totalRatio = columnRatios!.Sum();
			foreach (var col in columns)
			{
				//若有設固定欄寬，優先採用
				if (col.Width_px > 0)
				{
                    col.Width_caculated = $"{col.Width_px}px";
				}
				//其次依比例計算欄寬
				else
				{
                    var percent = (col.Width_ratio / totalRatio) * 100;
					col.Width_caculated = $"{percent}%";
				}
			}

		}
	}
}

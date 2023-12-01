namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// 表格欄位(定義)
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 資料型別
        /// </summary>
        public string DataType { get; set; } = "";
        /// <summary>
        /// 對應的資料欄位名稱
        /// </summary>
        public string FieldName { get; set; } = "";
        /// <summary>
        /// 資料欄位CssClass(若有多組，用逗號分隔)
        /// </summary>
        public string FieldCssClass { get; set; } = "padding-y-m"; //預設值padding-y-m須搭配lighter.css才有效
		/// <summary>
		/// 欄標頭文字
		/// </summary>
		public string HeaderText { get; set; } = "";
        /// <summary>
        /// 欄標頭CssClass(若有多組，用逗號分隔)
        /// </summary>
        public string HeaderCssClass { get; set; } = "";
        /// <summary>
        /// 欄位RWD CssClass(決定不同畫面尺寸下各欄位的顯示與否)
        /// </summary>
        public string RwdCssClass { get; set; } = "";
        /// <summary>
        /// 欄位RWD分級(參照Rwd列舉)
        /// </summary>
        public Rwd RwdLevel { get; set; }
        /// <summary>
        /// 文字對齊(水平)
        /// </summary>
        public TextAlign TextAlign { get; set; } = TextAlign.None;
        /// <summary>
        /// 文字格式
        /// </summary>
        public string Format { get; set; } = "";
        /// <summary>
        /// 是否將欄位內容HtmlEncode處理(預設:true)
        /// </summary>
        public bool HtmlEncode { get; set; } = true;
        /// <summary>
        /// 是否鍵值欄位
        /// </summary>
        public bool IsKeyField { get; set; } = false;
        /// <summary>
        /// 當表格縮至側邊面版時，是否可隱藏此欄位
        /// </summary>
        public bool CanHide { get; set; } = false;
        /// <summary>
        /// 是否可視
        /// </summary>
        public bool Visible { get; set; } = true;
        /// <summary>
        /// 是否必填欄位
        /// </summary>
        public bool IsRequired { get; set; } = false;
        /// <summary>
        /// 是否唯一值欄位
        /// </summary>
        public bool IsUnique { get; set; } = false;
        /// <summary>
        /// 最小數值或長度
        /// </summary>
        public int Minimum { get; set; }
        /// <summary>
        /// 最大數值或長度
        /// </summary>
        public int Maximum { get; set; }
        /// <summary>
        /// 欄位值轉換器(參數只包含欄位值)
        /// </summary>
        public Func<object?, object>? Converter { get; set; }
		/// <summary>
		/// 條件顯示
		/// </summary>
		public Dictionary<string, string> WhenCssClass = new Dictionary<string, string>();
        /// <summary>
        /// 子欄位(第二階欄位)
        /// </summary>
        public List<Column> ChildColumns { get; set; } = new List<Column>();
		/// <summary>
		/// 欄寬(比例)
		/// </summary>
		public double Width_ratio { get; set; } = 1;
		/// <summary>
		/// 欄寬(單位：px)(預設值：0，表示不固定欄位，改由Width_ratio自動依照比例決定欄寬)
		/// </summary>
		public double Width_px { get; set; } = 0;
        /// <summary>
        /// 計算後的欄寬屬性值(長度單位：px或%)
        /// </summary>
        public string Width_caculated = "";
		protected Column(){}
        /// <summary>
        /// 欄位定義
        /// </summary>
        /// <param name="fieldName">欄位名</param>
        public Column(string fieldName)
        {
            FieldName = fieldName;
            HeaderText = fieldName;
        }

        /// <summary>
        /// 欄位定義
        /// </summary>
        /// <param name="fieldName">欄位名</param>
        /// <param name="headerText">欄位標頭文字</param>
        public Column(string fieldName, string headerText)
        {
            FieldName = fieldName;
            HeaderText = headerText;
        }
        /// <summary>
        /// 欄位定義
        /// </summary>
        /// <param name="fieldName">欄位名</param>
        /// <param name="headerText">欄位標頭文字</param>
        /// <param name="rwdLevel">RWD等級</param>
        public Column(string fieldName, string headerText, Rwd rwdLevel)
        {
            FieldName = fieldName;
            HeaderText = headerText;
            RwdCssClass = $"rwd-{rwdLevel.ToString()}";
            RwdLevel = rwdLevel;
        }
        /// <summary>
        /// 欄位定義
        /// </summary>
        /// <param name="fieldName">欄位名</param>
        /// <param name="headerText">欄位標頭文字</param>
        /// <param name="fieldCssClass">欄位Css class</param>
        public Column(string fieldName,string headerText, string fieldCssClass) 
        { 
            FieldName= fieldName;
            HeaderText = headerText;
            FieldCssClass = fieldCssClass;
        }
        /// <summary>
        /// 欄位定義
        /// </summary>
        /// <param name="fieldName">欄位名</param>
        /// <param name="headerText">欄位標頭文字</param>
        /// <param name="fieldCssClass">欄位Css class</param>
        /// <param name="rwdLevel">RWD等級</param>
        public Column(string fieldName, string headerText, string fieldCssClass, Rwd rwdLevel)
        {
            FieldName = fieldName;
            HeaderText = headerText;
            FieldCssClass = fieldCssClass;
            RwdCssClass = $"rwd-{rwdLevel.ToString()}";
            RwdLevel = rwdLevel;
        }

        /// <summary>
        /// 欄位定義
        /// </summary>
        /// <param name="fieldName">欄位名</param>
        /// <param name="headerText">欄位標頭文字</param>
        /// <param name="isKeyField">是否鍵值欄位</param>
        /// <param name="visible">是否顯示</param>
        /// <param name="format">格式pattern</param>
        /// <param name="fieldCssClass">欄位Css class</param>
        /// <param name="rwdLevel">RWD等級</param>
        public Column(string fieldName, string headerText,bool isKeyField, bool visible, string format, string fieldCssClass, Rwd rwdLevel)
        {
            FieldName = fieldName;
            HeaderText = headerText;
            IsKeyField = isKeyField;
            Visible = visible;
            Format = format;
            FieldCssClass = fieldCssClass;
            RwdCssClass = $"rwd-{rwdLevel.ToString()}";
            RwdLevel = rwdLevel;
        }
        /// <summary>
        /// 設定欄位RWD顯示分段方式
        /// </summary>
        /// <param name="rwdLevel"></param>
        /// <returns></returns>
        public virtual Column Rwd(Rwd rwdLevel)
        {
            RwdCssClass = $"rwd-{rwdLevel.ToString()}";
            this.RwdLevel = rwdLevel;
            return this;
        }

        /// <summary>
        /// 設定必填欄位
        /// </summary>
        /// <returns></returns>
        public virtual Column Required()
        {
            this.IsRequired = true;
            return this;
        }

        /// <summary>
        /// 設定唯一值欄位
        /// </summary>
        /// <returns></returns>
        public virtual Column Unique()
        {
            this.IsUnique = true;
            return this;
        }

		/// <summary>
		/// 設定數值或長度下
		/// </summary>
		/// <param name="min">數值或長度下限</param>
		/// <returns></returns>
		public virtual Column Min(int min)
        {
            this.Minimum= min;
            return this;
        }

		/// <summary>
		/// 設定數值或長度上限
		/// </summary>
		/// <param name="max">數值或長度上限</param>
		/// <returns></returns>
		public virtual Column Max(int max)
		{
			this.Maximum = max;
			return this;
		}


		/// <summary>
		/// 條件顯示
		/// </summary>
		/// <param name="value">當欄位值等於value時</param>
		/// <param name="cssClass">要套用的css class</param>
		/// <returns></returns>
		public virtual Column When(object value, string cssClass)
        {
            string key = value.ToString()??"";
            if (key == "") return this;
            if (!WhenCssClass.ContainsKey(key))
            {
                WhenCssClass.Add(key, cssClass);
            }
            return this;
        }
        /// <summary>
        /// 加入子欄位
        /// </summary>
        /// <param name="childColumns"></param>
        public virtual void AddChildColumns(params Column[] childColumns)
        {
            this.ChildColumns.AddRange(childColumns);
        }
    }

}

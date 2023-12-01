using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Lighter.NET.Common;

namespace Lighter.NET.UiComponents
{
	/// <summary>
	/// 表格欄位(定義)(泛型)
	/// </summary>
	/// <typeparam name="TDataModel"></typeparam>
	public class Column<TDataModel> : Column where TDataModel : class
	{
		/// <summary>
		/// 取值函式
		/// </summary>
		private Func<TDataModel, object?> _valueGetter;
		/// <summary>
		/// 取顯示文字函式
		/// </summary>
		private Func<TDataModel, object?>? _textGetter;

        /// <summary>
        /// 欄位內容轉換器(參數包含欄位值和整列的data model)
        /// </summary>
        public Func<object?, TDataModel, object>? ContentConverter { get; set; }

        /// <summary>
        /// 子欄位(第二階欄位)
        /// </summary>
        public new List<Column<TDataModel>> ChildColumns { get; set; } = new List<Column<TDataModel>>();
		private Column()
		{
			_valueGetter = x => "";  //虛設
		}
		private Column(string headerText)
		{
			HeaderText = headerText;
			_valueGetter = x => "";  //虛設
		}

		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			HeaderText = FieldName;
			_valueGetter = columnSelector.Compile();
		}

		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="textColumnSelector">欄位顯示文字選擇子</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector, Expression<Func<TDataModel, object?>> textColumnSelector)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			HeaderText = FieldName;
			_valueGetter = columnSelector.Compile();
			_textGetter = textColumnSelector.Compile();
		}

		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector, string headerText)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			HeaderText = headerText;
			_valueGetter = columnSelector.Compile();
		}

		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="textColumnSelector">欄位顯示文字選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector, Expression<Func<TDataModel, object?>> textColumnSelector, string headerText)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			HeaderText = headerText;
			_valueGetter = columnSelector.Compile();
			_textGetter = textColumnSelector.Compile();
		}

		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="rwdLevel">RWD等級</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector, string headerText, Rwd rwdLevel)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			HeaderText = headerText;
			RwdCssClass = $"rwd-{rwdLevel.ToString()}";
			RwdLevel = rwdLevel;
			_valueGetter = columnSelector.Compile();
		}

		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="fieldCssClass">欄位Css class</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector, string headerText, string fieldCssClass)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			HeaderText = headerText;
			FieldCssClass = fieldCssClass;
			_valueGetter = columnSelector.Compile();
		}
		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="fieldCssClass">欄位Css class</param>
		/// <param name="rwdLevel">RWD等級</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector, string headerText, string fieldCssClass, Rwd rwdLevel)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			HeaderText = headerText;
			FieldCssClass = fieldCssClass;
			RwdCssClass = $"rwd-{rwdLevel.ToString()}";
			RwdLevel = rwdLevel;
			_valueGetter = columnSelector.Compile();
		}

		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="isKeyField">是否鍵值欄位</param>
		/// <param name="visible">是否顯示</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector, bool visible, bool isKeyField)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			IsKeyField = isKeyField;
			Visible = visible;
			_valueGetter = columnSelector.Compile();
		}

        /// <summary>
        /// 欄位定義
        /// </summary>
        /// <param name="columnSelector">欄位值選擇子</param>
        /// <param name="textColumnSelector">欄位顯示文字選擇子</param>
        /// <param name="isKeyField">是否鍵值欄位</param>
        /// <param name="visible">是否顯示</param>
        public Column(Expression<Func<TDataModel, object?>> columnSelector, Expression<Func<TDataModel, object?>> textColumnSelector, bool visible, bool isKeyField)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			IsKeyField = isKeyField;
			Visible = visible;
			_valueGetter = columnSelector.Compile();
			_textGetter = textColumnSelector.Compile();

		}

		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="isKeyField">是否鍵值欄位</param>
		/// <param name="visible">是否顯示</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector, string headerText, bool isKeyField, bool visible)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			HeaderText = headerText;
			IsKeyField = isKeyField;
			Visible = visible;
			_valueGetter = columnSelector.Compile();
		}

		/// <summary>
		/// 欄位定義
		/// </summary>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="isKeyField">是否鍵值欄位</param>
		/// <param name="visible">是否顯示</param>
		/// <param name="format">格式pattern</param>
		/// <param name="fieldCssClass">欄位Css class</param>
		/// <param name="rwdLevel">RWD等級</param>
		public Column(Expression<Func<TDataModel, object?>> columnSelector, string headerText, bool isKeyField, bool visible, string format, string fieldCssClass, Rwd rwdLevel)
		{
			FieldName = columnSelector.GetLambdaPropertyName();
			HeaderText = headerText;
			IsKeyField = isKeyField;
			Visible = visible;
			Format = format;
			FieldCssClass = fieldCssClass;
			RwdCssClass = $"rwd-{rwdLevel.ToString()}";
			RwdLevel = rwdLevel;
			_valueGetter = columnSelector.Compile();
		}
		/// <summary>
		/// 取值函式
		/// </summary>
		public Func<TDataModel, object?> ValueGetter
		{
			get { return _valueGetter; }
		}

		/// <summary>
		/// 取顯示文字函式
		/// </summary>
		public Func<TDataModel, object?>? TextGetter
		{
			get { return _textGetter; }
		}



		/// <summary>
		/// 設定欄位RWD顯示分段方式
		/// </summary>
		/// <param name="rwdLevel"></param>
		/// <returns></returns>
		public override Column<TDataModel> Rwd(Rwd rwdLevel)
		{
			RwdCssClass = $"rwd-{rwdLevel.ToString()}";
			RwdLevel = rwdLevel;
			return this;
		}

		/// <summary>
		/// 設定必填欄位
		/// </summary>
		/// <returns></returns>
		public override Column<TDataModel> Required()
		{
			this.IsRequired = true;
			return this;
		}

		/// <summary>
		/// 設定唯一值欄位
		/// </summary>
		/// <returns></returns>
		public override Column<TDataModel> Unique()
		{
			this.IsUnique = true;
			return this;
		}

		/// <summary>
		/// 設定數值或長度下限
		/// </summary>
		/// <param name="min">數值或長度下限</param>
		/// <returns></returns>
		public override Column Min(int min)
		{
			this.Minimum = min;
			return this;
		}

		/// <summary>
		/// 設定數值或長度上限
		/// </summary>
		/// <param name="max">數值或長度上限</param>
		/// <returns></returns>
		public override Column Max(int max)
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
		public override Column<TDataModel> When(object value, string cssClass)
		{
			string key = value.ToString() ?? "";
			if (key == "") return this;
			if (!WhenCssClass.ContainsKey(key))
			{
				WhenCssClass.Add(key, cssClass);
			}
			return this;
		}
		/// <summary>
		/// 加入子欄位(不適用於泛型欄位)
		/// </summary>
		/// <param name="childColumns"></param>
		public override void AddChildColumns(params Column[] childColumns)
		{
			throw new MemberAccessException("This method is not accessable for the generic subclass Column<TDataModel>. Call another overloaded method with parameter of type Column<TDataModel>");
		}
		/// <summary>
		/// 加入子欄位
		/// </summary>
		/// <param name="childColumns"></param>
		public void AddChildColumns(params Column<TDataModel>[] childColumns)
		{
			ChildColumns.AddRange(childColumns);
		}
	}

	/// <summary>
	/// Column of T 的延伸函式
	/// </summary>
	public static class ColumnOfTExtension
	{
		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		public static List<Column<TDataModel>> Add<TDataModel>(this List<Column<TDataModel>> columns, Expression<Func<TDataModel, object?>> columnSelector)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector);
			columns.Add(col);
			return columns;
		}

		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		public static List<Column<TDataModel>> Add<TDataModel>(this List<Column<TDataModel>> columns, Expression<Func<TDataModel, object?>> columnSelector, string headerText)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, headerText);
			columns.Add(col);
			return columns;
		}
		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="columnSetter">欄位設定式</param>
		public static List<Column<TDataModel>> Add<TDataModel>(this List<Column<TDataModel>> columns, Expression<Func<TDataModel, object?>> columnSelector, Action<Column<TDataModel>>? columnSetter)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector);
			if (columnSetter != null)
			{
				columnSetter(col);
			}
			columns.Add(col);
			return columns;
		}

		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="columnSelector">欄位顯示文字選擇子</param>
		/// <param name="columnSetter">欄位設定式</param>
		public static List<Column<TDataModel>> Add<TDataModel>(this List<Column<TDataModel>> columns, Expression<Func<TDataModel, object?>> columnSelector, Expression<Func<TDataModel, object?>> columnTextSelector, Action<Column<TDataModel>>? columnSetter)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, columnTextSelector);
			if (columnSetter != null)
			{
				columnSetter(col);
			}
			columns.Add(col);
			return columns;
		}

		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="columnSetter">欄位設定式</param>
		public static List<Column<TDataModel>> Add<TDataModel>(this List<Column<TDataModel>> columns, Expression<Func<TDataModel, object?>> columnSelector, string headerText, Action<Column<TDataModel>>? columnSetter)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, headerText);
			if (columnSetter != null)
			{
				columnSetter(col);
			}
			columns.Add(col);
			return columns;
		}
		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="rwdLevel">RWD等級</param>
		public static List<Column<TDataModel>> Add<TDataModel>(this List<Column<TDataModel>> columns, Expression<Func<TDataModel, object?>> columnSelector, string headerText, Rwd rwdLevel)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, headerText, rwdLevel);
			columns.Add(col);
			return columns;
		}
		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="fieldCssClass">欄位Css class</param>
		public static List<Column<TDataModel>> Add<TDataModel>(this List<Column<TDataModel>> columns, Expression<Func<TDataModel, object?>> columnSelector, string headerText, string fieldCssClass)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, headerText, fieldCssClass);
			columns.Add(col);
			return columns;
		}
		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="fieldCssClass">欄位Css class</param>
		/// <param name="rwdLevel">RWD等級</param>
		public static List<Column<TDataModel>> Add<TDataModel>(this List<Column<TDataModel>> columns, Expression<Func<TDataModel, object?>> columnSelector, string headerText, string fieldCssClass, Rwd rwdLevel)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, headerText, fieldCssClass, rwdLevel);
			columns.Add(col);
			return columns;
		}

		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="isKeyField">是否鍵值欄位</param>
		/// <param name="visible">是否顯示</param>
		public static List<Column<TDataModel>> Add<TDataModel>(
			this List<Column<TDataModel>> columns,
			Expression<Func<TDataModel, object?>> columnSelector,
			bool isKeyField,
			bool visible
			)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, visible, isKeyField);
			columns.Add(col);
			return columns;
		}

        /// <summary>
        /// 加入欄位定義
        /// </summary>
        /// <typeparam name="TDataModel"></typeparam>
        /// <param name="columns"></param>
        /// <param name="columnSelector">欄位值選擇子</param>
        /// <param name="columnTextSelector">欄位顯示文字選擇子</param>
        public static List<Column<TDataModel>> Add<TDataModel>(
            this List<Column<TDataModel>> columns,
            Expression<Func<TDataModel, object?>> columnSelector,
            Expression<Func<TDataModel, object?>> columnTextSelector
            )
        where TDataModel : class
        {
            var col = new Column<TDataModel>(columnSelector, columnTextSelector);
            columns.Add(col);
            return columns;
        }


        /// <summary>
        /// 加入欄位定義
        /// </summary>
        /// <typeparam name="TDataModel"></typeparam>
        /// <param name="columns"></param>
        /// <param name="columnSelector">欄位值選擇子</param>
        /// <param name="columnTextSelector">欄位顯示文字選擇子</param>
        /// <param name="isKeyField">是否鍵值欄位</param>
        /// <param name="visible">是否顯示</param>
        public static List<Column<TDataModel>> Add<TDataModel>(
			this List<Column<TDataModel>> columns,
			Expression<Func<TDataModel, object?>> columnSelector,
			Expression<Func<TDataModel, object?>> columnTextSelector,
			bool isKeyField,
			bool visible
			)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, columnTextSelector, visible, isKeyField);
			columns.Add(col);
			return columns;
		}

		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="isKeyField">是否鍵值欄位</param>
		/// <param name="visible">是否顯示</param>
		public static List<Column<TDataModel>> Add<TDataModel>(
			this List<Column<TDataModel>> columns,
			Expression<Func<TDataModel, object?>> columnSelector,
			string headerText,
			bool isKeyField,
			bool visible
			)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, headerText, isKeyField, visible);
			columns.Add(col);
			return columns;
		}

		/// <summary>
		/// 加入欄位定義
		/// </summary>
		/// <typeparam name="TDataModel"></typeparam>
		/// <param name="columns"></param>
		/// <param name="columnSelector">欄位值選擇子</param>
		/// <param name="headerText">欄標頭文字</param>
		/// <param name="isKeyField">是否鍵值欄位</param>
		/// <param name="visible">是否顯示</param>
		/// <param name="format">格式pattern</param>
		/// <param name="fieldCssClass">欄位Css class</param>
		/// <param name="rwdLevel">RWD等級</param>
		public static List<Column<TDataModel>> Add<TDataModel>(
			this List<Column<TDataModel>> columns,
			Expression<Func<TDataModel, object?>> columnSelector,
			string headerText,
			bool isKeyField,
			bool visible,
			string format,
			string fieldCssClass,
			Rwd rwdLevel
			)
		where TDataModel : class
		{
			var col = new Column<TDataModel>(columnSelector, headerText, isKeyField, visible, format, fieldCssClass, rwdLevel);
			columns.Add(col);
			return columns;
		}
	}
}

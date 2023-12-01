namespace Lighter.NET.UiComponents.Icon
{
    /// <summary>
    /// svg圖示集(需搭配lighter-svg-icon.js使用)
    /// </summary>
    public static class SvgIcons
    {
        #region SymbolIds
        /// <summary>
        /// 常用的svg icon的symbol定義的id屬性值
        /// </summary>
        public static class SymbolIds
        {
            /// <summary>
            /// 方格
            /// </summary>
            public static string Square_24x24 { get; set; } = "svg_icon_square_24x24";
            /// <summary>
            /// 方格打勾
            /// </summary>
            public static string Square_Checked_24x24 { get; set; } = "svg_icon_square_checked_24x24";
            /// <summary>
            /// 打勾(粗)
            /// </summary>
            public static string Check_Bold { get; set; } = "svg_icon_check_bold";
            /// <summary>
            /// 打叉
            /// </summary>
            public static string X { get; set; } = "svg_icon_x";
            /// <summary>
            /// 加號
            /// </summary>
            public static string Plus { get; set; } = "svg_icon_plus";
            /// <summary>
            /// 編輯(鉛筆)
            /// </summary>
            public static string Edit { get; set; } = "svg_icon_edit";
            /// <summary>
            /// 查詢(放大鏡)
            /// </summary>
            public static string Search { get; set; } = "svg_icon_search";
            /// <summary>
            /// 回頁首()
            /// </summary>
            public static string GoTop { get; set; } = "svg_icon_go_top";
            /// <summary>
            /// QRCode
            /// </summary>
            public static string QRCode { get; set; } = "svg_icon_qrcode";
			/// <summary>
			/// 人員(資訊卡)
			/// </summary>
			public static string UserCard { get; set; } = "svg_icon_user_card";
			/// <summary>
			/// 資訊(方塊i)
			/// </summary>
			public static string InfoSquare { get; set; } = "svg_icon_info_square";
            /// <summary>
            /// 提示(圓形驚嘆號)
            /// </summary>
			public static string ExclameCircle { get; set; } = "svg_icon_exclame_circle";
			/// <summary>
			/// 前往連結(方塊箭頭右上)
			/// </summary>
			public static string LinkOutUpRight { get; set; } = "svg_icon_link_out_up_right";
			/// <summary>
			/// 付款(信用卡)
			/// </summary>
			public static string CreditCard { get; set; } = "svg_icon_credit_card";
            /// <summary>
            /// 上傳(方塊折角+箭頭向上)
            /// </summary>
			public static string UploadDocument { get; set; } = "svg_icon_upload_file_arrow_up";
            /// <summary>
            /// 上傳(容器+箭頭向上)
            /// </summary>
			public static string Upload { get; set; } = "svg_icon_upload_arrow_up";
            /// <summary>
            /// 下載(容器+箭頭向下)
            /// </summary>
			public static string Download { get; set; } = "svg_icon_download_arrow_down";
            /// <summary>
            /// 項目清單(數字前綴)
            /// </summary>
			public static string OrderList { get; set; } = "svg_icon_order_list";
            /// <summary>
            /// 項目清單(圓點前綴)
            /// </summary>
			public static string UnOrderList { get; set; } = "svg_icon_unorder_list";
            /// <summary>
            /// 項目清單(階層)
            /// </summary>
			public static string NestList { get; set; } = "svg_icon_nest_list";
            /// <summary>
            /// 文字(折角文件)
            /// </summary>
			public static string TextDocument { get; set; } = "svg_icon_document_earmark";

            /// <summary>
            /// User(單人)
            /// </summary>
			public static string Person { get; set; } = "svg_icon_person";
            /// <summary>
            /// User(單人填滿)
            /// </summary>
			public static string PersonFill { get; set; } = "svg_icon_person_fill";
            /// <summary>
            /// User(雙人)
            /// </summary>
			public static string PersonGroup { get; set; } = "svg_icon_person_group";
            /// <summary>
            /// User(雙人填滿)
            /// </summary>
			public static string PersonGroupFill { get; set; } = "svg_icon_person_group_fill";

		}
        #endregion

        #region SvgIcons
        /// <summary>
        /// CheckBox圖形(可依實際值切換核取狀態)
        /// </summary>
        public static CheckBoxSvgIcon CheckBox_24x24 { get { return new CheckBoxSvgIcon(); } }
        /// <summary>
        /// 方格
        /// </summary>
        public static SvgIcon Square_24x24 { get { return new SvgIcon() { SymbolId = SymbolIds.Square_24x24 }; } }
        /// <summary>
        /// 方格打勾
        /// </summary>
        public static SvgIcon Square_Checked_24x24 { get { return new SvgIcon() { SymbolId = SymbolIds.Square_Checked_24x24 }; } }
        /// <summary>
        /// 打勾(粗)
        /// </summary>
        public static SvgIcon Check_Bold { get { return new SvgIcon() { SymbolId = SymbolIds.Check_Bold }; } }
        /// <summary>
        /// 打叉
        /// </summary>
        public static SvgIcon X { get { return new SvgIcon() { SymbolId = SymbolIds.X, Fill="#b7c8d9", SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        /// 加號
        /// </summary>
        public static SvgIcon Plus { get { return new SvgIcon() { SymbolId = SymbolIds.Plus, SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        /// 編輯(鉛筆)
        /// </summary>
        public static SvgIcon Edit { get { return new SvgIcon() { SymbolId = SymbolIds.Edit, SymbolWidth=16, SymbolHeight=16 }; } }
        /// <summary>
        /// 查詢(放大鏡)
        /// </summary>
        public static SvgIcon Search { get { return new SvgIcon() { SymbolId = SymbolIds.Search, SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        /// 回頁首()
        /// </summary>
        public static SvgIcon GoTop { get { return new SvgIcon() { SymbolId = SymbolIds.GoTop, SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        /// QRCode
        /// </summary>
        public static SvgIcon QRCode { get { return new SvgIcon() { SymbolId = SymbolIds.QRCode, SymbolWidth = 16, SymbolHeight = 16 }; } }
		/// <summary>
		/// 人員(資訊卡)
		/// </summary>
        public static SvgIcon UserCard { get { return new SvgIcon() { SymbolId = SymbolIds.UserCard, SymbolWidth = 16, SymbolHeight = 16 }; } }
		/// <summary>
		/// 資訊(方塊i)
		/// </summary>
		public static SvgIcon InfoSquare { get { return new SvgIcon() { SymbolId = SymbolIds.InfoSquare, SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        /// 提示(圓形驚嘆號)
        /// </summary>
        public static SvgIcon ExclameCircle { get { return new SvgIcon() { SymbolId = SymbolIds.ExclameCircle, SymbolWidth = 16, SymbolHeight = 16 }; } }
		/// <summary>
		/// 前往連結(方塊箭頭右上)
		/// </summary>
		public static SvgIcon LinkOutUpRight { get { return new SvgIcon() { SymbolId = SymbolIds.LinkOutUpRight, SymbolWidth = 16, SymbolHeight = 16 }; } }
		/// <summary>
		/// 付款(信用卡)
		/// </summary>
		public static SvgIcon CreditCard { get { return new SvgIcon() { SymbolId = SymbolIds.CreditCard, SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        /// 上傳(方塊折角+箭頭向上)
        /// </summary>
		public static SvgIcon UploadDocument { get { return new SvgIcon() { SymbolId = SymbolIds.UploadDocument, SymbolWidth = 16, SymbolHeight = 16 }; } }
        
        /// <summary>
        /// 上傳(容器+箭頭向上)
        /// </summary>
        public static SvgIcon Upload { get { return new SvgIcon() { SymbolId = SymbolIds.Upload, SymbolWidth = 16, SymbolHeight = 16 }; } }
        
        /// <summary>
        /// 下載(容器+箭頭向下)
        /// </summary>
        public static SvgIcon Download { get { return new SvgIcon() { SymbolId = SymbolIds.Download, SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        /// 項目清單(數字前綴)
        /// </summary>
        public static SvgIcon OrderList { get { return new SvgIcon() { SymbolId = SymbolIds.OrderList, SymbolWidth = 16, SymbolHeight = 16 }; } }

        /// <summary>
        /// 項目清單(圓點前綴)
        /// </summary>
        public static SvgIcon UnOrderList { get { return new SvgIcon() { SymbolId = SymbolIds.UnOrderList, SymbolWidth = 16, SymbolHeight = 16 }; } }

        /// <summary>
        /// 項目清單(階層)
        /// </summary>
        public static SvgIcon NestList { get { return new SvgIcon() { SymbolId = SymbolIds.NestList, SymbolWidth = 16, SymbolHeight = 16 }; } }

        /// <summary>
        /// 文字(折角文件)
        /// </summary>
        public static SvgIcon TextDocument { get { return new SvgIcon() { SymbolId = SymbolIds.TextDocument, SymbolWidth = 16, SymbolHeight = 16 }; } }

        /// <summary>
        /// User(單人)
        /// </summary>
        public static SvgIcon Person { get { return new SvgIcon() { SymbolId = SymbolIds.Person, SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        /// User(單人填滿)
        /// </summary>
        public static SvgIcon PersonFill { get { return new SvgIcon() { SymbolId = SymbolIds.PersonFill, SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        ///  User(雙人)
        /// </summary>
        public static SvgIcon PersonGroup { get { return new SvgIcon() { SymbolId = SymbolIds.PersonGroup, SymbolWidth = 16, SymbolHeight = 16 }; } }
        /// <summary>
        /// User(雙人填滿)
        /// </summary>
        public static SvgIcon PersonGroupFill { get { return new SvgIcon() { SymbolId = SymbolIds.PersonGroupFill, SymbolWidth = 16, SymbolHeight = 16 }; } }


		#endregion

	}
}

namespace Lighter.NET.Common
{
    /// <summary>
    /// 表單參數
    /// </summary>
    public class FormActionArgs
    {
        private CommandButton? _addButton;
        private CommandButton? _editButton;
        private CommandButton? _cancelButton;
        private CommandButton? _submitButton;
        private CommandButton? _deleteButton;
        private CommandButton? _closeButton;


        /// <summary>
        /// 表單標題
        /// </summary>
        public string FormTitle { get; set; } = "";
        /// <summary>
        /// 是否Multipart(有檔案欄位)
        /// </summary>
        public bool IsMultipart { get; set; } = false;
        /// <summary>
        /// 新增按鈕
        /// </summary>
        public CommandButton? AddButton
        {
            get
            {
                return _addButton;
            }
            set
            {
                if (value == null) return;
                if (string.IsNullOrEmpty(value.Text)) value.Text = "新增";
                if (string.IsNullOrEmpty(value.CommandName)) value.CommandName = "add";
                _addButton = value;
            }
        
        }

        /// <summary>
        /// 編輯按鈕
        /// </summary>
        public CommandButton? EditButton
        {
            get
            {
                return _editButton;
            }
            set
            {
                if (value == null) return;
                if (string.IsNullOrEmpty(value.Text)) value.Text = "編輯";
                if (string.IsNullOrEmpty(value.CommandName)) value.CommandName = "edit";
                _editButton = value;
            }

        }

        /// <summary>
        /// 取消按鈕
        /// </summary>
        public CommandButton? CancelButton
        {
            get
            {
                return _cancelButton;
            }
            set
            {
                if (value == null) return;
                if (string.IsNullOrEmpty(value.Text)) value.Text = "取消";
                if (string.IsNullOrEmpty(value.CommandName)) value.CommandName = "cancel";
                _cancelButton = value;
            }

        }

        /// <summary>
        /// 送出/儲存按鈕
        /// </summary>
        public CommandButton? SubmitButton
        {
            get
            {
                return _submitButton;
            }
            set
            {
                if (value == null) return;
                if (string.IsNullOrEmpty(value.Text)) value.Text = "送出";
                if (string.IsNullOrEmpty(value.CommandName)) value.CommandName = "submit";
                _submitButton = value;
            }

        }

        /// <summary>
        /// 刪除按鈕
        /// </summary>
        public CommandButton? DeleteButton
        {
            get
            {
                return _deleteButton;
            }
            set
            {
                if (value == null) return;
                if (string.IsNullOrEmpty(value.Text)) value.Text = "刪除";
                if (string.IsNullOrEmpty(value.CommandName)) value.CommandName = "delete";
                _deleteButton = value;
            }

        }

        /// <summary>
        /// 關閉按鈕
        /// </summary>
        public CommandButton? CloseButton
        {
            get
            {
                return _closeButton;
            }
            set
            {
                if (value == null) return;
                if (string.IsNullOrEmpty(value.Text)) value.Text = "關閉";
                if (string.IsNullOrEmpty(value.CommandName)) value.CommandName = "close";
                _closeButton = value;
            }

        }


        ///// <summary>
        ///// 送出/儲存按鈕要執行的ActionName
        ///// </summary>
        //public string SubmitActionName { get; set; } = "";
        ///// <summary>
        ///// 新增按鈕要執行的ActionName
        ///// </summary>
        //public string AddActionName { get; set; } = "";
        /// <summary>
        /// 載入表單的ActionName
        /// </summary>
        public string LoadFormActionName { get; set; } = "";
        /// <summary>
        /// 新增按鈕要執行的target window
        /// </summary>
        public HyperLinkTargetType AddActionTarget { get; set; } = HyperLinkTargetType.Blank;
        ///// <summary>
        ///// 新增按鈕文字
        ///// </summary>
        //public string AddButtonText
        //{
        //    get { return _addButtonText; }
        //    set
        //    {
        //        if(string.IsNullOrEmpty(value) == false)
        //        {
        //            _addButtonText = value;
        //            AddButtonVisible = true;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 編輯按鈕文字
        ///// </summary>
        //public string EditButtonText
        //{
        //    get { return _editButtonText; }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value) == false)
        //        {
        //            _editButtonText = value;
        //            EditButtonVisible = true;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 取消按鈕文字
        ///// </summary>
        //public string CancelButtonText
        //{
        //    get { return _cancelButtonText; }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value) == false)
        //        {
        //            _cancelButtonText = value;
        //            CancelButtonVisible = true;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 刪除按鈕文字
        ///// </summary>
        //public string DeleteButtonText
        //{
        //    get { return _deleteButtonText; }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value) == false)
        //        {
        //            _deleteButtonText = value;
        //            DeleteButtonVisible = true;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 送出/儲存按鈕文字
        ///// </summary>
        //public string SubmitButtonText
        //{
        //    get { return _submitButtonText; }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value) == false)
        //        {
        //            _submitButtonText = value;
        //            SubmitButtonVisible = true;
        //        }
        //    }
        //}

        /// <summary>
        /// 送出成功訊息
        /// </summary>
        public MessageModel? SuccessMessage { get; set; }
        /// <summary>
        /// 送出失敗訊息
        /// </summary>
        public MessageModel? FailedMessage { get; set; }
        ///// <summary>
        ///// 關閉按鈕可視(預設：false)
        ///// </summary>
        //public bool CloseButtonVisible { get; set; } = false;
        ///// <summary>
        ///// 取消按鈕可視(預設：false)
        ///// </summary>
        //public bool CancelButtonVisible { get; set; } = false;
        ///// <summary>
        ///// 編輯按鈕可視(預設：false)
        ///// </summary>
        //public bool EditButtonVisible { get; set; } = false;
        ///// <summary>
        ///// 刪除按鈕可視(預設：false)
        ///// </summary>
        //public bool DeleteButtonVisible { get;set; } = false;
        ///// <summary>
        ///// 新增按鈕可視(預設：false)
        ///// </summary>
        //public bool AddButtonVisible { get; set; } = false;
        ///// <summary>
        ///// 送出/儲存按鈕可視(預設:false)
        ///// </summary>
        //public bool SubmitButtonVisible { get; set; } = false;
        /// <summary>
        /// 上方命令按鈕列可視(預設：true)
        /// </summary>
        public bool TopCommandBarVisible { get; set; } = true;
        /// <summary>
        /// 下方命令按鈕列可視(預設：true)
        /// </summary>
        public bool BottomCommandBarVisible { get; set; } = true;
        /// <summary>
        /// 表單區塊(容器)寬度(%)
        /// </summary>
        public PercentWidth FormPanelWidth { get; set; } = "100%";
        /// <summary>
        /// 明細表單載入後的回呼函式(javascript function name)，
        /// 通常用以設定事件處理函式，或其他資料綁定動作
        /// </summary>
        public string AfterFormLoadCallBack { get; set; } = "";
    }
}

namespace Lighter.NET.UiComponents
{
    /// <summary>
    /// UI元素種類
    /// </summary>
    public enum UiElementType
    {
        /// <summary>
        /// 未定義
        /// </summary>
        Undefined,
        /// <summary>
        /// 對應到Html的input type="text"元素
        /// </summary>
        TextBox,
        /// <summary>
        /// 對應到Html的textarea元素
        /// </summary>
        TextArea,
        /// <summary>
        /// 對應到Html的input type="checkbox"元素
        /// </summary>
        CheckBox,
        /// <summary>
        /// 對應到Html的table元素
        /// </summary>
        Table,
        /// <summary>
        /// 對應到Html的 button元素
        /// </summary>
        Button,
        /// <summary>
        /// 對應到Html的input type="radio"元素
        /// </summary>
        RadioButton,
        /// <summary>
        /// 此類別應於client頁面生成一容器元素(例如：div), 內含多個name屬性相同，而id屬性不同的input type="radio"元素, 用以傳回單值選項參數
        /// </summary>
        RadioButtonList,
        /// <summary>
        /// 對應到Html的select元素
        /// </summary>
        Select,
        /// <summary>
        /// 對應到Html的select元素
        /// </summary>
        DropDownList,
        /// <summary>
        /// 此類別應於client頁面生成一容器元素(例如：div), 內含多個name屬性相同，而id屬性不同的input type="checkbox"元素, 用以傳回逗號分隔的多值參數
        /// </summary>
        CheckBoxList,
        /// <summary>
        /// 此類別應於client頁面生成一個table元素，其中每一列的開頭欄位中有一個input type="checkbox"元素,每列作為一個選項，用以提供多選條件設定，傳回逗號分隔的多值參數
        /// </summary>
        CheckBoxTable,
        /// <summary>
        /// 對應到Html的input type="hidden"元素
        /// </summary>
        HiddenField,
        /// <summary>
        /// 對應到Html的label元素
        /// </summary>
        Label,
        /// <summary>
        /// 對應到Html的input type="date"元素
        /// </summary>
        Date,
        /// <summary>
        /// 對應到Html的input type="datetime-local"元素
        /// </summary>
        DateTime,
        /// <summary>
        /// 對應到Html的input type="month"元素
        /// </summary>
        Month,
        /// <summary>
        /// 對應到Html的input type="time"元素
        /// </summary>
        Time,
        /// <summary>
        /// 對應到Html的input type="number"元素
        /// </summary>
        Number,
        /// <summary>
        /// 對應到Html的span元素
        /// </summary>
        Span,
        /// <summary>
        /// 對應到Html的script元素
        /// </summary>
        Javascript,
        /// <summary>
        /// unicode圖示
        /// </summary>
        UnicodeIcon,
        /// <summary>
        /// svg圖示
        /// </summary>
        SvgIcon,
        /// <summary>
        /// bootstrap圖示
        /// </summary>
        BootstrapIcon,
        /// <summary>
        /// 資料分頁頁碼列
        /// </summary>
        PaginationBar,
        /// <summary>
        /// 步驟(step-by-step)狀態列
        /// </summary>
        StepBar
    }

    /// <summary>
    /// 文字水平對齊
    /// </summary>
    public enum TextAlign
    {
        None,
        Left,
        Center,
        Right
    }

    public enum YesNoEnum
    {
        None = -1,
        No = 0,
        Yes = 1
    }

    /// <summary>
    /// YesNo選項列舉值(以class代替列舉，使不預設0)
    /// </summary>
    public static class YesNo
    {
        /// <summary>
        /// 未選定
        /// </summary>
        public const string NONE = "";
        /// <summary>
        /// 選定Yes
        /// </summary>
        public const string YES = "1";
        /// <summary>
        /// 選定No
        /// </summary>
        public const string NO = "0";
    }

    /// <summary>
    /// 表格種類
    /// </summary>
    public enum TableType
    {
        /// <summary>
        /// 未定義
        /// </summary>
        Undefined,
        /// <summary>
        /// 資料列表
        /// </summary>
        DataRowList,
        /// <summary>
        /// 排版表格
        /// </summary>
        LayoutTable
    }

    /// <summary>
    /// Html表格生成模式
    /// </summary>
    public enum TableRenderMode
    {
        /// <summary>
        /// 完整table(含table tag)
        /// </summary>
        FullTable,
        /// <summary>
        /// 只含資料列(tbody的部分)
        /// </summary>
        RowOnly,
        /// <summary>
        /// 含標頭列和資料列(thead+tbody的部分)
        /// </summary>
        HeaderAndRow
    }

    /// <summary>
    /// RWD顯示分段
    /// </summary>
    public enum Rwd
    {
        /// <summary>
        /// 不分級
        /// </summary>
        none = 0,
        /// <summary>
        /// <= 575.99px以下顯示
        /// </summary>
        xs = 1,
        /// <summary>
        /// <= 767.99px以下顯示
        /// </summary>
        s = 2,
        /// <summary>
        /// <= 991.99px以下顯示
        /// </summary>
        m = 3,
        /// <summary>
        /// <= 1199.99px以下顯示
        /// </summary>
        l = 4,
        /// <summary>
        /// <= 1439.99px以下顯示
        /// </summary>
        xl = 5,
        /// <summary>
        /// >=1440 以下顯示
        /// </summary>
        xxl = 6
    }
}

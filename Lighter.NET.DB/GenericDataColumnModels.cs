namespace Lighter.NET.DB
{
    /// <summary>
    /// A generic column model of one column of type TColumn1
    /// </summary>
    /// <typeparam name="TColumn1"></typeparam>
    public class ColumnModel<TColumn1>
    {
        /// <summary>
        /// an general use column name Column1
        /// </summary>
        public TColumn1? Column1 { get; set; }
    }

    /// <summary>
    /// A generic column model of two columns of type TColumn1 and TColumn2
    /// </summary>
    /// <typeparam name="TColumn1"></typeparam>
    /// <typeparam name="TColumn2"></typeparam>
    public class ColumnModel<TColumn1, TColumn2>
    {
        /// <summary>
        /// an general use column name Column1
        /// </summary>
        public TColumn1? Column1 { get; set; }
        /// <summary>
        /// an general use column name Column2
        /// </summary>
        public TColumn2? Column2 { get; set; }
    }

    /// <summary>
    /// A generic column model of three columns of type TColumn1, TColumn2 and TColumn3
    /// </summary>
    /// <typeparam name="TColumn1"></typeparam>
    /// <typeparam name="TColumn2"></typeparam>
    /// <typeparam name="TColumn3"></typeparam>
    public class ColumnModel<TColumn1, TColumn2, TColumn3>
    {
        /// <summary>
        /// an general use column name Column1
        /// </summary>
        public TColumn1? Column1 { get; set; }
        /// <summary>
        /// an general use column name Column2
        /// </summary>
        public TColumn2? Column2 { get; set; }
        /// <summary>
        /// an general use column name Column3
        /// </summary>
        public TColumn3? Column3 { get; set; }
    }

    /// <summary>
    /// A generic column model of four columns of type TColumn1, TColumn2, TColumn3 and TColumn4
    /// </summary>
    /// <typeparam name="TColumn1"></typeparam>
    /// <typeparam name="TColumn2"></typeparam>
    /// <typeparam name="TColumn3"></typeparam>
    /// <typeparam name="TColumn4"></typeparam>
    public class ColumnModel<TColumn1, TColumn2, TColumn3, TColumn4>
    {
        /// <summary>
        /// an general use column name Column1
        /// </summary>
        public TColumn1? Column1 { get; set; }
        /// <summary>
        /// an general use column name Column2
        /// </summary>
        public TColumn2? Column2 { get; set; }
        /// <summary>
        /// an general use column name Column3
        /// </summary>
        public TColumn3? Column3 { get; set; }
        /// <summary>
        /// an general use column name Column4
        /// </summary>
        public TColumn4? Column4 { get; set; }
    }

    /// <summary>
    /// A generic column model of five columns of type TColumn1, TColumn2, TColumn3, TColumn4 and TColumn5
    /// </summary>
    /// <typeparam name="TColumn1"></typeparam>
    /// <typeparam name="TColumn2"></typeparam>
    /// <typeparam name="TColumn3"></typeparam>
    /// <typeparam name="TColumn4"></typeparam>
    /// <typeparam name="TColumn5"></typeparam>
    public class ColumnModel<TColumn1, TColumn2, TColumn3, TColumn4, TColumn5>
    {
        /// <summary>
        /// an general use column name Column1
        /// </summary>
        public TColumn1? Column1 { get; set; }
        /// <summary>
        /// an general use column name Column2
        /// </summary>
        public TColumn2? Column2 { get; set; }
        /// <summary>
        /// an general use column name Column3
        /// </summary>
        public TColumn3? Column3 { get; set; }
        /// <summary>
        /// an general use column name Column4
        /// </summary>
        public TColumn4? Column4 { get; set; }
        /// <summary>
        /// an general use column name Column5
        /// </summary>
        public TColumn5? Column5 { get; set; }
    }
}

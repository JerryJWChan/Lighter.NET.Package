namespace Lighter.NET.Common
{
    /// <summary>
    /// Http�ШD���~�T�����ViewModel
    /// </summary>
    public class RequestExceptionViewModel
    {
        /// <summary>
        /// Exception���ɶ�
        /// </summary>
        public DateTime ErrorTime { get; set; } = DateTime.Now;
        /// <summary>
        /// �ШD�s��
        /// </summary>
        public string? RequestId { get; set; }
        /// <summary>
        /// �O�_��ܽШD�s��
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        /// <summary>
        /// ���A�X
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// �������A�X��²�u�y�z
        /// </summary>
        public string StatusDescription { get; set; } = "";
        /// <summary>
        /// ���~�T��(NOTE: ���������Үɥu�i���²�u�T���A���i���S�t�αӷP��T)
        /// </summary>
        public string ErrorMessage { get; set; } = "";
        /// <summary>
        /// ��l�s�����}
        /// </summary>
        public string OriginalUrl { get; set; } = "";
        /// <summary>
        /// �s�����}�Ѽ�
        /// </summary>
        public string QueryString { get; set; } = "";
        /// <summary>
        /// Exception��Type
        /// </summary>
        public string ExceptionErrorType { get; set; } = "";
        /// <summary>
        /// Exception��Message
        /// </summary>
        public string ExceptoinErrorMessage { get; set; } = "";
        /// <summary>
        /// Exception��StackTrace
        /// </summary>
        public string ExceptionStackTrace { get; set; } = "";

    }
}
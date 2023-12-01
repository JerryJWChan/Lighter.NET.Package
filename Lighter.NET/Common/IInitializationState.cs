namespace Lighter.NET.Common
{
    /// <summary>
    /// 初始化狀態介面
    /// </summary>
    public interface IInitializationState
    {
        bool IsInitialized { get; set; }
    }
}

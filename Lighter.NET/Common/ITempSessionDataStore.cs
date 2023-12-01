namespace Lighter.NET.Common
{
    public interface ITempSessionDataStore<T>
    {
        Dictionary<string, TempSessionData<T>> SessionDataStore { get; }
        string GetSessionID();
    }
}

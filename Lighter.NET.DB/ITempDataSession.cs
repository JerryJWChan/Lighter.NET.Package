namespace Lighter.NET.DB
{
    /// <summary>
    /// Interface for temp session data store
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface ITempSessionDataStore<TModel>
    {
        /// <summary>
        /// data store
        /// </summary>
        Dictionary<string,List<TModel>> SessionDataStore { get; }
        /// <summary>
        /// get an unique session id for distingruish session data in the data store
        /// </summary>
        /// <returns></returns>
        string GetSessionID();
    }
}

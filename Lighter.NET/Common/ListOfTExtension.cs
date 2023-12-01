
namespace Lighter.NET.Common
{
    /// <summary>
    /// List of T 延伸函式
    /// </summary>
    public static class ListOfTExtension
    {
        /// <summary>
        /// 移除項目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="match">符合條件</param>
        /// <returns></returns>
        public static bool Remove<T>(this List<T> list, Predicate<T> match) where T : class
        {
            bool removed = false;
            var found = list.Find(match);
            if (found != null)
            {
                removed = list.Remove(found);
            }
            return removed;
        }
    }
}

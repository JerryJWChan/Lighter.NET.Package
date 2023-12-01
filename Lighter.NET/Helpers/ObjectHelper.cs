using System.Collections.Specialized;
using System.Reflection;

namespace Lighter.NET.Helpers
{
    /// <summary>
    /// 物件操作輔助函式
    /// </summary>
    public class ObjectHelper
    {
        /// <summary>
        /// 賦值(將source物件的屬性值，套用到target物件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">目標物件</param>
        /// <param name="source">來源物件</param>
        /// <param name="props">屬性範圍(若無指定，則套用全部屬性)</param>
        /// <param name="ignoreDefaultValue">是否略過來源物件中的屬性是預設值的部分</param>
        public static void Assign<T>(T target,T source, PropertyInfo[]? srcProps = null, bool ignoreDefaultValue = false)
        {
            if(source == null || target== null) return;
            if(srcProps == null) srcProps = source.GetType().GetProperties();
            var trgProps = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Type pType;
            foreach( var p in srcProps ) {
                pType = p.PropertyType;
                var value = p.GetValue(source);
                if (ignoreDefaultValue)
                {
                    //若為null，略過
                    if (value == null) continue;
                    //若型別為string而值是""時，視為預設值，略過
                    if (pType == typeof(string) && value.ToString() == "") { continue; }
                    //其他型別，若同預設值，略過
                    if(value.Equals(GetTypeDefaultValue(pType))) { continue; }
                }
                trgProps.First(x=>x.Name == p.Name).SetValue(target,value);
            }
        }

        /// <summary>
        /// 克隆(完整拷背物件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">來源物件</param>
        /// <returns></returns>
        public static T? Clone<T>(T? source) where T : class,new()
        {
            if (source == null) return null;
            T newObject = new T();
            Assign(newObject,source);
            return newObject;
        }

        /// <summary>
        /// 型別預設值暫存
        /// </summary>
        private static HybridDictionary _typeDefaultValueCache = new HybridDictionary();
        /// <summary>
        /// 取得某一型別的預設值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? GetTypeDefaultValue(Type type)
        {
            //reference type
            if (!type.IsValueType)
            {
                return null;
            }

            //value type
            if (_typeDefaultValueCache.Contains(type))
            {
                return _typeDefaultValueCache[type];
            }

            object? defaultValue = Activator.CreateInstance(type);
            //暫存(提高效能)
            _typeDefaultValueCache[type] = defaultValue;
            return defaultValue;
        }
    }
}

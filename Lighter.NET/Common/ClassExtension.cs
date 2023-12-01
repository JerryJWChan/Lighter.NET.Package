using System.Reflection;

namespace Lighter.NET.Common
{
    /// <summary>
    /// 類別延伸函式
    /// </summary>
    public static class ClassExtension
    {
        /// <summary>
        /// 設定類別物件的屬性值(以lambda表示式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anyClassInstance"></param>
        /// <param name="assignExpression"></param>
        public static void Set<T>(this T anyClassInstance, Action<T> assignExpression) where T:class
        {
            if (anyClassInstance == null) return;
            assignExpression(anyClassInstance);
        }

        /// <summary>
        /// 設定類別物件的屬性值(從另一個來源物件將屬性名稱相同，且資料型別相同的屬性值套用到現有物件)
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="target">目的物件</param>
        /// <param name="source">來源物件</param>
        /// <param name="typeMode">資料型別決定模式(StrictType:嚴格模式，LooseType:鬆散模式)</param>
        public static void AssignValueFrom<TTarget,TSource>(this TTarget target, TSource source,TypeAssignMode typeMode = TypeAssignMode.StrictType) 
            where TTarget : class 
            where TSource : class
        {
            if(target == null || source == null) return;
            var propsT = typeof(TTarget).GetProperties();
            var propsS = typeof(TSource).GetProperties();
            PropertyInfo? found;
            foreach( var p in propsT)
            {
               if(typeMode == TypeAssignMode.StrictType)
                {
                    found = propsS.FirstOrDefault(x => x.Name == p.Name && x.PropertyType.Equals(p.PropertyType));
                }
                else
                {
                    found = propsS.FirstOrDefault(x => x.Name == p.Name && x.PropertyType.IsAssignableTo(p.PropertyType));
                }
                
                if (found != null)
                {
                    p.SetValue(target, found.GetValue(source));
                }
            }
        }
    }
}

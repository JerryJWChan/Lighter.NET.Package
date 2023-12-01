using System.Linq.Expressions;
using System.Reflection;

namespace Lighter.NET.Common
{
    public static class ExpressionExtension
    {
        #region Lambda Expression
        /// <summary>
        /// 取得Lambda表示式指定的屬性物件
        /// </summary>
        /// <typeparam name="TType">類別參數項</typeparam>
        /// <typeparam name="TReturn">(單一)屬性表示式</typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static PropertyInfo? GetLambdaProperty<TType, TReturn>(this Expression<Func<TType, TReturn>> selector)
        {
            if(selector==null) throw new ArgumentNullException(nameof(selector));
            if(!(selector is LambdaExpression)) throw new ArgumentException($"{nameof(selector)} must be a LambdaExpression.");
            LambdaExpression lambda = selector;
            var memberExpression = lambda.Body is UnaryExpression expression
                ? expression.Operand as MemberExpression
                : lambda.Body as MemberExpression;

            return memberExpression?.Member as PropertyInfo;
        }

        /// <summary>
        /// 取得Lambda表示式指定的屬性名稱
        /// </summary>
        /// <typeparam name="TType">類別參數項</typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="selector">(單一)屬性表示式</param>
        /// <returns></returns>
        public static string GetLambdaPropertyName<TType, TReturn>(this Expression<Func<TType, TReturn>> selector)
        {
            return selector.GetLambdaProperty()?.Name??"";
        }

        #endregion

        #region Anonymous Expression
        /// <summary>
        /// 取得Anonymous表示式指定的(TType所含的)屬性物件
        /// </summary>
        /// <typeparam name="TType">類別參數項</typeparam>
        /// <param name="selector">匿名型別表示式的屬性(欄位)選擇器</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static PropertyInfo[]? GetAnonymousProperties<TType>(this Expression<Func<TType, object>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if(selector.Body == null) throw new ArgumentException($"{nameof(selector)} must has a expression body of NewExpression.");
            if (!(selector.Body is NewExpression)) throw new ArgumentException($"{nameof(selector)} must has a expression body of NewExpression.");

            var body = selector.Body as NewExpression;
            if (body == null) return null;
            return body.Type.GetProperties();
        }

        /// <summary>
        /// 取得Anonymous表示式指定的屬性名稱
        /// </summary>
        /// <typeparam name="TType">類別參數項</typeparam>
        /// <param name="selector">匿名型別表示式的屬性(欄位)選擇器</param>
        /// <returns></returns>
        public static string[] GetAnonymousPropertyNames<TType>(this Expression<Func<TType, object>> selector)
        {
            var props = GetAnonymousProperties(selector);
            if (props == null) return new string[] { };
            return props.Select(p => p.Name).ToArray();
        }

        /// <summary>
        /// 取得Anonymous表示式本身的屬性物件
        /// </summary>
        /// <typeparam name="TType">類別參數項</typeparam>
        /// <param name="selector">匿名型別表示式的屬性(欄位)選擇器</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static PropertyInfo[]? GetSelectedProperties<TType>(this Expression<Func<TType, object>> selector)
        {
            var propNames = GetSelectedPropertyNames(selector);
            var targetType = typeof(TType);
            var selectedProps = targetType.GetProperties().Where(p => propNames.Contains(p.Name))?.ToArray() ?? null;
            return selectedProps;
        }

        /// <summary>
        /// 取得Anonymous表示式指定的屬性名稱
        /// </summary>
        /// <typeparam name="TType">類別參數項</typeparam>
        /// <param name="selector">匿名型別表示式的屬性(欄位)選擇器</param>
        /// <returns></returns>
        public static string[] GetSelectedPropertyNames<TType>(this Expression<Func<TType, object>> selector)
        {
            var props = GetAnonymousProperties(selector);
            if (props == null) return new string[] { };
            return props.Select(p => p.Name).ToArray();
        }

        #endregion
    }
}

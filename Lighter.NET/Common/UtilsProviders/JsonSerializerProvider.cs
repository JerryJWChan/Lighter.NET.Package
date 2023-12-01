
namespace Lighter.NET.Common.UtilsProviders
{

    /// <summary>
    /// Json序列化提供者
    /// </summary>
    public static class JsonSerializerProvider
    {
        /// <summary>
        /// 序列化成json格式
        /// </summary>
        public static Func<object?, string> Serialize { get; set; } = Common.CustomJsonSerializer.Serialize;
    }
}

namespace Lighter.NET.Common
{
    /// <summary>
    /// [NonApiResult]用以標式特定Action的回傳型別非ApiResult型別的JsonReuslt
    /// 其作用是bypass ActionModelValidateFilter的介入檢核動作
    /// </summary>
    public class NonApiResultAttribute:Attribute
    {
    }
}

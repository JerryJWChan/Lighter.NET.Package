//using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lighter.NET.DB
{
    /// <summary>
    /// Extension methods for ICreator
    /// </summary>
    public static class ICreatorExtension
    {
        /// <summary>
        /// 設定建立者
        /// </summary>
        /// <param name="model"></param>
        public static void SetCreator(this ICreator? model)
        {
            if (model == null) return;
            model.createAt = DateTime.Now;
            model.createBy = "";
            model.createIp = "";
            var getter = DbServiceBase.DbServiceConfig.DbAccessUserInfoGetter;
            if(getter != null)
            {
                var userInfo = getter() ?? null;
                if(userInfo != null)
                {
                    model.createBy = $"{userInfo.UserId}_{userInfo.UserName}";
                    model.createIp = userInfo.IpAddress??"";
                }
            }
            
        }
    }
}

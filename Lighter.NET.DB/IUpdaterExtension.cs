namespace Lighter.NET.DB
{
    /// <summary>
    /// Extension methods for IUpdater
    /// </summary>
    public static class IUpdaterExtension
    {
        /// <summary>
        /// 設定更新者
        /// </summary>
        /// <param name="model"></param>
        public static void SetUpdater(this IUpdater? model)
        {
            if (model == null) return;
            model.updateAt = DateTime.Now;
            model.updateBy = "";
            model.updateIp = "";
            var getter = DbServiceBase.DbServiceConfig.DbAccessUserInfoGetter;
            if (getter != null)
            {
                var userInfo = getter() ?? null;
                if (userInfo != null)
                {
                    model.updateBy = $"{userInfo.UserId}_{userInfo.UserName}";
                    model.updateIp = userInfo.IpAddress??"";
                }
            }

        }
    }
}

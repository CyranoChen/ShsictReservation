using System;
using System.Reflection;
using System.Threading;
using Shsict.Core.Logger;
using Shsict.Core.Scheduler;
using Shsict.Core.Utility;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.SecureNode;
using Shsict.Reservation.Mvc.Models.SecureNode;

namespace Shsict.Reservation.Mvc.Scheduler
{
    internal class RefreshCache : ISchedule
    {
        private readonly ILog _log = new AppLog();

        public void Execute(object state)
        {
            var logInfo = new LogInfo
            {
                MethodInstance = MethodBase.GetCurrentMethod(),
                ThreadInstance = Thread.CurrentThread
            };

            try
            {
                _log.Info("Scheduler Start: (RefreshCache)", logInfo);

                Config.UpdateAssemblyInfo(Assembly.GetExecutingAssembly(), ConfigSystem.Reservation);

                ConfigGlobal.Refresh();
                ConfigGlobalSecureNode.Refresh();

                Delivery.Cache.RefreshCache();
                Menu.Cache.RefreshCache();

                // 删除30天前的无效订单记录
                Order.Clean(-30);

                OperationStandard.Cache.RefreshCache();
                OperationStandardDto.Cache.RefreshCache();

                // 删除30天前的无效安全检查记录
                CheckList.Clean(-30);

                _log.Info("Scheduler End: (RefreshCache)", logInfo);
            }
            catch (Exception ex)
            {
                _log.Warn(ex, logInfo);
            }
        }
    }
}
using System;
using System.Reflection;
using System.Threading;
using Shsict.Core;
using Shsict.Core.Logger;
using Shsict.Core.Scheduler;
using Shsict.Reservation.Mvc.Entities;

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

                Delivery.Cache.RefreshCache();
                Menu.Cache.RefreshCache();

                // 删除30天前的无效订单记录
                Order.Clean(-30);

                _log.Info("Scheduler End: (RefreshCache)", logInfo);
            }
            catch (Exception ex)
            {
                _log.Warn(ex, logInfo);
            }
        }
    }
}
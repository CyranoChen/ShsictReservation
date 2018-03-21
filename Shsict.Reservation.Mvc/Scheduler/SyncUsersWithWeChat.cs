using System;
using System.Reflection;
using System.Threading;
using Shsict.Core;
using Shsict.Core.Dapper;
using Shsict.Core.Logger;
using Shsict.Core.Scheduler;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Mvc.Scheduler
{
    internal class SyncUsersWithWeChat : ISchedule
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
                _log.Info("Scheduler Start: (SyncUsersWithWeChat)", logInfo);

                IRepository repo = new Repository();

                var list = repo.All<UserWeChat>();

                var auth = new AuthorizeManager();

                if (list != null && list.Count > 0)
                {
                    foreach (var uw in list)
                    {
                        auth.SyncUserWithWeChat(uw.UserName);
                    }
                }

                _log.Info("Scheduler End: (SyncUsersWithWeChat)", logInfo);
            }
            catch (Exception ex)
            {
                _log.Warn(ex, logInfo);
            }
        }
    }
}
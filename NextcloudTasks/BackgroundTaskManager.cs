using Microsoft.Toolkit.Uwp;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace NextcloudTasks
{
    public class BackgroundTaskManager
    {
        private const string updateTask = "AppUpdateTask";
        private const string syncTask = "SynchronizeTask";

        public static async Task<bool> RegisterTasks()
        {
            // Check for background access (optional)
            var bgAccessState = await BackgroundExecutionManager.RequestAccessAsync();
            if (bgAccessState == BackgroundAccessStatus.DeniedBySystemPolicy
                || bgAccessState == BackgroundAccessStatus.DeniedByUser)
            {
                // Info to user
                return false;
            }
            if (!BackgroundTaskHelper.IsBackgroundTaskRegistered(updateTask))
            {

                // Register Task for Updates
                BackgroundTaskHelper.Register(updateTask, "NextcloudTasks.Tasks.AppUpdateTask",
                    new SystemTrigger(SystemTriggerType.ServicingComplete, false));
            }
            if (!BackgroundTaskHelper.IsBackgroundTaskRegistered(syncTask))
            {
                // Register SyncTask, TODO Make time configurable
                BackgroundTaskHelper.Register(syncTask, "NextcloudTasks.Tasks.SynchronizationTask", new TimeTrigger(15, false), false, true,
                    new SystemCondition(SystemConditionType.InternetAvailable));
            }
            return true;
        }

        public static void UnregisterTasks()
        {
            //Unregister
            BackgroundTaskHelper.Unregister(updateTask);
            BackgroundTaskHelper.Unregister(syncTask);
        }
    }
}

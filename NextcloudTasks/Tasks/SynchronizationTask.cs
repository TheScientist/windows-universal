using NextcloudApp.Models;
using NextcloudApp.Services;
using NextcloudApp.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage.AccessCache;

namespace NextcloudTasks.Tasks
{
    public sealed class SynchronizationTask
    {
        volatile bool _cancelRequested = false;

        //
        // The Run method is the entry point of a background task.
        //
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            Debug.WriteLine("SyncTask " + taskInstance.Task.Name + " starting...");
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            await new SyncServiceHelper().SyncAllFolders();

            deferral.Complete();
        }

        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
            Debug.WriteLine("SyncTask " + sender.Task.Name + " Cancel Requested...");
        }
    }
}

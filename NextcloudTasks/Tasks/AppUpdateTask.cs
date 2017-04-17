using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace NextcloudTasks.Tasks
{
    //
    // A background task always implements the IBackgroundTask interface.
    //
    public sealed class AppUpdateTask : IBackgroundTask
    {
        volatile bool _cancelRequested = false;

        //
        // The Run method is the entry point of a background task.
        //
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            Debug.WriteLine("ServicingComplete " + taskInstance.Task.Name + " starting...");
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            BackgroundTaskManager.UnregisterTasks();
            BackgroundExecutionManager.RemoveAccess();
            await BackgroundTaskManager.RegisterTasks();
            deferral.Complete();
        }

        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
            Debug.WriteLine("ServicingComplete " + sender.Task.Name + " Cancel Requested...");
        }
    }

}

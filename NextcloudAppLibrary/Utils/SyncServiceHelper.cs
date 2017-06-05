using NextcloudApp.Models;
using NextcloudApp.Services;
using NextcloudApp.Utils;
using NextcloudClient.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.AccessCache;

namespace NextcloudApp.Utils
{
    public sealed class SyncServiceHelper
    {
        public IAsyncAction SyncAllFolders()
        {
            return SyncAllFoldersInternal().AsAsyncAction();
        }

        private async Task SyncAllFoldersInternal()
        {
            List<Task> allSyncTasks = new List<Task>();
            List<FolderSyncInfo> infos = SyncDbUtils.GetAllFolderSyncInfos();
            foreach (var fsi in infos)
            {
                var resInfo = new ResourceInfo()
                {
                    Path = fsi.Path,
                    ContentType = "dav/directory"
                };
                var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(fsi.AccessListKey);
                SyncService service = new SyncService(folder, resInfo, fsi);
                allSyncTasks.Add(service.StartSync(true));
            }
            Task.WaitAll(allSyncTasks.ToArray());
        }
    }
}

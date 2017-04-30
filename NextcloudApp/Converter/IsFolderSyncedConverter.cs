﻿using NextcloudApp.Utils;
using NextcloudClient.Types;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NextcloudApp.Converter
{
    /// <summary>
    /// Checks if the folder is Synchronized or not
    /// </summary>
    public class IsFolderSyncedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var invert = parameter != null;
            var item = (ResourceInfo)value;

            if (item.ContentType== null || !item.ContentType.Equals("dav/directory"))
            {
                return Visibility.Collapsed;
            }

            if (SyncDbUtils.IsSynced(item))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new ResourceInfo();
        }
    }
}

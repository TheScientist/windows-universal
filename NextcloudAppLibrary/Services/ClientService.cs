using NextcloudApp.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NextcloudAppLib.Services
{
    public static class ClientService
    {
        private static NextcloudClient.NextcloudClient _client;

        public static async Task<NextcloudClient.NextcloudClient> GetClient()
        {
            if (_client != null)
            {
                return _client;
            }

            if (!string.IsNullOrEmpty(SettingsService.Instance.LocalSettings.ServerAddress) &&
                !string.IsNullOrEmpty(SettingsService.Instance.LocalSettings.Username))
            {
                var vault = new PasswordVault();

                IReadOnlyList<PasswordCredential> credentialList = null;
                try
                {
                    credentialList = vault.FindAllByResource(SettingsService.Instance.LocalSettings.ServerAddress);
                }
                catch
                {
                    // ignored
                }

                var credential = credentialList?.FirstOrDefault(item => item.UserName.Equals(SettingsService.Instance.LocalSettings.Username));

                if (credential == null)
                {
                    return null;
                }
                
                credential.RetrievePassword();

                try
                {
                    var response = await NextcloudClient.NextcloudClient.GetServerStatus(credential.Resource, SettingsService.Instance.LocalSettings.IgnoreServerCertificateErrors);
                    if (response == null)
                    {
                        ShowServerAddressNotFoundMessage(credential.Resource);
                        return null;
                    }
                }
                catch
                {
                    ShowServerAddressNotFoundMessage(credential.Resource);
                    return null;
                }

                _client = new NextcloudClient.NextcloudClient(
                    credential.Resource,
                    credential.UserName,
                    credential.Password
                ) {
                    IgnoreServerCertificateErrors =
                        SettingsService.Instance.LocalSettings.IgnoreServerCertificateErrors
                };
            }

            SettingsService.Instance.LocalSettings.PropertyChanged += async (sender, args) =>
            {
                if (_client != null && args.PropertyName == "IgnoreServerCertificateErrors")
                {
                    _client.IgnoreServerCertificateErrors =
                        SettingsService.Instance.LocalSettings.IgnoreServerCertificateErrors;
                }

                if (
                    string.IsNullOrEmpty(SettingsService.Instance.LocalSettings.ServerAddress) ||
                    string.IsNullOrEmpty(SettingsService.Instance.LocalSettings.Username)
                    )
                {
                    _client = null;
                    return;
                }

                var vault = new PasswordVault();

                IReadOnlyList<PasswordCredential> credentialList = null;
                try
                {
                    credentialList = vault.FindAllByResource(SettingsService.Instance.LocalSettings.ServerAddress);
                }
                catch
                {
                    // ignored
                }

                var credential = credentialList?.FirstOrDefault(item => item.UserName.Equals(SettingsService.Instance.LocalSettings.Username));

                if (credential == null)
                {
                    _client = null;
                    return;
                }

                credential.RetrievePassword();

                try
                {
                    var response = await NextcloudClient.NextcloudClient.GetServerStatus(credential.Resource, SettingsService.Instance.LocalSettings.IgnoreServerCertificateErrors);
                    if (response == null)
                    {
                        _client = null;
                        ShowServerAddressNotFoundMessage(credential.Resource);
                        return;
                    }
                }
                catch
                {
                    _client = null;
                    ShowServerAddressNotFoundMessage(credential.Resource);
                    return;
                }

                _client = new NextcloudClient.NextcloudClient(
                    credential.Resource,
                    credential.UserName,
                    credential.Password
                ) {
                    IgnoreServerCertificateErrors =
                            SettingsService.Instance.LocalSettings.IgnoreServerCertificateErrors
                };
            };

            return _client;
        }

        private static void ShowServerAddressNotFoundMessage(string serverAddress)
        {
            // TODO how to handle this here?
            Debug.WriteLine("Error creating NextcloudClient");
        }

        public static void Reset()
        {
            _client = null;
        }
    }
}

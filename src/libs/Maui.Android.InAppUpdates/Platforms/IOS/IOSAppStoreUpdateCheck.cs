using InAppUpdates.iOSAndroid.Maui.Common;
using System.Text.Json;

namespace InAppUpdates.iOSAndroid.Maui
{
    /// <summary>
    /// Provides functionality to check for app updates on iOS
    /// </summary>
    internal class IOSAppStoreUpdateCheck
    {

        // App Store constants
        //private const string APP_STORE_ID = "6741144561";
        private const string ITUNES_LOOKUP_URL = "https://itunes.apple.com/lookup";
        private const string APP_STORE_URL_FORMAT = "itms-apps://itunes.apple.com/app/id{0}";
        private const string COUNTRY_PARAMETER = "country";
        private const string BUNDLE_ID_PARAMETER = "bundleId";
        //private const string COUNTRY_VALUE = "au";

        // JSON property names
        private const string RESULTS_PROPERTY = "results";
        private const string VERSION_PROPERTY = "version";

        // Log messages
        private const string ERROR_MESSAGE_FORMAT = "Error checking for iOS update: {0}";

        /// <summary>
        /// Checks for updates to the application by comparing the current version with the latest version available in
        /// the App Store.
        /// </summary>
        /// <remarks>This method performs the following operations: <list type="bullet"> <item>Determines
        /// whether an update check should be skipped based on cached preferences.</item> <item>Retrieves the latest
        /// version of the application from the App Store.</item> <item>Prompts the user with an update alert if a newer
        /// version is available.</item> <item>Opens the App Store page for the application if the user chooses to
        /// update.</item> </list> If the <paramref name="mainPage"/> parameter is null, the method will still check for
        /// updates but will not display an alert.</remarks>
        /// <param name="appInfo">Information about the current application, including its version and package name.</param>
        /// <param name="mainPage">The main page of the application, used to display update alerts to the user. Can be null.</param>
        /// <param name="appUpdatePreferenceCheck">An object that determines whether the update check should be skipped based on cached preferences.</param>
        /// <param name="httpClient">The HTTP client used to retrieve the latest version information from the App Store.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. Defaults to <see langword="default"/>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CheckForUpdatesAsync(
                            IAppInfo appInfo,
                            Page mainPage,
                            AppUpdatePreferenceCheck appUpdatePreferenceCheck,
                            HttpClient httpClient,
                            InAppUpdatesOptions options,
                            CancellationToken cancellationToken = default)
        {

            try
            {
                // We still need MainPage for displaying alerts later, but we don't exit early
                if (mainPage == null)
                {
                    options.DebugAction("[AppStoreUpdate | CheckForUpdatesAsync] Main page is null, will check for updates but cannot show alert");
                }

                // Check if we should skip this update check based on cache
                if (await appUpdatePreferenceCheck.ShouldSkipUpdateCheck(options))
                {
                    return;
                }

                string currentVersion = appInfo.VersionString;
                string? latestVersion = null;
                string storeUrl = string.Format(APP_STORE_URL_FORMAT,
                                                        options.AppBundleID);

                latestVersion = await GetLatestiOSVersion(options.AppPackageName, httpClient, 
                                                            options, cancellationToken);

                // Update the cache with this check
                await appUpdatePreferenceCheck.UpdateCache(latestVersion, options);

                // Only proceed with update alert if we successfully got a version and it's newer
                if (latestVersion != null && new Version(latestVersion) > new Version(currentVersion) && mainPage != null)
                {
                    options.DebugAction($"[AppStoreUpdate | CheckForUpdatesAsync] New version available: {latestVersion}, current version: {currentVersion}");

                    bool shouldUpdate = false;
                    // Switch to the main thread for UI operations
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        shouldUpdate = await mainPage.DisplayAlert(
                            options.AppUpdateAlertTitle,
                            string.Format(options.AppUpdateAlertMessage, latestVersion),
                            options.AppUpdateAlertButtonYesTxt,
                            options.AppUpdateAlertButtonNoTxt);
                    });

                    if (shouldUpdate)
                    {
                        await Launcher.OpenAsync(storeUrl);
                    }

                    options.DebugAction($"[AppStoreUpdate | CheckForUpdatesAsync] User choose shouldUpdate: {shouldUpdate} for version {latestVersion}");
                }
                else if (latestVersion != null)
                {
                    options.DebugAction($"[AppStoreUpdate | CheckForUpdatesAsync] No new version available. Current: {currentVersion}, Latest: {latestVersion}");
                }
            }
            catch (Exception ex)
            {
                options.DebugAction($"[AppStoreUpdate | CheckForUpdatesAsync] Exception during update check: {ex.Message}");
            }

        }

        /// <summary>
        /// Retrieves the latest version of an iOS application from the App Store.
        /// </summary>
        /// <remarks>This method queries the iTunes App Store API using the provided bundle identifier and
        /// country code. If the application is found, the latest version is returned. If no results are found or an
        /// error occurs, <see langword="null"/> is returned.</remarks>
        /// <param name="bundleId">The unique bundle identifier of the iOS application. This parameter cannot be null or empty.</param>
        /// <param name="httpClient">An instance of <see cref="HttpClient"/> used to perform the HTTP request.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.</param>
        /// <returns>The latest version of the iOS application as a string, or <see langword="null"/> if the version cannot be
        /// determined.</returns>
        internal async Task<string?> GetLatestiOSVersion(string bundleId,
                                                HttpClient httpClient,
                                                InAppUpdatesOptions options,
                                                CancellationToken cancellationToken = default)
        {
            try
            {
                string lookupUrl = $"{ITUNES_LOOKUP_URL}?{BUNDLE_ID_PARAMETER}={bundleId}&{COUNTRY_PARAMETER}={options.AppCountryCode}";
                options.DebugAction($"[AppStoreUpdate | GetLatestiOSVersion] Checking for updates at: {lookupUrl}");

                string response = await httpClient.GetStringAsync(lookupUrl, cancellationToken);

                using JsonDocument jsonDoc = JsonDocument.Parse(response);
                JsonElement results = jsonDoc.RootElement.GetProperty(RESULTS_PROPERTY);

                if (results.GetArrayLength() > 0)
                {
                    string? version = results[0].GetProperty(VERSION_PROPERTY).GetString();
                    options.DebugAction($"[AppStoreUpdate | GetLatestiOSVersion] Found version: {version}");
                    return version;
                }
            }
            catch (TaskCanceledException ex)
            {
                options.DebugAction($"[AppStoreUpdate | GetLatestiOSVersion] TaskCanceledException: {string.Format(ERROR_MESSAGE_FORMAT, ex.Message)}");
            }
            catch (Exception ex)
            {
                // Just log general errors, no user-facing message
                options.DebugAction($"[AppStoreUpdate | GetLatestiOSVersion] Exception: {string.Format(ERROR_MESSAGE_FORMAT, ex.Message)}");
            }
            return null;
        }

    }
}

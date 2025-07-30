namespace InAppUpdates.iOSAndroid.Maui.Common
{
    internal class AppUpdatePreferenceCheck
    {
        // Cache constants
        private const string CACHE_LOG_MESSAGE = "Using cached update check data - last check was on {0}";

        /// <summary>
        /// Determines if the update check should be skipped based on the cache
        /// </summary>
        /// <returns>True if the check should be skipped, false otherwise</returns>
        internal async Task<bool> ShouldSkipUpdateCheck(InAppUpdatesOptions options)
        {
            AppStoreUpdateCheckCache? cache = await GetCache(options);

            // If no cache exists, create a new one and don't skip
            if (cache == null)
            {
                options.DebugAction("No cache exists, will create default data");
                cache = new AppStoreUpdateCheckCache();
                // Set default values for AppStoreUpdateCheckCache
                DateTime now = DateTime.Now;
                cache.LastCheckDate = now.AddDays(-10); // Set to past date to trigger update check
                cache.CurrentSessionDate = now.Date;
                cache.CheckedInCurrentSession = false;
                cache.LastCheckedVersion = "0.0.0";

                await SaveCache(cache, options);
            }

            // If AppUpdateCheckIntervalDays is 0 or less, always check for updates
            if (options.AppUpdateCheckIntervalDays <= 0)
            {
                options.DebugAction("Always performing update check (AppUpdateCheckIntervalDays <= 0)");
                return false;
            }

            // Get the current date (using Date to ignore time)
            DateTime today = DateTime.Now.Date;

            // Check if we already performed a check in this session
            if (cache.CurrentSessionDate.Date == today && cache.CheckedInCurrentSession)
            {
                options.DebugAction("Skipping update check - already checked today in this session");
                return true;
            }
            // If the last check was today, skip the check
            // Check if it's been less than the interval days since last check
            if ((today - cache.LastCheckDate.Date).TotalDays < options.AppUpdateCheckIntervalDays)
            {
                options.DebugAction($"{string.Format(CACHE_LOG_MESSAGE, cache.LastCheckDate.ToString("yyyy-MM-dd"))}");
                return true;
            }

            options.DebugAction(" Will perform update check due to elapsed time");
            return false;
        }

        /// <summary>
        /// Updates the cache with information from the current check
        /// </summary>
        /// <param name="latestVersion">The latest version that was checked</param>
        internal async Task UpdateCache(string? latestVersion, InAppUpdatesOptions options)
        {
            AppStoreUpdateCheckCache cache = await GetCache(options) ?? new AppStoreUpdateCheckCache();

            // Update cache information
            cache.LastCheckDate = DateTime.Now;
            cache.LastCheckedVersion = latestVersion;
            cache.CurrentSessionDate = DateTime.Now.Date;
            cache.CheckedInCurrentSession = true;

            await SaveCache(cache, options);
            options.DebugAction($"Updated cache with latest version: {latestVersion}");
        }

        /// <summary>
        /// Gets the update check cache from storage
        /// </summary>
        /// <returns>The cache object, or null if no cache exists</returns>
        private async Task<AppStoreUpdateCheckCache?> GetCache(InAppUpdatesOptions options)
        {
            try
            {
                var cache = await AppFileWriter<AppStoreUpdateCheckCache>
                                            .GetAsync(options);
                options.DebugAction("Update check cache read successfully");

                return cache;
            }
            catch (Exception ex)
            {
                options.DebugAction($"Error reading update check cache: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Saves the update check cache to storage
        /// </summary>
        /// <param name="cache">The cache to save</param>
        private async Task SaveCache(AppStoreUpdateCheckCache cache, InAppUpdatesOptions options)
        {
            try
            {
                await AppFileWriter<AppStoreUpdateCheckCache>
                         .SaveAsync(cache, options);

                options.DebugAction("Update check cache saved successfully");
            }
            catch (Exception ex)
            {
                options.DebugAction($"Error saving update check cache: {ex.Message}");
            }
        }
    }
}

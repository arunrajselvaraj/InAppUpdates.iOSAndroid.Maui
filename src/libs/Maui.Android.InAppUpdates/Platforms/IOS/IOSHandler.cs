// ReSharper disable once CheckNamespace
using InAppUpdates.iOSAndroid.Maui.Common;
using UIKit;

namespace InAppUpdates.iOSAndroid.Maui.Internal;

/// <summary>
/// This class is responsible for handling the update events.
/// </summary>
public static class IOSHandler
{
    /// <summary>
    /// Indicates whether a specific action has been performed once during the current session.
    /// </summary>
    private static bool doOnceForSession = false;

    /// <summary>
    /// Options for the in-app updates.
    /// </summary>
    public static InAppUpdatesOptions Options { get; set; } = new();

    /// <summary>
    /// Handles the application's resume event, typically triggered when the application returns to the foreground.
    /// </summary>
    /// <remarks>This method is intended to manage tasks that need to be performed when the application
    /// resumes, such as refreshing data or restoring state. Ensure that any necessary resources are available and ready
    /// for use when this method is called.</remarks>
    /// <param name="application">The <see cref="UIApplication"/> instance representing the application being resumed.</param>
    public static void HandleResume(UIApplication application)
    {
        if (doOnceForSession)
        {
            // Already checked for updates in this session
            return;
        }

        doOnceForSession = true;

        // Check for updates in the background
        Task.Run(async () =>
        {
            await CheckForUpdates();
        });
    }
    
    /// <summary>
    /// Handles the activation of the application after it has been launched.
    /// </summary>
    /// <remarks>This method is typically called when the application transitions to the active state. Use
    /// this method to perform tasks that should occur when the application becomes active, such as refreshing the user
    /// interface or resuming paused operations.</remarks>
    /// <param name="application">The <see cref="UIApplication"/> instance representing the application being activated.</param>
    public static void HandleActivated(UIApplication application)
    {
        if (doOnceForSession)
        {
            // Already checked for updates in this session
            return;
        }
        
        doOnceForSession = true;

        // Check for updates in the background
        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(Options.AppUpdateDelayAfterSplashInSeconds)); 
            await CheckForUpdates();
        });
    }


    private static async Task CheckForUpdates()
    {

        IOSAppStoreUpdateCheck iOSAppStoreUpdateCheck = new IOSAppStoreUpdateCheck();
        AppUpdatePreferenceCheck appUpdatePreferenceCheck = new AppUpdatePreferenceCheck();

            try
            {
                await iOSAppStoreUpdateCheck.CheckForUpdatesAsync(
                                                AppInfo.Current,
                                                Application.Current?.MainPage,
                                                appUpdatePreferenceCheck,
                                                new HttpClient 
                                                        { 
                                                            Timeout = TimeSpan.FromSeconds(60) 
                                                        },
                                                Options
                                                );
            }
            catch (Exception ex)
            {
                Options.DebugAction($"Error checking for iOS updates: {ex.Message}");
            }

    }
}
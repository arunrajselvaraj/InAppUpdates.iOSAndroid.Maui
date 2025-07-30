using Android.App;
using Android.Content;
using Android.Runtime;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model;
using Xamarin.Google.Android.Play.Core.AppUpdate.Testing;
using Bundle = Android.OS.Bundle;
using InAppUpdates.iOSAndroid.Maui.Common;

// ReSharper disable once CheckNamespace
namespace InAppUpdates.iOSAndroid.Maui.Internal;

/// <summary>
/// This class is responsible for handling the update events.
/// </summary>
public static class AndroidHandler
{
    /// <summary>
    /// The app update manager.
    /// </summary>
    public static IAppUpdateManager? AppUpdateManager { get; private set; }
    
    public static AndroidAppUpdateSuccessListener? AppUpdateSuccessListener { get; private set; }
    public static AndroidResumeSuccessListener? ResumeSuccessListener { get; private set; }
    
    /// <summary>
    /// Options for the in-app updates.
    /// </summary>
    public static InAppUpdatesOptions Options { get; set; } = new();

    /// <summary>
    /// This method will be triggered when the app is created.
    /// </summary>
    /// <param name="activity"></param>
    /// <param name="savedInstanceState"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void HandleCreate(Activity activity, Bundle? savedInstanceState)
    {
        activity = activity ?? throw new ArgumentNullException(nameof(activity));

        AppUpdatePreferenceCheck appUpdatePreferenceCheck = new AppUpdatePreferenceCheck();

        // Check for updates in the background
        Task.Run(async () =>
        {
            Options.DebugAction("started checing  for updates in the background");

            // Check if we should skip this update check based on cache
            if (await appUpdatePreferenceCheck.ShouldSkipUpdateCheck(Options))
            {
                Options.DebugAction("Skipping update check - using cached data");
                return;
            }

            Options.DebugAction("Checking for updates in the background");
            AppUpdateManager = Options.UseFakeAppUpdateManager
                ? new FakeAppUpdateManager(activity)
                : AppUpdateManagerFactory.Create(activity);
            AppUpdateSuccessListener ??= new AndroidAppUpdateSuccessListener(
                appUpdateManager: AppUpdateManager,
                activity: activity,
                updateRequest: Options.RequestCode);
            AppUpdateManager.GetAppUpdateInfo().AddOnSuccessListener(AppUpdateSuccessListener);
        });
        
    }
    
    /// <summary>
    /// This method will be triggered when the app is resumed.
    /// </summary>
    /// <param name="activity"></param>
    public static void HandleResume(Activity activity)
    {
        if (AppUpdateManager is null)
        {
            return;
        }

        AppUpdatePreferenceCheck appUpdatePreferenceCheck = new AppUpdatePreferenceCheck();
        // Check for updates in the background
        Task.Run(async () =>
        {
            Options.DebugAction("started checking  for updates in the background");

            // Check if we should skip this update check based on cache
            if (await appUpdatePreferenceCheck.ShouldSkipUpdateCheck(Options))
            {
                Options.DebugAction("Skipping update check - using cached data");
                return;
            }

            Options.DebugAction("Checking for updates in the background");

            ResumeSuccessListener ??= new AndroidResumeSuccessListener(
            appUpdateManager: AppUpdateManager,
            activity: activity,
            updateRequest: Options.RequestCode);
            AppUpdateManager.GetAppUpdateInfo().AddOnSuccessListener(ResumeSuccessListener);
        });
    }
    
    /// <summary>
    /// This method will be triggered when the activity result is returned.
    /// </summary>
    /// <param name="activity"></param>
    /// <param name="requestCode"></param>
    /// <param name="resultCode"></param>
    /// <param name="data"></param>
    public static void HandleActivityResult(
        Activity activity,
        int requestCode,
        [GeneratedEnum] Result resultCode,
        Intent? data)
    {
        Options.DebugAction($"Activity result received: RequestCode={requestCode}, ResultCode={resultCode}");
        if (requestCode != Options.RequestCode)
        {
            return;
        }
        
        // The switch block will be triggered only with flexible update since
        // it returns the install result codes
        switch (resultCode)
        {
            case Result.Ok:
                Options.AppUpdatedAction();
                break;
            
            case Result.Canceled:
                Options.UpdateCancelledAction();
                break;
            
            case (Result)ActivityResult.ResultInAppUpdateFailed:
                Options.UpdateFailedAction();
                break;
        }
    }
}
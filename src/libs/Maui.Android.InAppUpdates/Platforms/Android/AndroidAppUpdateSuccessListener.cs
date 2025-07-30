using Android.App;
using Android.Gms.Tasks;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model;
using InAppUpdates.iOSAndroid.Maui.Common;

// ReSharper disable once CheckNamespace
namespace InAppUpdates.iOSAndroid.Maui.Internal;

public class AndroidAppUpdateSuccessListener(
    IAppUpdateManager appUpdateManager,
    Activity activity,
    int updateRequest)
    : Java.Lang.Object, IOnSuccessListener
{
    public AndroidInstallStateUpdatedListener? InstallStateUpdatedListener { get; private set; }

    public void OnSuccess(Java.Lang.Object? result)
    {
        if (result is not AppUpdateInfo info)
        {
            return;
        }

        AndroidHandler.Options.DebugAction($"AVAILABLE VERSION CODE {info.AvailableVersionCode()}");

        var updateAvailability = info.UpdateAvailability();
        var updatePriority = info.UpdatePriority();
        var isImmediateUpdatesAllowed = info.IsUpdateTypeAllowed(AppUpdateType.Immediate);
        var isFlexibleUpdatesAllowed = info.IsUpdateTypeAllowed(AppUpdateType.Flexible);

        // Get the available version as a string for caching
        string? availableVersion = info.AvailableVersionCode().ToString();

        // Create an instance of AppUpdatePreferenceCheck to update the cache
        var appUpdatePreferenceCheck = new AppUpdatePreferenceCheck();

        System.Threading.Tasks.Task.Run(async () =>
        {
            AndroidHandler.Options.DebugAction("Updating cache with available version -> " + availableVersion);
            // Update the cache with the version information we've checked
            await appUpdatePreferenceCheck.UpdateCache(availableVersion, AndroidHandler.Options);
        });

        switch (updateAvailability)
        {
            case UpdateAvailability.UpdateAvailable or
                UpdateAvailability.DeveloperTriggeredUpdateInProgress
                when updatePriority >= AndroidHandler.Options.ImmediateUpdatePriority &&
                     isImmediateUpdatesAllowed:
            {
                _ = appUpdateManager.StartUpdateFlowForResult(
                    info,
                    activity,
                    AppUpdateOptions
                        .NewBuilder(AppUpdateType.Immediate)
                        .SetAllowAssetPackDeletion(AndroidHandler.Options.AllowAssetPackDeletion)
                        .Build(),
                    updateRequest);
                break;
            }

            case UpdateAvailability.UpdateAvailable or
                UpdateAvailability.DeveloperTriggeredUpdateInProgress
                when isFlexibleUpdatesAllowed:
            {
                InstallStateUpdatedListener ??= new AndroidInstallStateUpdatedListener();
                appUpdateManager.RegisterListener(InstallStateUpdatedListener);
            
                _ = appUpdateManager.StartUpdateFlowForResult(
                    info,
                    activity,
                    AppUpdateOptions
                        .NewBuilder(AppUpdateType.Flexible)
                        .SetAllowAssetPackDeletion(AndroidHandler.Options.AllowAssetPackDeletion)
                        .Build(),
                    updateRequest);
                break;
            }

            case UpdateAvailability.UpdateNotAvailable:
            case UpdateAvailability.Unknown:
                AndroidHandler.Options.DebugAction($"UPDATE NOT AVAILABLE {info.AvailableVersionCode()}");
                break;
        }
    }
}
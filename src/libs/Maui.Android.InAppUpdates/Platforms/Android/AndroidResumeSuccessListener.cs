using Android.Gms.Tasks;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model;
using Activity = Android.App.Activity;

// ReSharper disable once CheckNamespace
namespace InAppUpdates.iOSAndroid.Maui.Internal;

/// <summary>
/// Whenever the user brings your app to the foreground,
/// check whether your app has an update waiting to be installed.
/// If your app has an update in the DOWNLOADED state, prompt the user to install the update.
/// Otherwise, the update data continues to occupy the user's device storage. <br/>
/// According: https://developer.android.com/guide/playcore/in-app-updates/kotlin-java#install-flexible
/// </summary>
/// <param name="appUpdateManager"></param>
/// <param name="activity"></param>
/// <param name="updateRequest"></param>
public class AndroidResumeSuccessListener(
    IAppUpdateManager appUpdateManager,
    Activity activity,
    int updateRequest)
    : Java.Lang.Object, IOnSuccessListener
{
    public void OnSuccess(Java.Lang.Object? result)
    {
        if (result is not AppUpdateInfo info)
        {
            return;
        }
        
        // https://developer.android.com/guide/playcore/in-app-updates/kotlin-java#install-flexible
        // If the update is downloaded but not installed,
        // notify the user to complete the update.
        if (info.InstallStatus() == InstallStatus.Downloaded)
        {
            AndroidHandler.Options.CompleteUpdateAction();
        }
        // https://developer.android.com/guide/playcore/in-app-updates/kotlin-java#immediate
        else if (info.UpdateAvailability() == UpdateAvailability.DeveloperTriggeredUpdateInProgress) {
            // If an in-app update is already running, resume the update.
            _ = appUpdateManager.StartUpdateFlowForResult(
                info,
                activity,
                AppUpdateOptions
                    .NewBuilder(AppUpdateType.Immediate)
                    .SetAllowAssetPackDeletion(AndroidHandler.Options.AllowAssetPackDeletion)
                    .Build(),
                updateRequest);
        }
    }
}
namespace InAppUpdates.iOSAndroid.Maui;

/// <summary>
/// Represents options for in-app updates on iOS and Android.
/// </summary>
public class InAppUpdatesOptions
{
#if ANDROID
    /// <summary>
    /// This value is used to differentiate between multiple update 
    /// or request processes within your app.
    /// </summary>
    public const int DefaultRequestCode = 4711;
    
    /// <summary>
    /// Show the download progress. <br/>
    /// Default is false. <br/>
    /// </summary>
    public bool ShowDownload { get; set; }
    
    /// <summary>
    /// Sets the priority of the update for immediate updates. <br/>
    /// Default is >= 4. <br/>
    /// </summary>
    public int ImmediateUpdatePriority { get; set; } = 4;
    
    /// <summary>
    /// Set this to true to use the fake app update manager. <br/>
    /// </summary>
    public bool UseFakeAppUpdateManager { get; set; }
    
    /// <summary>
    /// By default, the Android system does not allow the automatic deletion of downloaded asset packs when the app is updated. <br/>
    /// The default setting (false) is primarily chosen to prevent potential data loss. <br/>
    /// Android strives to balance efficient storage management with the risk of inadvertently removing assets that might still be needed by the application. <br/>
    /// By not automatically deleting asset packs upon an app update, the system errs on the side of caution—preserving any downloaded content that might not 
    /// necessarily be included in the updated version of the app but could still be important for its functionality or user data continuity.  <br/>
    /// This approach allows developers to manage their app’s assets more deliberately and ensures that users don’t lose access to critical resources 
    /// due to an automatic cleanup process. <br/>
    /// </summary>
    public bool AllowAssetPackDeletion { get; set; }
    
    /// <summary>
    /// This value is used to differentiate between multiple update or request processes within your app. <br/>
    /// To avoid intersection with other libraries or request codes, choose a unique value within your application context.
    /// </summary>
    public int RequestCode { get; set; } = DefaultRequestCode;

    /// <summary>
    /// This action will be triggered when the app is updated. <br/>
    /// Default action will show the toast with english text. <br/>
    /// </summary>
    public Action AppUpdatedAction { get; set; } = static () =>
#if ANDROID
        Internal.AndroidUserInterface.ShowShortToast(
            text: "App update in progress");
#else
        {};
#endif
    
    /// <summary>
    /// This action will be triggered when the update is cancelled. <br/>
    /// Default action will show the toast with english text. <br/>
    /// </summary>
    public Action UpdateCancelledAction { get; set; } = static () =>
#if ANDROID
        Internal.AndroidUserInterface.ShowShortToast(
            text: "In app update cancelled");
#else
        {};
#endif
    
    /// <summary>
    /// This action will be triggered when the update is failed. <br/>
    /// Default action will show the toast with english text. <br/>
    /// </summary>
    public Action UpdateFailedAction { get; set; } = static () =>
#if ANDROID
        Internal.AndroidUserInterface.ShowShortToast(
            text: "In app update failed");
#else
        {};
#endif
    
    /// <summary>
    /// This action will be triggered when the download is failed. <br/>
    /// Default action will show the toast with english text. <br/>
    /// </summary>
    public Action DownloadFailedAction { get; set; } = static () =>
#if ANDROID
        Internal.AndroidUserInterface.ShowShortToast(
            text: "Update download failed.");
#else
        {};
#endif
    
    /// <summary>
    /// This action will be triggered when downloading the update. <br/>
    /// Second value is the percentage of the download. <br/>
    /// Default action will show the toast with english text. <br/>
    /// </summary>
    public Action<double> DownloadingAction { get; set; } = static percents =>
#if ANDROID
        Internal.AndroidUserInterface.ShowShortToast(
            text: $"Downloaded {percents}%");
#else
        {};
#endif
    
    /// <summary>
    /// This action will be triggered when the update is completed. <br/>
    /// Default action will show the alert dialog to complete the update. <br/>
    /// </summary>
    public Action CompleteUpdateAction { get; set; } = static () =>
#if ANDROID
        Internal.AndroidUserInterface.ShowSnackbar(
            text: "An update has just been downloaded.",
            actionText: "RESTART",
            clickHandler: static _ => Internal.AndroidHandler.AppUpdateManager?.CompleteUpdate());
#else
        {};
#endif

#endif

    /// <summary>
    /// This action will be triggered when debug event occurs. <br/>
    /// Default action will write the text to the debug output. <br/>
    /// </summary>
    public Action<string> DebugAction { get; set; } = static text =>
        System.Diagnostics.Debug.WriteLine(text);

    /// <summary>
    /// Gets or sets the interval, in days, at which the application checks for updates.
    /// </summary>
    public int AppUpdateCheckIntervalDays { get; set; } = 0;

    /// <summary>
    /// Gets or sets the name of the file used to store application update preferences.
    /// </summary>
    public string AppUpdatePreferenceFileName { get; set; } = "AppUpdatePreferences";

#if IOS

    /// <summary>
    /// Gets or sets the unique identifier for the application bundle.
    /// </summary>
    public string AppBundleID { get; set; }

    public string AppPackageName { get; set; }
    /// <summary>
    /// Gets or sets the country code associated with the application.
    /// </summary>
    public string AppCountryCode { get; set; }

    /// <summary>
    /// Gets or sets the title displayed in the alert dialog for application updates.
    /// </summary>
    public string AppUpdateAlertTitle { get; set; }

    /// <summary>
    /// Gets or sets the message displayed to alert users about an available application update.
    /// </summary>
    public string AppUpdateAlertMessage { get; set; } 

    /// <summary>
    /// Gets or sets the text displayed on the "Yes" button in the application update alert dialog.
    /// </summary>
    public string AppUpdateAlertButtonYesTxt { get; set; }

    /// <summary>
    /// Gets or sets the text displayed on the "No" button in the application update alert.
    /// </summary>
    public string AppUpdateAlertButtonNoTxt { get; set; }

    /// <summary>
    /// Gets the delay, in seconds, after the splash screen before the application checks for updates.
    /// </summary>
    public int AppUpdateDelayAfterSplashInSeconds { get; set; }

#endif
}
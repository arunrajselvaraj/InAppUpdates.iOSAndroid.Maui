
namespace InAppUpdates.iOSAndroid.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.UseInAppUpdates(static options =>
			{
				options.AppUpdateCheckIntervalDays = 0; // Check for updates every time the app starts
                options.AppUpdatePreferenceFileName = "AppStoreVersionPreference";
				
#if ANDROID
                options.ImmediateUpdatePriority = 2;
#endif
#if DEBUG
#if ANDROID
				options.UseFakeAppUpdateManager = true;
#endif

				options.DebugAction = (applog) =>
                {
                    Console.WriteLine("Debug action executed: " + applog);
                };
#endif

#if IOS
				options.AppBundleID = "6741144561";
				options.AppPackageName = "com.company.example";
				options.AppCountryCode = "au";
				options.AppUpdateAlertTitle = "Update Available";
				options.AppUpdateAlertMessage = "A new version {0} of the app is available. " +
												"Please update to continue using the app.";
				options.AppUpdateAlertButtonYesTxt = "Update Now";
				options.AppUpdateAlertButtonNoTxt = "Later";

				options.AppUpdateDelayAfterSplash = 0.5;

				var bundleIdentifier = Foundation.NSBundle.MainBundle.BundleIdentifier;

				options.DebugAction("Arun: " + bundleIdentifier);
#endif


            })
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});


		return builder.Build();
	}
}

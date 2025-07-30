# Usage
- Add NuGet package to your project:
```xml
    <PackageReference Include="InAppUpdates.iOSAndroid.Maui" Version="0.0.5" />
```
- Add the following to your `MauiProgram.cs` `CreateMauiApp` method:
```diff
  builder
	.UseMauiApp<App>()
+	.UseInAppUpdates(static options =>
	{
		options.AppUpdateCheckIntervalDays = 0; // Check for updates every time the app starts
        options.AppUpdatePreferenceFileName = "AppStoreVersionPreference";
#if IOS
        options.AppBundleID = "6741144561";
		options.AppPackageName = "com.company.example";
		options.AppCountryCode = "au";
		options.AppUpdateAlertTitle = "Update Available";
		options.AppUpdateAlertMessage = "A new version {0} of the app is available. " +
										"Please update to continue using the app.";
		options.AppUpdateAlertButtonYesTxt = "Update Now";
		options.AppUpdateAlertButtonNoTxt = "Later";

		options.AppUpdateDelayAfterSplashInSeconds = 60;
#endif

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

    })
	.ConfigureFonts(fonts =>
	{
		fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
		fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
	});
```

# iOS

## Notes
 - Properties mentioned in the iOS Macro are required for iOS
 - By using the bundle ID, the app will be able to check for updates in the App Store.
 - Package name used to redirect iOS Store, when update is available.

# Android 

## Notes
The default behavior:
- If priority 1-3 is specified, flexible update will be offered
- If priority 4-5 is specified, immediate update will be offered

There is no need to specify conditional compilation here, the package provides an empty method for non netx.0-android platforms as a stub to make integration as simple as possible.  
It will display a window when starting an application or resume according to the official guides.  
You cannot see the popup dialog while developing or if you distribute it manually. 
As you can [see here](https://developer.android.com/guide/playcore/in-app-review/test), 
you have to download the app from the Play Store to see the popup. 
I recommend using Android Play Store's [“Internal App Sharing”](https://play.google.com/console/about/internalappsharing/) feature to test.  


# Reference Links
- https://developer.android.com/guide/playcore/in-app-updates/kotlin-java
- https://github.com/PatGet/XamarinPlayCoreUpdater
- https://github.com/xamarin/GooglePlayServicesComponents/issues/796
- https://github.com/PatGet/XamarinPlayCoreUpdater/issues/22
- https://github.com/PatGet/XamarinPlayCoreUpdater/issues/17
- https://github.com/PatGet/XamarinPlayCoreUpdater/pull/20#issuecomment-1273774958
- https://stackoverflow.com/questions/56218160/how-to-implement-google-play-in-app-update-and-use-play-core-library-with-xamari

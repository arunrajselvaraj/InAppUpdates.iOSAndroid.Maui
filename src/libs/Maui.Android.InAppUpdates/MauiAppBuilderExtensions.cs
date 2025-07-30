
using Microsoft.Maui.LifecycleEvents;

namespace InAppUpdates.iOSAndroid.Maui;

/// <summary>
/// This class contains the extension method to enable the in-app updates
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// This method will enable the in-app updates for Android and iOS.
    /// Set debugMode to true to enable the fake app update manager.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static MauiAppBuilder UseInAppUpdates(
        this MauiAppBuilder builder,
        Action<InAppUpdatesOptions>? setupAction = null) 
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        
#if ANDROID
        setupAction?.Invoke(Internal.AndroidHandler.Options);
        
        return builder
            .ConfigureLifecycleEvents(static events =>
            {
                events.AddAndroid(static android =>
                {
                    android
                        .OnActivityResult(Internal.AndroidHandler.HandleActivityResult)
                        .OnCreate(Internal.AndroidHandler.HandleCreate)
                        .OnResume(Internal.AndroidHandler.HandleResume)
                        ;
                });
            });
#elif IOS
        setupAction?.Invoke(Internal.IOSHandler.Options);
        
        return builder
            .ConfigureLifecycleEvents(static events =>
            {
                events.AddiOS(static ios =>
                {
                    ios
                        .OnActivated(Internal.IOSHandler.HandleActivated)
                        //Enable this on needed basis. 
                        //.OnResignActivation(Internal.IOSHandler.HandleResume)
                        ;
                });
            });
#else
        return builder;
#endif
    }
}

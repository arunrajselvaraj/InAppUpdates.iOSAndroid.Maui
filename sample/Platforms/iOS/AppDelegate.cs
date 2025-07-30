using Foundation;
using Maui.InAppUpdates;

namespace Maui.Android.InAppUpdates;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

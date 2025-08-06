using CommunityToolkit.Mvvm.Input;
#if ANDROID
using Xamarin.Google.Android.Play.Core.AppUpdate.Testing;
#endif

namespace InAppUpdates.iOSAndroid.Maui;

public partial class MainPage : ContentPage
{  
    public string AppBundleID => "1234567890"; // Replace with your actual App Bundle ID
    public string AppPackageName => "com.example.app"; // Replace with your actual App Package Name

#if ANDROID
	private int _availableVersionCode = 2;
#endif

	public MainPage()
	{
		InitializeComponent();
		BindingContext = this;
	}

	[RelayCommand]
	private void SetUpdateAvailableWithPriorityOf5()
	{
#if ANDROID
		FakeAppUpdateManager.SetUpdateAvailable(availableVersionCode: _availableVersionCode++);
		FakeAppUpdateManager.SetUpdatePriority(updatePriority: 5);
		AddOnSuccessListener();
#endif
	}

	[RelayCommand]
	private void SetUpdateAvailableWithPriorityOf3()
	{
#if ANDROID
		FakeAppUpdateManager.SetUpdateAvailable(availableVersionCode: _availableVersionCode++);
		FakeAppUpdateManager.SetUpdatePriority(updatePriority: 3);
		AddOnSuccessListener();
#endif
	}
	
	[RelayCommand]
	private void UserAcceptsUpdate()
	{
#if ANDROID
		FakeAppUpdateManager.UserAcceptsUpdate();
#endif
	}
	
	[RelayCommand]
	private void DownloadStarts()
	{
#if ANDROID
		FakeAppUpdateManager.SetBytesDownloaded(0);
		FakeAppUpdateManager.SetTotalBytesToDownload(10_000_000);
		FakeAppUpdateManager.DownloadStarts();
#endif
	}
	
	[RelayCommand]
	private void DownloadCompletes()
	{
#if ANDROID
		FakeAppUpdateManager.SetBytesDownloaded(10_000_000);
		FakeAppUpdateManager.DownloadCompletes();
#endif
	}
	
	[RelayCommand]
	private void InstallCompletes()
	{
#if ANDROID
		FakeAppUpdateManager.InstallCompletes();
#endif
	}
	
	[RelayCommand]
	private void InstallFails()
	{
#if ANDROID
		FakeAppUpdateManager.InstallFails();
#endif
	}
	
	[RelayCommand]
	private void CompleteUpdate()
	{
#if ANDROID
		Internal.AndroidHandler.Options.CompleteUpdateAction();
#endif
	}
	
	[RelayCommand]
	private async Task Downloading()
	{
#if ANDROID
		for (var i = 0; i < 100; i += 10)
		{
			Internal.AndroidHandler.Options.DownloadingAction(i);
			await Task.Delay(TimeSpan.FromMilliseconds(250));
		}
#else
		await Task.Delay(TimeSpan.FromMilliseconds(250));
#endif
	}
	
#if ANDROID
	private static FakeAppUpdateManager FakeAppUpdateManager =>
		(Internal.AndroidHandler.AppUpdateManager as FakeAppUpdateManager)!;
	
	private static void AddOnSuccessListener()
	{
		FakeAppUpdateManager.GetAppUpdateInfo()?.AddOnSuccessListener(Internal.AndroidHandler.AppUpdateSuccessListener!);
	}
#endif
}


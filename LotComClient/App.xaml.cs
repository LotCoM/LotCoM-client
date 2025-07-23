using LotComClient.Views;

namespace LotComClient;

public partial class App : Application {
	public App() {
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState) {
		// create the Main Window
		Window MainWindow = new Window(new NavigationPage(new HomePage("Welcome")));
		// get the version of the assembly at entry (the app version number)
		string Version = System.Reflection.Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
		// create a TitleBar for the Window
		TitleBar MainWindowTitleBar = new TitleBar {
			Icon = "lotcom_logo.png",
			Title = "LotCom Client",
			Subtitle = $"v{Version}"      
		};
		// add the TitleBar to the Main Window
		MainWindow.TitleBar = MainWindowTitleBar;
		return MainWindow;
	}
}
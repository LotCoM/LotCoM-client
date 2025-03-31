using LotCoMClient.Views;

namespace LotCoMClient;

public partial class App : Application {
	public App() {
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState) {
		// create a TitleBar for the Window
		TitleBar MainWindowTitleBar = new TitleBar {
			Icon = "lotcom_logo.png",
			Title = "LotCom Client",
			Subtitle = "v0.1.0.0-alpha"      
		};
		// create the Main Window
		Window MainWindow = new Window(new DataTablePage("\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints\\4134-CRV-Pipe-Comp.txt", "Test Data Page"));
		MainWindow.TitleBar = MainWindowTitleBar;
		return MainWindow;
	}
}
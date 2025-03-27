using LotCoMClient.Views;

namespace LotCoMClient;

public partial class App : Application {
	public App() {
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState) {
		return new Window(new DataTablePage());
	}
}
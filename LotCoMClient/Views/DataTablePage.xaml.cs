namespace LotCoMClient.Views;

/// <summary>
/// Code-behind (View Layer) for the DataTablePage View.
/// </summary>
public partial class DataTablePage : ContentPage {
    /// <summary>
    /// ViewModel object controlling the logic of this Page.
    /// </summary>
	private readonly ViewModels.DataTableViewModel _viewModel;

    /// <summary>
    /// Creates a new DataTablePage.
    /// </summary>
    public DataTablePage() {
		// instantiate the ViewModel
        _viewModel = new ViewModels.DataTableViewModel("\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints\\4134-CRV-Pipe-Comp.txt");
        BindingContext = _viewModel;

        // create the page from XAML
		InitializeComponent();
    }
}
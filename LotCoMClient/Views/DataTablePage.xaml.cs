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
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    public DataTablePage(string DataTablePath, string PageTitle) {
		// instantiate the ViewModel
        _viewModel = new ViewModels.DataTableViewModel(DataTablePath, PageTitle);
        BindingContext = _viewModel;

        // create the page from XAML
		InitializeComponent();
    }
}
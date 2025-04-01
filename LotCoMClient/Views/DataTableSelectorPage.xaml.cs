using LotCoMClient.Models.Datasources;

namespace LotCoMClient.Views;

/// <summary>
/// Code-behind (View Layer) for the DataTableSelectorPage View.
/// </summary>
public partial class DataTableSelectorPage : DataTablePage {
    /// <summary>
    /// ViewModel object controlling the logic of this Page.
    /// </summary>
	private readonly ViewModels.DataTableSelectorViewModel _viewModel;

    /// <summary>
    /// Creates a new DataTableSelectorPage.
    /// </summary>
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    public DataTableSelectorPage(string DataTablePath, string PageTitle, string Department, Type RecordType, bool IsProcessAssigned = true) : base(DataTablePath, PageTitle, RecordType, IsProcessAssigned) {
		// instantiate the ViewModel
        _viewModel = new ViewModels.DataTableSelectorViewModel(DataTablePath, PageTitle, Department, RecordType, IsProcessAssigned);
        BindingContext = _viewModel;

        // create the page from XAML
		InitializeComponent();
    }

    /// <summary>
    /// Handler for the SelectedIndexChanged event from the PageProcessPicker control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPageProcessSelectionChanged(object sender, EventArgs e) {
        // invoke the ViewModel method to update the UI
        await _viewModel.UpdatePageProcess(PageProcessPicker);
        // update and collapse the Page's Left Frame Panel
        _viewModel.LeftFramePanelHeader = ((Process)PageProcessPicker.ItemsSource[_viewModel.SelectedProcessIndex]!).FullName;
        if (_viewModel.LeftFrameShown) {
            OnPageLeftFrameCollapseButtonClicked(PageLeftFrameCollapseButton, new EventArgs());
        }
    }
}
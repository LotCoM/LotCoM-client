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
    /// <param name="DataTablePath"></param>
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    /// <param name="RecordType">The subclass of DataRecord this Page is meant to display (PrintRecord || ScanRecord).</param>
    /// <param name="IsProcessAssigned">Indicates whether the Selector Page has been assigned a Process (True by default).</param>
    public DataTablePage(string DataTablePath, string PageTitle, Type RecordType, bool IsProcessAssigned = true) {
		// instantiate the ViewModel
        _viewModel = new ViewModels.DataTableViewModel(DataTablePath, PageTitle, RecordType, IsProcessAssigned);
        BindingContext = _viewModel;

        // create the page from XAML
		InitializeComponent();
    }

    /// <summary>
    /// Handler for the Clicked event from the PageLeftFrameCollapseButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnPageLeftFrameCollapseButtonClicked(object sender, EventArgs e) {
        // the Panel needs to collapse
        if (_viewModel.LeftFrameShown) {
            // set the Left Panel properties in the ViewModel
            _viewModel.LeftFrameShown = false;
            _viewModel.LeftFrameHidden = true;
            // 12 frame animation (150 -> 30 by increments of 10)
            while (_viewModel.LeftFrameWidth > 30) {
                // animate the panel shrinking
                _viewModel.LeftFrameWidth -= 10;
                // animate the collapse button rotating
                PageLeftFrameCollapseButton.Rotation += 15;
                await Task.Delay(1);
            }
        // the Panel needs to raise
        } else {
            // 12 frame animation (30 -> 150 by increments of 10)
            while (_viewModel.LeftFrameWidth < 150) {
                // animate the panel raising
                _viewModel.LeftFrameWidth += 10;
                // animate the collapse button rotating
                PageLeftFrameCollapseButton.Rotation += 15;
                await Task.Delay(1);
            }
            // set the Left Panel properties in the ViewModel
            _viewModel.LeftFrameShown = true;
            _viewModel.LeftFrameHidden = false;
        }
    }
}
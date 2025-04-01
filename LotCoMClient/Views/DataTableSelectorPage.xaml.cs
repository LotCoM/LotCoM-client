using LotCoMClient.Models.Datasources;

namespace LotCoMClient.Views;

/// <summary>
/// Code-behind (View Layer) for the DataTableSelectorPage View.
/// </summary>
public partial class DataTableSelectorPage : ContentPage {
    /// <summary>
    /// ViewModel object controlling the logic of this Page.
    /// </summary>
	private readonly ViewModels.DataTableSelectorViewModel _viewModel;

    /// <summary>
    /// Creates a new DataTableSelectorPage.
    /// </summary>
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    public DataTableSelectorPage(string DataTablePath, string PageTitle, string Department, Type RecordType, bool IsProcessAssigned = true) {
		// instantiate the ViewModel
        _viewModel = new ViewModels.DataTableSelectorViewModel(DataTablePath, PageTitle, Department, RecordType, IsProcessAssigned);
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

    /// <summary>
    /// Handler for the SelectedIndexChanged event from the PageProcessPicker control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPageProcessSelectionChanged(object sender, EventArgs e) {
        // invoke the ViewModel method to update the UI
        _viewModel.UpdatePageProcess(PageProcessPicker);
        // update and collapse the Page's Left Frame Panel
        _viewModel.LeftFramePanelHeader = ((Process)PageProcessPicker.ItemsSource[_viewModel.SelectedProcessIndex]!).FullName;
        if (_viewModel.LeftFrameShown) {
            OnPageLeftFrameCollapseButtonClicked(PageLeftFrameCollapseButton, new EventArgs());
        }
    }
}
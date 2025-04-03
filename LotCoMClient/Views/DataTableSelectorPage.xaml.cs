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
    /// Asynchronously evaluates the state of the Data property and sets the BodyTableHeader property accordingly.
    /// If Data is loaded, shows the Record count. Else, shows "Loading Records...".
    /// </summary>
    private async Task ConfigureBodyTableHeader() {
        await Task.Run(() => {
            // get the count of Data entries
            int DataCount;
            // do not do any processing if Data is null (no-Process instantiation)
            if (_viewModel.Data == null) {
                return;
            }
            // the Data property is set and is either loading or completed
            if (_viewModel.Data!.IsCompleted && _viewModel.Data.Result != null) {
                DataCount = _viewModel.Data.Result.Count;
                // update the BodyTableHeader to show the item count
                if (DataCount > 1) {
                    _viewModel.BodyTableHeader = $"Showing {DataCount} records";
                } else {
                    _viewModel.BodyTableHeader = $"Showing {DataCount} records";
                }
            // the data is not loaded yet; default BodyTableHeader property
            } else {
                _viewModel.BodyTableHeader = "Loading records...";
            }
        });
    }

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
        await Task.Delay(0);
        // the Panel needs to collapse
        if (_viewModel.LeftFrameShown) {
            // set the Left Panel properties in the ViewModel
            _viewModel.LeftFrameShown = false;
            _viewModel.LeftFrameHidden = true;
            // non-animated collapse
            _viewModel.LeftFrameWidth = 30;
            PageLeftFrameCollapseButton.Rotation += 180;
            // // 12 frame animation (150 -> 30 by increments of 10)
            // while (_viewModel.LeftFrameWidth > 30) {
            //     // animate the panel shrinking
            //     _viewModel.LeftFrameWidth -= 10;
            //     // animate the collapse button rotating
            //     PageLeftFrameCollapseButton.Rotation += 15;
            //     await Task.Delay(1);
            // }
        // the Panel needs to raise
        } else {
            // set the Left Panel properties in the ViewModel
            _viewModel.LeftFrameShown = true;
            _viewModel.LeftFrameHidden = false;
            // non-animated raise
            _viewModel.LeftFrameWidth = 150;
            PageLeftFrameCollapseButton.Rotation += 180;
            // // 12 frame animation (30 -> 150 by increments of 10)
            // while (_viewModel.LeftFrameWidth < 150) {
            //     // animate the panel raising
            //     _viewModel.LeftFrameWidth += 10;
            //     // animate the collapse button rotating
            //     PageLeftFrameCollapseButton.Rotation += 15;
            //     await Task.Delay(1);
            // }
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
    
    /// <summary>
    /// Handler for the PropertyChanged event from the PageDataTableListView control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnPageDataTableListViewPropertyChanged(object sender, EventArgs e) {
        // update BodyTableHeader property
        await ConfigureBodyTableHeader();
    }
}
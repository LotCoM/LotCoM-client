using LotCoMClient.Models.Datasources;

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
    /// Checks that the Table has DataRecords available. Updates the Data fields based on that Data.
    /// </summary>
    /// <returns></returns>
    private async Task ConfigureDataFields() {
        // perform the config logic on a new CPU thread
        await Task.Run(() => {
            // confirm that the Data property has completed its async task
            if (_viewModel.Data == null || _viewModel.Data.IsNotCompleted) {
                return;
            }
            List<string> Sortables = ["Part Number", "Part Name", "Quantity"];
            // add the variably-required DataRecord fields (only if Data is loaded)
            DataRecord SampleRecord;
            if (_viewModel.Data.Result != null && _viewModel.Data.Result.Count > 0) {
                SampleRecord = _viewModel.Data.Result[0];
                if (SampleRecord.IncludesJBKNumber) {
                    Sortables.Add("JBK Number");
                }
                if (SampleRecord.IncludesLotNumber) {
                    Sortables.Add("Lot Number");
                }
                if (SampleRecord.IncludesDeburrJBKNumber) {
                    Sortables.Add("Deburr JBK Number");
                }
                if (SampleRecord.IncludesDieNumber) {
                    Sortables.Add("Die Number");
                }
                if (SampleRecord.IncludesModelNumber) {
                    Sortables.Add("Model Number");
                }
                if (SampleRecord.IncludesHeatNumber) {
                    Sortables.Add("Heat Number");
                }
            }
            Sortables.AddRange(["Production Date", "Production Time", "Production Shift", "Operator ID"]);
            // update the ViewModel DataField property
            _viewModel.DataFields = Sortables;
        });
    }

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
        await Task.Delay(0);
        // the Panel needs to collapse
        if (_viewModel.LeftFrameShown) {
            // set the Left Panel properties in the ViewModel
            _viewModel.LeftFrameShown = false;
            _viewModel.LeftFrameHidden = true;
            // non-animated collapse
            _viewModel.LeftFrameWidth = 30;
            PageLeftFrameCollapseButton.Rotation += 180;
            // // 12 frame animation (250 -> 30 by increments of 10)
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
            _viewModel.LeftFrameWidth = 250;
            PageLeftFrameCollapseButton.Rotation += 180;
            // // 12 frame animation (30 -> 250 by increments of 10)
            // while (_viewModel.LeftFrameWidth < 250) {
            //     // animate the panel raising
            //     _viewModel.LeftFrameWidth += 10;
            //     // animate the collapse button rotating
            //     PageLeftFrameCollapseButton.Rotation += 15;
            //     await Task.Delay(1);
            // }
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
        await ConfigureDataFields();
    }

    /// <summary>
    /// Handler for the SelectedIndexChanged event from the ListViewSortingFieldPicker and ListViewSortingOrderPicker controls.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnSortParameterSelectedIndexChanged(object sender, EventArgs e) {
        // update ViewModel sorting indexes
        _viewModel.SelectedSortingFieldIndex = ListViewSortingFieldPicker.SelectedIndex;
        _viewModel.SelectedSortingOrderIndex = ListViewSortingOrderPicker.SelectedIndex;
        // invoke the ViewModel sort method
        await _viewModel.SortDataTable();
    }

    /// <summary>
    /// Handler for the SearchButtonPressed event from the ListViewSearchingSearchBar control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnListViewSearchButtonPressed(object sender, EventArgs e) {
        // invoke the ViewModel local sort method using the current search term
        string SearchTerm = ListViewSearchingSearchBar.Text;
        await _viewModel.SearchDataTable(SearchTerm);
    }
}
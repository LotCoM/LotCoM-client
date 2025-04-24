using LotCoMClient.Models.Datasources;
using LotCoMClient.Models.Options;

namespace LotCoMClient.Views;

/// <summary>
/// Code-behind (View Layer) for the DataTablePage View.
/// </summary>
public partial class DataTablePage : ContentPage 
{
    /// <summary>
    /// ViewModel object controlling the logic of this Page.
    /// </summary>
	private readonly ViewModels.DataTableViewModel _viewModel;
    
    /// <summary>
    /// The ViewModel's Options property, exposed for easier access.
    /// </summary>
    private readonly DataTablePageOptions _options;

    /// <summary>
    /// Asynchronously evaluates the state of the Data property and configures the Body Header accordingly.
    /// If Data is loaded, shows the Navigation Panel. Else, shows "Loading Records...".
    /// </summary>
    private async Task ConfigureBodyHeader() 
    {
        await Task.Run(() => 
        {
            // do not do any processing if Data is null (no-Process instantiation)
            if (_viewModel.Data == null) 
            {
                return;
            }
            // the Data property is set and is either loading or completed
            if (_viewModel.Data!.IsCompleted && _viewModel.Data.Result != null) 
            {
                // update the BodyTableHeader to show the item count and navigation
                _options.SetBodyHeaderModeToNavigation();
            } 
            else 
            {
                // the data is not loaded yet; default Body Table Header options
                _options.SetBodyHeaderModeToLabel("Loading Records...");
            }
        });
    }

    /// <summary>
    /// Checks that the Table has DataRecords available. Updates the Data fields based on that Data.
    /// </summary>
    /// <returns></returns>
    private async Task ConfigureDataFields() 
    {
        // perform the config logic on a new CPU thread
        await Task.Run(() => 
        {
            // confirm that the Data property has completed its async task
            if (_viewModel.Data == null || _viewModel.Data.IsNotCompleted) 
            {
                return;
            }
            List<string> Sortables = ["Part Number", "Part Name", "Quantity"];
            // add the variably-required DataRecord fields (only if Data is loaded)
            DataRecord SampleRecord;
            if (_viewModel.Data.Result != null && _viewModel.Data.Result.Count > 0) 
            {
                SampleRecord = _viewModel.Data.Result[0];
                if (SampleRecord.IncludesJBKNumber) 
                {
                    Sortables.Add("JBK Number");
                }
                if (SampleRecord.IncludesLotNumber) 
                {
                    Sortables.Add("Lot Number");
                }
                if (SampleRecord.IncludesDeburrJBKNumber) 
                {
                    Sortables.Add("Deburr JBK Number");
                }
                if (SampleRecord.IncludesDieNumber) 
                {
                    Sortables.Add("Die Number");
                }
                if (SampleRecord.IncludesModelNumber) 
                {
                    Sortables.Add("Model Number");
                }
                if (SampleRecord.IncludesHeatNumber) 
                {
                    Sortables.Add("Heat Number");
                }
            }
            Sortables.AddRange(["Production Date", "Production Time", "Production Shift", "Operator ID"]);
            // update the ViewModel DataFields and SearchableFields property
            _options.DataFields = Sortables;
            _options.SearchableFields = Sortables
                .Prepend("All")
                .ToList();
        });
    }

    /// <summary>
    /// Creates a new DataTablePage.
    /// </summary>
    /// <param name="DataTablePath"></param>
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    /// <param name="RecordType">The subclass of DataRecord this Page is meant to display (PrintRecord || ScanRecord).</param>
    /// <param name="IsProcessAssigned">Indicates whether the Selector Page has been assigned a Process (True by default).</param>
    public DataTablePage(string DataTablePath, string PageTitle, Type RecordType, bool IsProcessAssigned = true) 
    {
		// instantiate the ViewModel
        _viewModel = new ViewModels.DataTableViewModel(DataTablePath, PageTitle, RecordType, IsProcessAssigned);
        _options = _viewModel.Options;
        BindingContext = _viewModel;

        // create the page from XAML
		InitializeComponent();
    }

    /// <summary>
    /// Handler for the Clicked event from the PageLeftFrameCollapseButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnPageLeftFrameCollapseButtonClicked(object sender, EventArgs e) 
    {
        await Task.Delay(0);
        // the Panel needs to collapse
        if (_options.IsLeftPanelShown) 
        {
            // set the Left Panel properties in the Page Options
            _options.IsLeftPanelShown = false;
            _options.IsLeftPanelHidden = true;
            _options.LeftPanelWidth = (int)DataTablePageOptions.LeftPanelWidths.Closed;
            PageLeftFrameCollapseButton.Rotation += 180;
        // the Panel needs to raise
        } 
        else 
        {
            // set the Left Panel properties in the Page Options
            _options.IsLeftPanelShown = true;
            _options.IsLeftPanelHidden = false;
            _options.LeftPanelWidth = (int)DataTablePageOptions.LeftPanelWidths.Open;
            PageLeftFrameCollapseButton.Rotation += 180;
        }
    }

    /// <summary>
    /// Handler for the PropertyChanged event from the PageDataTableListView control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnPageDataTableListViewPropertyChanged(object sender, EventArgs e) 
    {
        // update BodyTableHeader property
        await ConfigureBodyHeader();
        await ConfigureDataFields();
    }

    /// <summary>
    /// Handler for the SelectedIndexChanged event from the ListViewSortingFieldPicker and ListViewSortingOrderPicker controls.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnSortParameterSelectedIndexChanged(object sender, EventArgs e) 
    {
        await Task.Run(() => 
        {
            // update ViewModel sorting indexes
            _options.SelectedSortingFieldIndex = ListViewSortingFieldPicker.SelectedIndex;
            _options.SelectedSortingOrderIndex = ListViewSortingOrderPicker.SelectedIndex;
            // invoke the ViewModel sort method
            _viewModel.SortDataTable();
        });
    }

    /// <summary>
    /// Handler for the SearchButtonPressed event from the ListViewSearchingSearchBar control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnListViewSearchButtonPressed(object sender, EventArgs e) 
    {
        await Task.Run(() => 
        {
            // invoke the ViewModel local sort method using the current search term
            _options.SearchTerm = ListViewSearchingSearchBar.Text;
            _options.SelectedSearchingFieldIndex = ListViewSearchingFieldPicker.SelectedIndex;
            _viewModel.SearchDataTable();
        });
    }
}
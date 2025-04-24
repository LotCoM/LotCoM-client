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
                _viewModel.Options.SetBodyHeaderModeToNavigation();
            } 
            else 
            {
                // the data is not loaded yet; default Body Table Header options
                _viewModel.Options.SetBodyHeaderModeToLabel("Loading Records...");
            }
        });
    }

    /// <summary>
    /// Checks that the Table has DataRecords available. Updates the Data and Search fields based on that Data.
    /// </summary>
    /// <returns></returns>
    private async Task ConfigurePageFields() 
    {
        // confirm that the Data property has completed its async task
        if (_viewModel.Data == null || _viewModel.Data.IsNotCompleted) 
        {
            return;
        }
        await _viewModel.Options.ConfigurePageFields();
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
        if (_viewModel.Options.IsLeftPanelShown) 
        {
            // set the Left Panel properties in the Page Options
            _viewModel.Options.IsLeftPanelShown = false;
            _viewModel.Options.IsLeftPanelHidden = true;
            _viewModel.Options.LeftPanelWidth = (int)DataTablePageOptions.LeftPanelWidths.Closed;
            PageLeftFrameCollapseButton.Rotation += 180;
        // the Panel needs to raise
        } 
        else 
        {
            // set the Left Panel properties in the Page Options
            _viewModel.Options.IsLeftPanelShown = true;
            _viewModel.Options.IsLeftPanelHidden = false;
            _viewModel.Options.LeftPanelWidth = (int)DataTablePageOptions.LeftPanelWidths.Open;
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
        await ConfigurePageFields();
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
            _viewModel.Options.SelectedSortingFieldIndex = ListViewSortingFieldPicker.SelectedIndex;
            _viewModel.Options.SelectedSortingOrderIndex = ListViewSortingOrderPicker.SelectedIndex;
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
            _viewModel.Options.SearchTerm = ListViewSearchingSearchBar.Text;
            _viewModel.Options.SelectedSearchingFieldIndex = ListViewSearchingFieldPicker.SelectedIndex;
            _viewModel.SearchDataTable();
        });
    }
}
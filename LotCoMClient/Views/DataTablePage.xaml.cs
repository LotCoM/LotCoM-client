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
    /// Asynchronously evaluates the state of the CurrentPage property and configures the Body Header accordingly.
    /// If CurrentPage is loaded, shows the Navigation Panel. Else, shows "Loading Records...".
    /// </summary>
    private async Task ConfigureBodyHeader() 
    {
        await Task.Run(() => 
        {
            // do not do any processing if there is no CurrentPage to show (no-Process instantiation)
            if (_viewModel.CurrentPage is null) 
            {
                return;
            }
            // the CurrentPage is set and is either loading or completed
            if (_viewModel.CurrentPage!.IsCompleted && _viewModel.CurrentPage.Result != null) 
            {
                // CurrentPage is loaded; update the BodyTableHeader to show the item count and navigation
                _viewModel.Options.SetBodyHeaderModeToNavigation();
            } 
            else 
            {
                // the CurrentPage is not loaded yet; default Body Table Header options
                _viewModel.Options.SetBodyHeaderModeToLabel("Loading records...");
            }
        });
    }

    /// <summary>
    /// Checks that the Table has a CurrentPage available. Updates the Data and Search fields based on that Page's DataRecords.
    /// </summary>
    /// <returns></returns>
    private async Task ConfigurePageFields() 
    {
        // confirm that the Data property has completed its async task
        if (_viewModel.CurrentPage is null || _viewModel.CurrentPage.IsNotCompleted) 
        {
            return;
        }
        await _viewModel.Options.ConfigurePageFields();
    }

    /// <summary>
    /// Handler for the Clicked event from the PageLeftFrameCollapseButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPageLeftFrameCollapseButtonClicked(object sender, EventArgs e) 
    {
        await Task.Delay(0);
        if (_viewModel.Options.IsLeftPanelShown) 
        {
            _viewModel.Options.CollapseLeftPanel();
        } 
        else 
        {
            _viewModel.Options.RaiseLeftPanel();
        }
        LeftPanelCollapseButton.Rotation += 180;
    }

    /// <summary>
    /// Handler for the PropertyChanged event from the PageDataTableListView control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPageDataTableListViewPropertyChanged(object sender, EventArgs e) 
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
    private async void OnSortParameterSelectedIndexChanged(object sender, EventArgs e) 
    {
        // update ViewModel sorting indexes
        if (ListViewSortingFieldPicker is null || ListViewSortingOrderPicker is null)
        {
            return;
        }
        _viewModel.Options.SelectedSortingFieldIndex = ListViewSortingFieldPicker.SelectedIndex;
        _viewModel.Options.SelectedSortingOrderIndex = ListViewSortingOrderPicker.SelectedIndex;
        // invoke the ViewModel sort method
        await _viewModel.SortPage();
    }

    /// <summary>
    /// Handler for the SearchButtonPressed event from the ListViewSearchingSearchBar control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnListViewSearchButtonPressed(object sender, EventArgs e) 
    {
        await Task.Run(() => 
        {
            // invoke the ViewModel local sort method using the current search term
            if (ListViewSearchingFieldPicker is null)
            {
                return;
            }
            _viewModel.Options.SearchTerm = ListViewSearchingSearchBar.Text;
            _viewModel.Options.SelectedSearchingFieldIndex = ListViewSearchingFieldPicker.SelectedIndex;
            _viewModel.SearchDataTable();
        });
    }

    /// <summary>
    /// Handler for the Clicked event from the OnGoToFirstPageButtonClicked control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnGoToFirstPageButtonClicked(object sender, EventArgs e)
    {
        await _viewModel.GoToFirstPage();
    }

    /// <summary>
    /// Handler for the Clicked event from the OnGoToPreviousPageButtonClicked control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnGoToPreviousPageButtonClicked(object sender, EventArgs e)
    {
        await _viewModel.GoToPreviousPage();
    }

    /// <summary>
    /// Handler for the Clicked event from the OnGoToNextPageButtonClicked control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnGoToNextPageButtonClicked(object sender, EventArgs e)
    {
        await _viewModel.GoToNextPage();
    }

    /// <summary>
    /// Handler for the Clicked event from the OnGoToLastPageButtonClicked control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnGoToLastPageButtonClicked(object sender, EventArgs e)
    {
        await _viewModel.GoToLastPage();
    }

    /// <summary>
    /// Handler for the SelectedIndexChanged event from the PageLengthPicker control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPageLengthPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        // get the newly selected PageLength value and update it in the ViewModel
        if (PageLengthPicker is null)
        {
            return;
        }
        int PageLength = (int)PageLengthPicker.ItemsSource[_viewModel.Options.SelectedPageLengthIndex]!;
        _viewModel.Options.PageLength = PageLength;
        // refresh the Data Table to use a new PageSet based on the selected PageLength
        await _viewModel.RefreshPages();
        _viewModel.ClearFilterOptions();
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
}
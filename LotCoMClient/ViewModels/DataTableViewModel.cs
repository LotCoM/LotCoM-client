using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMClient.Models.Datasources;
using LotCoMClient.Models.Options;
using LotCoMClient.Models.Services;
using System.Linq.Dynamic;

namespace LotCoMClient.ViewModels;

/// <summary>
/// ViewModel (ViewModel Layer) controlling the logic of the DataTablePage View class.
/// Interacts with the Model Layer to invoke business logic and retrieve data.
/// </summary>
public partial class DataTableViewModel : ObservableObject 
{
    private DataTable? _table;
    /// <summary>
    /// Serves the DataTable object for this Page's Database Table. 
    /// </summary>
    public DataTable? Table 
    {
        get {return _table;}
        set 
        {
            _table = value;
            OnPropertyChanged(nameof(_table));
            OnPropertyChanged(nameof(Table));
        }
    }

    private NotifyTaskCompletion<Models.Datasources.Page>? _basePage;
    /// <summary>
    /// An unmodified version of the DataRecord Page currently shown by the DataTablePage.
    /// </summary>
    public NotifyTaskCompletion<Models.Datasources.Page>? BasePage 
    {
        get {return _basePage;}
        set 
        {
            _basePage = value;
            OnPropertyChanged(nameof(_basePage));
            OnPropertyChanged(nameof(BasePage));
        }
    }

    private NotifyTaskCompletion<Models.Datasources.Page>? _currentPage;
    /// <summary>
    /// A modifyable version of the DataRecord Page currently shown by the DataTablePage.
    /// </summary>
    public NotifyTaskCompletion<Models.Datasources.Page>? CurrentPage 
    {
        get {return _currentPage;}
        set 
        {
            _currentPage = value;
            OnPropertyChanged(nameof(_currentPage));
            OnPropertyChanged(nameof(CurrentPage));
        }
    }

    private DataTablePageOptions _options = new DataTablePageOptions();
    /// <summary>
    /// Provides an Options structure to control the various settings of the DataTablePage.
    /// </summary>
    public DataTablePageOptions Options 
    {
        get {return _options;}
        private set
        {
            _options = value;
            OnPropertyChanged(nameof(_options));
            OnPropertyChanged(nameof(Options));
        }
    }

    /// <summary>
    /// Resolves a defined Property Name on DataRecord from a passed String.
    /// </summary>
    /// <param name="String"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static string ResolveDataRecordPropertyName(string String) 
    {
        // create a conversion Library to convert plaintext selections to DataRecord property names
        Dictionary<string, string> Conversions = new Dictionary<string, string>() 
        {
            {"Part Number", "RecordPart.PartNumber"},
            {"Part Name", "RecordPart.PartName"},
            {"Quantity", "Quantity"},
            {"JBK Number", "JBKNumber"},
            {"Lot Number", "LotNumber"},
            {"Deburr JBK Number", "DeburrJBKNumber"},
            {"Die Number", "DieNumber"},
            {"Model Number", "ModelNumber"},
            {"Heat Number", "HeatNumber"},
            {"Production Date", "RecordDate"},
            {"Production Time", "RecordTime"},
            {"Production Shift", "RecordShift"},
            {"Operator ID", "OperatorID"}
        };
        // resolve the property name from the Dictionary
        try 
        {
            return Conversions[String];
        } 
        catch 
        {
            throw new ArgumentException($"{String} is not a defined property on `DataRecord.`");
        }
    }

    /// <summary>
    /// Performs an in-place sort of the DataRecords property of CurrentPage.
    /// </summary>
    /// <param name="SortingProperty"></param>
    /// <param name="SortOrder"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    private async Task<NotifyTaskCompletion<Models.Datasources.Page>> SortCurrentPage(string SortingProperty, int SortOrder)
    {
        // confirm that the CurrentPage is available for operations
        if (CurrentPage is null 
            || CurrentPage.IsNotCompleted 
            || CurrentPage.Result is null)
        {
            throw new OperationCanceledException();
        }
        return await Task.Run(() => 
        {
            // use LINQ dynamic to sort using the property selected in the sorting field picker
            CurrentPage.Result.DataRecords = CurrentPage.Result.DataRecords
                .AsQueryable()
                .OrderBy(SortingProperty)
                .ToList();
            // invert the order (ascending by default) if descending sort was selected
            if (SortOrder == 1) 
            {
                CurrentPage.Result.DataRecords.Reverse();
            }
            return CurrentPage;
        });
    }

    /// <summary>
    /// Creates a ViewModel for the DataTablePage.
    /// </summary>
    /// <param name="DataTablePath">The desired display Database Table's full path.</param>
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    /// <param name="RecordType">The subclass of DataRecord this Page is meant to display (PrintRecord || ScanRecord).</param>
    /// <param name="IsProcessAssigned">Indicates whether the Selector Page has been assigned a Process (True by default).</param>
    public DataTableViewModel(string DataTablePath, string PageTitle, Type RecordType, bool IsProcessAssigned = true) 
    {
        // configure the Page's basic properties
        Options.Title = PageTitle;
        Options.RecordType = RecordType;
        Options.IsProcessAssigned = IsProcessAssigned;
        // configure the Page based on whether an initial Process is assigned
        if (Options.IsProcessAssigned) 
        {
            // create a DataTable from the path passed in DataTablePath
            Table = new DataTable(DataTablePath);
            // display the first Page in the DataTable with the default PageLength
            SetNewPage(new NotifyTaskCompletion<Models.Datasources.Page>(Table.RequestPage(0, Options.PageLength)));
            Options.Process = Table.Process;
            Options.LeftPanelHeaderText = Table.Process!.FullName;
            Options.SetBodyHeaderModeToLabel("Loading records...");
        // no Process is assigned at instantiation
        } 
        else 
        {
            Options.SetBodyHeaderModeToLabel("Select Process...");
        }
    }

    /// <summary>
    /// Sets the BasePage and CurrentPage properties to NewPage, resetting the Page's shown DataRecord Page.
    /// </summary>
    /// <param name="NewPage"></param>
    public void SetNewPage(NotifyTaskCompletion<Models.Datasources.Page> NewPage)
    {
        BasePage = NewPage;
        CurrentPage = NewPage;
    }

    /// <summary>
    /// Jumps to the first Page in the Table's current Pages (the newest Records).
    /// </summary>
    public async Task GoToFirstPage()
    {
        await Table!.GoToFirstPage();
        // update the DataTablePage to show the new Active Page of the PageSet
        SetNewPage(new NotifyTaskCompletion<Models.Datasources.Page>(Table!.ActivePageSet.GetActivePage()));
    }

    /// <summary>
    /// Goes to the previous Page in the Table's current Pages (if one exists).
    /// </summary>
    public async Task GoToPreviousPage()
    {
        await Table!.GoToPreviousPage();
        // update the DataTablePage to show the new Active Page of the PageSet
        SetNewPage(new NotifyTaskCompletion<Models.Datasources.Page>(Table!.ActivePageSet.GetActivePage()));
    }

    /// <summary>
    /// Jumps to the last Page in the Table's current Pages (the oldest Records).
    /// </summary>
    public async Task GoToLastPage()
    {
        await Table!.GoToLastPage();
        // update the DataTablePage to show the new Active Page of the PageSet
        SetNewPage(new NotifyTaskCompletion<Models.Datasources.Page>(Table!.ActivePageSet.GetActivePage()));
    }

    /// <summary>
    /// Goes to the next Page in the Table's current Pages (if one exists).
    /// </summary>
    public async Task GoToNextPage()
    {
        await Table!.GoToNextPage();
        // update the DataTablePage to show the new Active Page of the PageSet
        SetNewPage(new NotifyTaskCompletion<Models.Datasources.Page>(Table!.ActivePageSet.GetActivePage()));
    }

    /// <summary>
    /// Refreshes the DataTable's PageSets to display a new PageSet with the passed configurations.
    /// </summary>
    /// <param name="MaxCount">(Optional) Set a limit on the number of Pages allowed in the PageSet.</param>
    /// <returns></returns>
    public async Task RefreshPages(int MaxCount = -1)
    {
        await Table!.RefreshPages(MaxCount, Options.PageLength);
        // update the DataTablePage to show the Active Page of the new PageSet
        SetNewPage(new NotifyTaskCompletion<Models.Datasources.Page>(Table!.ActivePageSet.GetActivePage()));
    }

    /// <summary>
    /// Sorts the DataRecords in the CurrentPage property using the Sorting Field and orders it according to the Order selection.
    /// </summary>
    /// <returns></returns>
    public async Task SortPage()
    {
        // get the selected Field from the Sorting Field Picker
        string SortField = Options.DataFields[Options.SelectedSortingFieldIndex];
        SortField = ResolveDataRecordPropertyName(SortField);
        // sort using the Model class
        CurrentPage = await SortCurrentPage(SortField, Options.SelectedSortingOrderIndex);
    }

    /// <summary>
    /// Searches for match hits in the Data list of the DataTable.
    /// Configures the Data property to only show those match hits.
    /// </summary>
    /// <returns></returns>
    public void SearchDataTable() 
    {
        // get the selected Field from the Searching Field Picker
        string PropertyName = Options.SearchableFields[Options.SelectedSearchingFieldIndex];
        if (PropertyName != "All") 
        {
            PropertyName = ResolveDataRecordPropertyName(PropertyName);
        }
        // Search using the Model class
        CurrentPage = new NotifyTaskCompletion<Models.Datasources.Page>(Table!.SearchAsync(Options.SearchTerm, PropertyName, Options.PageLength));
    }
}
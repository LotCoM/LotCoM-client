using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMClient.Models.Datasources;
using LotCoMClient.Models.Options;
using LotCoMClient.Models.Services;

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

    private NotifyTaskCompletion<List<DataRecord>>? _data;
    /// <summary>
    /// Serves the Data in the Page's assigned Database Table.
    /// </summary>
    public NotifyTaskCompletion<List<DataRecord>>? Data 
    {
        get {return _data;}
        set 
        {
            _data = value;
            OnPropertyChanged(nameof(_data));
            OnPropertyChanged(nameof(Data));
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
            Data = new NotifyTaskCompletion<List<DataRecord>>(Table.ReadRecordsAsync());
            Options.Process = Table.Process;
            Options.LeftPanelHeaderText = Table.Process!.FullName;
            Options.SetBodyHeaderModeToLabel("Loading records...");
        // no Process is assigned at instantiation
        } 
        else 
        {
            // set the table to null
            Table = null;
            Data = null;
            Options.SetBodyHeaderModeToLabel("Select Process...");
        }
    }

    /// <summary>
    /// Sorts the DataRecords in the Data property using the Sorting Field and orders it according to the Order selection.
    /// </summary>
    /// <returns></returns>
    public void SortDataTable()
    {
        // get the selected Field from the Sorting Field Picker
        string SortField = Options.DataFields[Options.SelectedSortingFieldIndex];
        SortField = ResolveDataRecordPropertyName(SortField);
        // sort using the Model class
        Data = new NotifyTaskCompletion<List<DataRecord>>(Table!.RequestSort(SortField, Options.SelectedSortingOrderIndex));
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
        Data = new NotifyTaskCompletion<List<DataRecord>>(Table!.RequestSearch(Options.SearchTerm, PropertyName));
    }
}
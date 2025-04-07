using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMClient.Models.Datasources;
using LotCoMClient.Models.Services;

namespace LotCoMClient.ViewModels;

/// <summary>
/// ViewModel (ViewModel Layer) controlling the logic of the DataTablePage View class.
/// Interacts with the Model Layer to invoke business logic and retrieve data.
/// </summary>
public partial class DataTableViewModel : ObservableObject {
    private string _pageTitle = "";
    /// <summary>
    /// Serves the Page's Title.
    /// </summary>
    public string PageTitle {
        get {return _pageTitle;}
    }
    private Department? _pageDepartment = null;
    /// <summary>
    /// Serves the Department assigned to this Page.
    /// </summary>
    public Department? PageDepartment {
        get {return _pageDepartment;}
        set {
            _pageDepartment = value;
            OnPropertyChanged(nameof(_pageDepartment));
            OnPropertyChanged(nameof(PageDepartment));
        }
    }
    private Type _recordType;
    /// <summary>
    /// Serves the DataRecord subclass this Page is meant to Display.
    /// </summary>
    public Type RecordType {
        get {return _recordType;}
        set {
            _recordType = value;
            OnPropertyChanged(nameof(_recordType));
            OnPropertyChanged(nameof(RecordType));
        }
    }

    private string _leftFramePanelHeader;
    /// <summary>
    /// Serves the Page's Left Frame Panel Header.
    /// </summary>
    public string LeftFramePanelHeader {
        get {return _leftFramePanelHeader;}
        set {
            _leftFramePanelHeader = value;
            OnPropertyChanged(nameof(_leftFramePanelHeader));
            OnPropertyChanged(nameof(LeftFramePanelHeader));
        }
    }
    private string _leftFramePanelFooter = "Click to Collapse";
    /// <summary>
    /// Serves the Page's Left Frame Panel Footer.
    /// </summary>
    public string LeftFramePanelFooter {
        get {return _leftFramePanelFooter;}
        set {
            _leftFramePanelFooter = value;
            OnPropertyChanged(nameof(_leftFramePanelFooter));
            OnPropertyChanged(nameof(LeftFramePanelFooter));
        }
    }
    private bool _leftFrameShown = true;
    /// <summary>
    /// Serves the Left Frame Panel's Shown state (boolean).
    /// </summary>
    public bool LeftFrameShown {
        get {return _leftFrameShown;} 
        set {
            _leftFrameShown = value;
            OnPropertyChanged(nameof(_leftFrameShown));
            OnPropertyChanged(nameof(LeftFrameShown));
        }
    }
    private bool _leftFrameHidden = false;
    /// <summary>
    /// Serves the inverse of the Left Frame Panel's Shown state (boolean).
    /// </summary>
    public bool LeftFrameHidden {
        get {return _leftFrameHidden;} 
        set {
            _leftFrameHidden = value;
            OnPropertyChanged(nameof(_leftFrameHidden));
            OnPropertyChanged(nameof(LeftFrameHidden));
        }
    }
    /// <summary>
    /// Serves the assigned width of the Left Frame Panel (30 when collapsed, 150 when raised).
    /// </summary>
    private int _leftFrameWidth = 150;
    public int LeftFrameWidth {
        get {return _leftFrameWidth;} 
        set {
            _leftFrameWidth = value;
            OnPropertyChanged(nameof(_leftFrameWidth));
            OnPropertyChanged(nameof(LeftFrameWidth));
        }
    }
    private string _bodyTableHeader;
    /// <summary>
    /// Serves the Header for the Page's Table.
    /// </summary>
    public string BodyTableHeader {
        get {return _bodyTableHeader;} 
        set {
            _bodyTableHeader = value;
            OnPropertyChanged(nameof(_bodyTableHeader));
            OnPropertyChanged(nameof(BodyTableHeader));
        }
    }
    private List<string> _sortingFields;
    /// <summary>
    /// Serves the fields that can be used to sort the Page's ListView.
    /// </summary>
    public List<string> SortingFields {
        get {return _sortingFields;} 
        set {
            _sortingFields = value;
            OnPropertyChanged(nameof(_sortingFields));
            OnPropertyChanged(nameof(SortingFields));
        }
    }
    private DataTable? _table;
    /// <summary>
    /// Serves the DataTable object for this Page's Database Table. 
    /// </summary>
    public DataTable? Table {
        get {return _table;}
        set {
            _table = value;
            OnPropertyChanged(nameof(_table));
            OnPropertyChanged(nameof(Table));
        }
    }
    private NotifyTaskCompletion<List<DataRecord>>? _data;
    /// <summary>
    /// Serves the Data in the Page's assigned Database Table.
    /// </summary>
    public NotifyTaskCompletion<List<DataRecord>>? Data {
        get {return _data;}
        set {
            _data = value;
            OnPropertyChanged(nameof(_data));
            OnPropertyChanged(nameof(Data));
        }
    }

    /// <summary>
    /// Creates a ViewModel for the DataTablePage.
    /// </summary>
    /// <param name="DataTablePath">The desired display Database Table's full path.</param>
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    /// <param name="RecordType">The subclass of DataRecord this Page is meant to display (PrintRecord || ScanRecord).</param>
    /// <param name="IsProcessAssigned">Indicates whether the Selector Page has been assigned a Process (True by default).</param>
    public DataTableViewModel(string DataTablePath, string PageTitle, Type RecordType, bool IsProcessAssigned = true) {
        // assign properties
        _pageTitle = PageTitle;
        _recordType = RecordType;
        // configure the Page based on whether an initial Process is assigned
        if (IsProcessAssigned) {
            // create a DataTable from the path passed in DataTablePath
            _table = new DataTable(DataTablePath);
            _data = new NotifyTaskCompletion<List<DataRecord>>(_table.GetRecordsAsync());
            // set the left frame panel's header
            _leftFramePanelHeader = Table!.TableProcess;
            _bodyTableHeader = "Loading records...";
        // no Process is assigned at instantiation
        } else {
            // set the table to null
            _table = null;
            _data = null;
            // set the left frame panel's header to a default no process string
            _leftFramePanelHeader = "Select Process...";
            _bodyTableHeader = "";
        }
        // configure the sortable fields for this Page's table
        _sortingFields = ["Part Number", "Part Name", "Quantity", "Production Date", "Production Time", "Production Shift", "Operator ID"];
    }
}
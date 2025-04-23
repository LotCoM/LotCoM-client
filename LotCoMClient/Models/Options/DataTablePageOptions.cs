using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMClient.Models.Datasources;

namespace LotCoMClient.Models.Options;

/// <summary>
/// Encapsulates several options and controls that change the way a DataTablePage looks and functions.
/// </summary>
public class DataTablePageOptions() : ObservableObject() 
{
    /// <summary>
    /// Provides default Left Panel widths for Open and Closed states.
    /// </summary>
    public enum LeftPanelWidths {
        Closed = 30,
        Open = 250
    }

    private string _title = "";
    /// <summary>
    /// Controls the Page's Title which appears in the top-left corner.
    /// </summary>
    public string Title 
    {
        get {return _title;}
        set 
        {
            _title = value;
            OnPropertyChanged(nameof(_title));
            OnPropertyChanged(nameof(Title));
        }
    }

    private string _bodyTableHeaderText = "";
    /// <summary>
    /// Controls the text of the Page's Body Table Header.
    /// </summary>
    public string BodyTableHeaderText {
        get {return _bodyTableHeaderText;} 
        set 
        {
            _bodyTableHeaderText = value;
            OnPropertyChanged(nameof(_bodyTableHeaderText));
            OnPropertyChanged(nameof(BodyTableHeaderText));
        }
    }

    private string _leftPanelHeaderText = "";
    /// <summary>
    /// Controls the text shown in the Page's Left Panel Header which appears at the top of the collapsable Left Panel.
    /// </summary>
    public string LeftPanelHeaderText 
    {
        get {return _leftPanelHeaderText;}
        set 
        {
            _leftPanelHeaderText = value;
            OnPropertyChanged(nameof(_leftPanelHeaderText));
            OnPropertyChanged(nameof(LeftPanelHeaderText));
        }
    }

    private string _leftPanelFooterText = "Click to Collapse";
    /// <summary>
    /// Controls the text shown in the Page's Left Panel Footer which appears at the bottom of the collapsable Left Panel.
    /// </summary>
    public string LeftPanelFooterText 
    {
        get {return _leftPanelFooterText;}
        set 
        {
            _leftPanelFooterText = value;
            OnPropertyChanged(nameof(_leftPanelFooterText));
            OnPropertyChanged(nameof(LeftPanelFooterText));
        }
    }

    private bool _isLeftPanelShown = true;
    /// <summary>
    /// Controls the Page's Left Panel Shown state (boolean).
    /// </summary>
    public bool IsLeftPanelShown 
    {
        get {return _isLeftPanelShown;} 
        set 
        {
            _isLeftPanelShown = value;
            OnPropertyChanged(nameof(_isLeftPanelShown));
            OnPropertyChanged(nameof(IsLeftPanelShown));
        }
    }

    private bool _isLeftPanelHidden = false;
    /// <summary>
    /// Controls the Page's Left Panel Hidden state (boolean).
    /// </summary>
    public bool IsLeftPanelHidden 
    {
        get {return _isLeftPanelHidden;} 
        set 
        {
            _isLeftPanelHidden = value;
            OnPropertyChanged(nameof(_isLeftPanelHidden));
            OnPropertyChanged(nameof(IsLeftPanelHidden));
        }
    }

    private int _leftPanelWidth = (int)LeftPanelWidths.Open;
    /// <summary>
    /// Controls the width of the Page's Left Panel.
    /// </summary>
    public int LeftPanelWidth 
    {
        get {return _leftPanelWidth;} 
        set 
        {
            _leftPanelWidth = value;
            OnPropertyChanged(nameof(_leftPanelWidth));
            OnPropertyChanged(nameof(LeftPanelWidth));
        }
    }
    
    private List<string> _dataFields = ["Part Number", "Part Name", "Quantity", "Production Date", "Production Time", "Production Shift", "Operator ID"];
    /// <summary>
    /// Controls the data fields that are included in DataRecords for the Page's ListView.
    /// </summary>
    public List<string> DataFields 
    {
        get {return _dataFields;} 
        set 
        {
            _dataFields = value;
            OnPropertyChanged(nameof(_dataFields));
            OnPropertyChanged(nameof(DataFields));
        }
    }

    private List<string> _searchableFields = ["All", "Part Number", "Part Name", "Quantity", "Production Date", "Production Time", "Production Shift", "Operator ID"];
    /// <summary>
    /// Controls the fields that can be used to search the Page's ListView.
    /// </summary>
    public List<string> SearchableFields 
    {
        get {return _searchableFields;}
        set 
        {
            _searchableFields = value;
            OnPropertyChanged(nameof(_searchableFields));
            OnPropertyChanged(nameof(SearchableFields));
        }
    }

    private int _selectedSortingFieldIndex;
    /// <summary>
    /// Controls the currently selected index of the Page's SortingField Picker.
    /// </summary>
    public int SelectedSortingFieldIndex 
    {
        get {return _selectedSortingFieldIndex;}
        set 
        {
            _selectedSortingFieldIndex = value;
            OnPropertyChanged(nameof(_selectedSortingFieldIndex));
            OnPropertyChanged(nameof(SelectedSortingFieldIndex));
        }
    }

    private int _selectedSortingOrderIndex;
    /// <summary>
    /// Controls the currently selected index of the Page's SortingOrder Picker.
    /// </summary>
    public int SelectedSortingOrderIndex 
    {
        get {return _selectedSortingOrderIndex;}
        set 
        {
            _selectedSortingOrderIndex = value;
            OnPropertyChanged(nameof(_selectedSortingOrderIndex));
            OnPropertyChanged(nameof(SelectedSortingOrderIndex));
        }
    }

    private int _selectedSearchingFieldIndex;
    /// <summary>
    /// Controls the currently selected index of the Page's SearchingField Picker.
    /// </summary>
    public int SelectedSearchingFieldIndex 
    {
        get {return _selectedSearchingFieldIndex;}
        set 
        {
            _selectedSearchingFieldIndex = value;
            OnPropertyChanged(nameof(_selectedSearchingFieldIndex));
            OnPropertyChanged(nameof(SelectedSearchingFieldIndex));
        }
    }

    private string _searchTerm = "";
    /// <summary>
    /// Controls the currently entered Text value of the Page's ListViewSearchBar.
    /// </summary>
    public string SearchTerm 
    {
        get {return _searchTerm;}
        set 
        {
            _searchTerm = value;
            OnPropertyChanged(nameof(_searchTerm));
            OnPropertyChanged(nameof(SearchTerm));
        }
    }

    public Department? _department = null;
    /// <summary>
    /// Controls the Department that defines the Process currently shown on the Page.
    /// </summary>
    public Department? Department 
    {
        get {return _department;}
        set 
        {
            _department = value;
            OnPropertyChanged(nameof(_department));
            OnPropertyChanged(nameof(Department));
        }
    }

    private List<Process> _departmentProcesses = [];
    /// <summary>
    /// For Selector Pages;
    /// Controls the Processes that are selectable in the Page's PageProcess Picker.
    /// </summary>
    public List<Process> DepartmentProcesses 
    {
        get {return _departmentProcesses;}
        private set 
        {
            _departmentProcesses = value;
            OnPropertyChanged(nameof(_departmentProcesses));
            OnPropertyChanged(nameof(DepartmentProcesses));
        }
    }

    private int _selectedProcessIndex = -1;
    /// <summary>
    /// For Selector Pages;
    /// Controls the currently selected index of the Page's PageProcess Picker.
    /// </summary>
    public int SelectedProcessIndex 
    {
        get {return _selectedProcessIndex;}
        set 
        {
            _selectedProcessIndex = value;
            OnPropertyChanged(nameof(_selectedProcessIndex));
            OnPropertyChanged(nameof(SelectedProcessIndex));
        }
    }

    private Type _recordType = typeof(DataRecord);
    /// <summary>
    /// Controls the subclass of DataRecord displayed by the Page's ListView.
    /// </summary>
    public Type RecordType 
    {
        get {return _recordType;}
        set 
        {
            _recordType = value;
            OnPropertyChanged(nameof(_recordType));
            OnPropertyChanged(nameof(RecordType));
        }
    }

    private bool _isProcessAssigned;
    /// <summary>
    /// Controls the boolean condition of whether a Process has been assigned to the Page.
    /// </summary>
    public bool IsProcessAssigned 
    {
        get {return _isProcessAssigned;}
        set 
        {
            _isProcessAssigned = value;
            OnPropertyChanged(nameof(_isProcessAssigned));
            OnPropertyChanged(nameof(IsProcessAssigned));
        }
    }
}
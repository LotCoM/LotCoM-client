using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMClient.Models.Datasources;

namespace LotCoMClient.ViewModels;

/// <summary>
/// ViewModel (ViewModel Layer) controlling the logic of the DataTablePage View class.
/// Interacts with the Model Layer to invoke business logic and retrieve data.
/// </summary>
public partial class DataTableViewModel : ObservableObject {
    private List<DataRecord> _data = [];
    /// <summary>
    /// Serves the Data in the Page's assigned Database Table.
    /// </summary>
    public List<DataRecord> Data {
        get {return _data;}
        set {
            _data = value;
            OnPropertyChanged(nameof(_data));
            OnPropertyChanged(nameof(Data));
        }
    }
    private string _pageTitle = "";
    /// <summary>
    /// Serves the Page's Title.
    /// </summary>
    public string PageTitle {
        get {return _pageTitle;}
    }
    private string _leftFramePanelHeader = "Panel Header";
    /// <summary>
    /// Serves the Page's Left Frame Panel Header.
    /// </summary>
    public string LeftFramePanelHeader {
        get {return _leftFramePanelHeader;}
    }
    private string _leftFramePanelFooter = "Panel Footer";
    /// <summary>
    /// Serves the Page's Left Frame Panel Footer.
    /// </summary>
    public string LeftFramePanelFooter {
        get {return _leftFramePanelFooter;}
    }
    
    // UI visual controls
    private bool _leftFrameShown = true;
    public bool LeftFrameShown {
        get {return _leftFrameShown;} 
        set {
            _leftFrameShown = value;
            OnPropertyChanged(nameof(_leftFrameShown));
            OnPropertyChanged(nameof(LeftFrameShown));
        }
    }
    private bool _leftFrameHidden = false;
    public bool LeftFrameHidden {
        get {return _leftFrameHidden;} 
        set {
            _leftFrameHidden = value;
            OnPropertyChanged(nameof(_leftFrameHidden));
            OnPropertyChanged(nameof(LeftFrameHidden));
        }
    }
    private int _leftFrameWidth = 150;
    public int LeftFrameWidth {
        get {return _leftFrameWidth;} 
        set {
            _leftFrameWidth = value;
            OnPropertyChanged(nameof(_leftFrameWidth));
            OnPropertyChanged(nameof(LeftFrameWidth));
        }
    }

    /// <summary>
    /// Private property that holds the DataTable object for this Page's Database Table. 
    /// </summary>
    private DataTable _table;

    /// <summary>
    /// Creates a ViewModel for the DataTablePage.
    /// </summary>
    /// <param name="DataTablePath">The desired display Database Table's full path.</param>
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    public DataTableViewModel(string DataTablePath, string PageTitle) {
        // assign the Page's Title
        _pageTitle = PageTitle;
        // create a DataTable from the path passed in DataTablePath
        _table = new DataTable(DataTablePath);
        // read the Table
        _data = _table.GetRecords();
    }
}
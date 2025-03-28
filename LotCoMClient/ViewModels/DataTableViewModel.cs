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

    /// <summary>
    /// Private property that holds the DataTable object for this Page's Database Table. 
    /// </summary>
    private DataTable _table;

    /// <summary>
    /// Creates a ViewModel for the DataTablePage.
    /// </summary>
    /// <param name="DataTablePath"></param>
    public DataTableViewModel(string DataTablePath) {
        // create a DataTable from the path passed in DataTablePath
        _table = new DataTable(DataTablePath);
        // read the Table
        _data = _table.GetRecords();
    }
}
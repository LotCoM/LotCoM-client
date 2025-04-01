using LotCoMClient.Models.Datasources;

namespace LotCoMClient.ViewModels;

/// <summary>
/// ViewModel (ViewModel Layer) controlling the logic of the DataTableSelectorPage View class.
/// Interacts with the Model Layer to invoke business logic and retrieve data.
/// </summary>
public partial class DataTableSelectorViewModel : DataTableViewModel {
    // add a Processes property
    private List<Process> _processes = ProcessData.GetProcesses();
    /// <summary>
    /// Serves the selectable Processes for this Page.
    /// </summary>
    public List<Process> Processes {
        get {return _processes;}
    }
    // add a SelectedProcessIndex (for the ProcessPicker) property
    private int _selectedProcessIndex = -1;
    /// <summary>
    /// Serves the currently selected index of the ProcessPicker Control.
    /// </summary>
    public int SelectedProcessIndex {
        get {return _selectedProcessIndex;}
        set {
            _selectedProcessIndex = value;
            OnPropertyChanged(nameof(_selectedProcessIndex));
            OnPropertyChanged(nameof(SelectedProcessIndex));
        }
    }

    /// <summary>
    /// Creates a ViewModel for the DataTableSelectorPage.
    /// </summary>
    /// <param name="DataTablePath"></param>
    /// <param name="PageTitle"></param>
    /// <param name="Department"></param>
    /// <param name="IsProcessAssigned">Indicates whether the Selector Page has been assigned a Process (True by default).</param>
    /// <exception cref="ArgumentException"></exception>
    public DataTableSelectorViewModel(string DataTablePath, string PageTitle, string Department, bool IsProcessAssigned = true) : base(DataTablePath, PageTitle, IsProcessAssigned) {
        // try to match the Department passed with a defined Department Title
        Department Dept;
        try {
            Dept = ProcessData.GetIndividualDepartment(Department);
        // there was no match found for the passed Department Title
        } catch (Exception _ex) {
            throw new ArgumentException(_ex.Message);
        }
        // configure Processes list to only contain the necessary Lines
        _processes = _processes.Where(x => Dept.Lines.Contains(x.Line)).ToList();
    }

    /// <summary>
    /// Updates the Table and Data properties to consume data from a newly-selected Process Database Table.
    /// </summary>
    /// <param name="PageProcessPicker">The Process Picker control.</param>
    /// <returns></returns>
    public async Task UpdatePageProcess(Picker PageProcessPicker) {
        // elicit the record type of this page using the current DataTable and set the path accordingly
        Type RecordType = Table!.RecordType;
        string Path = "\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables";
        if (RecordType.Equals(typeof(PrintRecord))) {
            // page is displaying printing data
            Path = $"{Path}\\prints";
        } else {
            // page is displaying scanning data
            Path = $"{Path}\\scans";
        }
        // get the Process currently selected in the ProcessPicker control
        SelectedProcessIndex = PageProcessPicker.SelectedIndex;
        Process SelectedProcess = (Process)PageProcessPicker.ItemsSource[SelectedProcessIndex]!;
        // update the Page's DataTable to consume data from the newly selected Process Database Table
        Path = $"{Path}\\{SelectedProcess.FullName}.txt";
        Table = new DataTable(Path);
        // update the Page's Data
        Data = await Table.GetRecordsAsync();
    }
}
using LotCoMClient.Models.Datasources;
using LotCoMClient.Models.Services;

namespace LotCoMClient.ViewModels;

/// <summary>
/// ViewModel (ViewModel Layer) controlling the logic of the DataTableSelectorPage View class.
/// Interacts with the Model Layer to invoke business logic and retrieve data.
/// </summary>
public partial class DataTableSelectorViewModel : DataTableViewModel 
{
    private const string DataTablesPathBase = "\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables";

    /// <summary>
    /// Allows access to the Process Data source file.
    /// </summary>
    private readonly ProcessData ProcessData = new ProcessData();

    private List<Process> _processes = [];
    /// <summary>
    /// Serves the selectable Processes for this Page.
    /// </summary>
    public List<Process> Processes 
    {
        get {return _processes;}
        private set 
        {
            _processes = value;
            OnPropertyChanged(nameof(_processes));
            OnPropertyChanged(nameof(Processes));
        }
    }

    private int _selectedProcessIndex = -1;
    /// <summary>
    /// Serves the currently selected index of the ProcessPicker Control.
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

    private bool _isProcessAssigned;
    /// <summary>
    /// Serves the boolean condition of Process assignment for this Data Table Page.
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

    /// <summary>
    /// Creates a ViewModel for the DataTableSelectorPage.
    /// </summary>
    /// <param name="DataTablePath"></param>
    /// <param name="PageTitle"></param>
    /// <param name="Department">A Department Title to assign as the Department of this Page.</param>
    /// <param name="RecordType">The subclass of DataRecord this Page is meant to display (PrintRecord || ScanRecord).</param>
    /// <param name="IsProcessAssigned">Indicates whether the Selector Page has been assigned a Process (True by default).</param>
    /// <exception cref="ArgumentException"></exception>
    public DataTableSelectorViewModel(string DataTablePath, string PageTitle, string Department, Type RecordType, bool IsProcessAssigned = true) : base(DataTablePath, PageTitle, RecordType, IsProcessAssigned) 
    {
        // try to match the Department passed with a defined Department Title
        try 
        {
            PageDepartment = ProcessData.GetIndividualDepartment(Department);
        // there was no match found for the passed Department Title
        } 
        catch (Exception _ex) 
        {
            throw new ArgumentException(_ex.Message);
        }
        // configure Processes list to only contain the necessary Lines
        Processes = ProcessData
            .GetAllProcesses()
            .Where(x => PageDepartment.Lines
            .Contains(x.Line))
            .ToList();
        // configure the IsProcessAssigned property
        this.IsProcessAssigned = IsProcessAssigned;
    }

    /// <summary>
    /// Updates the Table and Data properties to consume data from a newly-selected Process Database Table.
    /// </summary>
    /// <param name="PageProcessPicker">The Process Picker control.</param>
    /// <returns></returns>
    public void UpdatePageProcess(Picker PageProcessPicker) 
    {
        // use the record type of this page to set the path accordingly
        string FullPath;
        if (RecordType.Equals(typeof(PrintRecord))) 
        {
            // page is displaying printing data
            FullPath = $"{DataTablesPathBase}\\prints";
        } 
        else 
        {
            // page is displaying scanning data
            FullPath = $"{DataTablesPathBase}\\scans";
        }
        // get the Process currently selected in the ProcessPicker control
        SelectedProcessIndex = PageProcessPicker.SelectedIndex;
        Process SelectedProcess = (Process)PageProcessPicker.ItemsSource[SelectedProcessIndex]!;
        // update the Page's DataTable to consume data from the newly selected Process Database Table
        Table = new DataTable($"{FullPath}\\{SelectedProcess.FullName}.txt");
        // update the Page's Data
        Data = new NotifyTaskCompletion<List<DataRecord>>(Table.RequestRecords());
        IsProcessAssigned = true;
    }
}
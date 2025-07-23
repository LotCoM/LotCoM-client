using LotComClient.Models.Datasources;
using LotComClient.Models.Services;

namespace LotComClient.ViewModels;

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
            Options.Department = ProcessData.GetIndividualDepartment(Department);
        // there was no match found for the passed Department Title
        } 
        catch (Exception _ex) 
        {
            throw new ArgumentException(_ex.Message);
        }
        // configure Processes list to only contain the necessary Lines
        Options.DepartmentProcesses = ProcessData
            .GetAllProcesses()
            .Where(x => Options.Department.Lines
            .Contains(x.Line))
            .ToList();
        // configure the IsProcessAssigned property
        Options.IsProcessAssigned = IsProcessAssigned;
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
        if (Options.RecordType.Equals(typeof(PrintRecord))) 
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
        Options.SelectedProcessIndex = PageProcessPicker.SelectedIndex;
        Process SelectedProcess = (Process)PageProcessPicker.ItemsSource[Options.SelectedProcessIndex]!;
        // update the Page's DataTable to consume data from the newly selected Process Database Table
        Table = new DataTable($"{FullPath}\\{SelectedProcess.FullName}.txt");
        // update the Page's Data
        SetNewPage(new NotifyTaskCompletion<Models.Datasources.Page>(Table.RequestPage(0, Options.PageLength)));
        Options.IsProcessAssigned = true;
    }
}
using LotCoMClient.Models.Datasources;

namespace LotCoMClient.ViewModels;

/// <summary>
/// ViewModel (ViewModel Layer) controlling the logic of the DataTableSelectorPage View class.
/// Interacts with the Model Layer to invoke business logic and retrieve data.
/// </summary>
public partial class DataTableSelectorViewModel(string DataTablePath, string PageTitle) : DataTableViewModel(DataTablePath, PageTitle) {
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
}
using LotCoMClient.Models.Services;

namespace LotCoMClient.Models.Datasources;

public class DataTableRecordsState 
{
    /// <summary>
    /// List of DataRecords that is unmodified since the last ReadAsync() call.
    /// </summary>
    public NotifyTaskCompletion<List<DataRecord>>? LastRead;

    /// <summary>
    /// An unprotected List of DataRecords that contains the current state of the DataRecords List.
    /// Can be Sorted and Filtered using the corresponding DataTable methods.
    /// </summary>
    public NotifyTaskCompletion<List<DataRecord>>? Current;

    /// <summary>
    /// Creates a DataTableRecordsState to track a DataTable object's DataRecord states.
    /// </summary>
    public DataTableRecordsState() 
    {
    }

    /// <summary>
    /// Updates the LastRead DataRecord State.
    /// </summary>
    /// <param name="LastRead"></param>
    public void SetLastRead(NotifyTaskCompletion<List<DataRecord>> LastRead) 
    {
        this.LastRead = LastRead;
    }

    /// <summary>
    /// Updates the Current DataRecord State.
    /// </summary>
    /// <param name="LastRead"></param>
    public void SetCurrent(NotifyTaskCompletion<List<DataRecord>> Current) 
    {
        this.Current = Current;
    }

}
namespace LotCoMClient.Models.Datasources;

public class DataTableRecordsState 
{
    /// <summary>
    /// List of DataRecords that is unmodified since the last ReadAsync() call.
    /// </summary>
    public List<DataRecord>? LastRead;

    /// <summary>
    /// An unprotected List of DataRecords that contains the current state of the DataRecords List.
    /// Can be Sorted and Filtered using the corresponding DataTable methods.
    /// </summary>
    public List<DataRecord>? Current;

    /// <summary>
    /// Creates a DataTableRecordsState to track a DataTable object's DataRecord states.
    /// </summary>
    public DataTableRecordsState() 
    {
    }
    
}
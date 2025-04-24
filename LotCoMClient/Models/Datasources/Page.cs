namespace LotCoMClient.Models.Datasources;

public class Page(int PageLength)
{
    /// <summary>
    /// The maximum number of Lines/DataRecords this Page can hold.
    /// </summary>
    public int MaxLength = PageLength;

    /// <summary>
    /// Returns the current number of DataRecords contained in this Page.
    /// </summary>
    public int Count => DataRecords.Count;

    /// <summary>
    /// Contains an unparsed list of Lines assigned to this Page.
    /// </summary>
    public List<string> Lines = [];

    /// <summary>
    /// Contains a parsed list of DataRecord objects assigned to this Page.
    /// </summary>
    public List<DataRecord> DataRecords = [];
}
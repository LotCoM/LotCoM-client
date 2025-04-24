namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Creates a set of Lines and/or DataRecords that can be displayed in a ListView.
/// </summary>
/// <param name="PageLength">The maximum number of Lines/DataRecords this Page can hold.</param>
public class Page(int PageLength, Type RecordType)
{
    /// <summary>
    /// Sets the type of DataRecord that this Page can hold.
    /// </summary>
    private Type RecordType = RecordType;

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

    /// <summary>
    /// Parses a DataRecord of the Page's RecordType from CSVLine.
    /// </summary>
    /// <param name="Parser">A RecordParser object.</param>
    /// <param name="CSVLine">A CSV-formatted string.</param>
    /// <returns></returns>
    private async Task<DataRecord> ParseRecordAsync(RecordParser Parser, string CSVLine)
    {
        // attempt to parse the proper type of DataRecord from the CSV Line
        DataRecord ParsedRecord;
        if (RecordType == typeof(PrintRecord)) 
        {
            ParsedRecord = await Parser.ParsePrintRecordFromCSVAsync(CSVLine);
        // parse a ScanRecord
        } 
        else 
        {
            ParsedRecord = await Parser.ParseScanRecordFromCSVAsync(CSVLine);
        }
        // return the parsed DataRecord
        return ParsedRecord;
    }

    /// <summary>
    /// Parses all of the Lines in Page.Lines into DataRecords of Page.RecordType.
    /// Stores the DataRecords in Page.DataRecords.
    /// </summary>
    public async Task GenerateDataRecords() 
    {
        // create a new RecordParser instance
        RecordParser Parser = new RecordParser();
        // parse a DataRecord from every Line in Lines and save it in DataRecords
        IEnumerable<Task<DataRecord>>? ParseTasks = Lines
            .Select(x => ParseRecordAsync(Parser, x));
        DataRecord[]? ParseResults = await Task.WhenAll(ParseTasks);
        // confirm that the Parse was successful and add the Parsed DataRecords to DataRecords
        if (ParseResults is null) 
        {
            DataRecords = [];
        }
        DataRecords = ParseResults!.ToList();
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMClient.Models.Exceptions;
using LotCoMClient.Models.Options;
using System.Linq.Dynamic;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides controlled access and manipulation of database tables in the LotCoM Database.
/// </summary>
public partial class DataTable : ObservableObject 
{
    /// <summary>
    /// Allows parsing of CSV Lines into DataRecord objects.
    /// </summary>
    private readonly RecordParser Parser = new RecordParser();

    private string _path = "";
    /// <summary>
    /// The Path of the database table file in the LotCoM database filing system.
    /// </summary>
    public string Path 
    {
        get {return _path;}
        private set {_path = value;}
    }

    private Type _recordType = typeof(DataRecord);
    /// <summary>
    /// The type of Data Record the table file contains (PrintRecord || ScanRecord).
    /// </summary>
    public Type RecordType 
    {
        get {return _recordType;}
        private set {_recordType = value;}
    }

    private DataTableRecordsState _recordsState = new DataTableRecordsState();
    /// <summary>
    /// Holds the Table's current DataRecords State.
    /// </summary>
    public DataTableRecordsState RecordsState {
        get {return _recordsState;}
        set {_recordsState = value;}
    }

    /// <summary>
    /// Flag to use to check if the DataTable has performed a ReadAsync call or not.
    /// </summary>
    public bool IsFirstRead = false;

    private List<string> _headers = [];
    /// <summary>
    /// Holds the Headers (keys) for each data field that the DataRecords in this Table contain.
    /// </summary>
    public List<string> Headers 
    {
        get {return _headers;}
        private set {_headers = value;}
    }

    [ObservableProperty]
    /// <summary>
    /// Observable property exposing the Name of the Process producing the Records in this Table.
    /// </summary>
    public partial string TableProcess {get; set;}

    /// <summary>
    /// Parses a DataRecord of the DataTable's RecordType from CSVLine.
    /// </summary>
    /// <param name="CSVLine"></param>
    /// <exception cref="RecordParseException"></exception>
    /// <returns>A DataRecord object.</returns>
    private async Task<DataRecord> ParseRecordAsync(string CSVLine) 
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
    /// Parses a bulk string into Lines that can be parsed.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns>A List of strings.</returns>
    private async Task<List<string>> ParseLinesAsync(string Text) {
        return await Task.Run(() => {
            // separate the read text into record lines (split by newline character)
            List<string> RecordLines = Text
                .Split("\n")
                .ToList();
            // remove the first entry and save it as the headers property
            Headers = RecordLines[0]
                .Split(",")
                .ToList();
            RecordLines.RemoveAt(0);
            // remove any empty lines
            RecordLines = RecordLines
                .Where(x => !x.Equals(""))
                .ToList();
            return RecordLines;
        });
    }

    /// <summary>
    /// Asynchronously opens, reads, and formats the text in DataTable.Path as a list of DataRecords.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="RecordParseException"></exception>
    /// <returns>A List of DataRecords.</returns>
    private async Task<List<DataRecord>> ReadAsync() 
    {
        // read the Database Table at the Path property
        string Text;
        try 
        {
            Text = await File.ReadAllTextAsync(Path);
        } 
        catch (Exception _ex) 
        {
            throw new OperationCanceledException($"Failed to read the Database file: '{Path}' due to the following exception:\n{_ex.Message}.");
        }
        // parse the text into individual DataRecord objects as a batch of async Tasks
        List<string> RecordLines = await ParseLinesAsync(Text);
        IEnumerable<Task<DataRecord>>? Tasks = RecordLines.Select(ParseRecordAsync);
        DataRecord[]? ParseResult = await Task.WhenAll(Tasks);
        if (ParseResult == null) 
        {
            return [];
        }
        else 
        {
            return ParseResult.ToList();
        }
    }

    /// <summary>
    /// Asynchronously opens and overwrites the data in DataTable._path with the current list of DataRecords in DataTable._records.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of DataRecords.</returns>
    private async Task SaveAsync(List<DataRecord> Records) 
    {
        // format the passed DataRecords as single string separated by newlines on a new CPU thread
        if (Records != null) 
        {
            string Text = await Task.Run(() => 
            {
                string Formatted = "";
                foreach (DataRecord _record in Records) 
                {
                    Formatted = $"{Formatted}{_record.ToCSV()}\n";
                }
                // return the formatted single string
                return Formatted;
            });
            // asynchronously write the single string as text to the Database Table file at _path
            await File.WriteAllTextAsync(Path, Text);
        } 
        else 
        {
            throw new OperationCanceledException("Cannot save null to the Database Table file.");
        }
    }

    /// <summary>
    /// Sorts the Table's Records list using SortingProperty as the sort.
    /// Order can be either 0 or 1, where 0 indicates ascending order and 1 indicates descending.
    /// Does NOT overwrite with the sorted list.
    /// </summary>
    /// <param name="SortingProperty"></param>
    /// <param name="SortOrder"></param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of DataRecords sorted using the Property and Order.</returns>
    private async Task<List<DataRecord>> SortRecordsAsync(string SortingProperty, int SortOrder) 
    {
        if (RecordsState.Current == null) 
        {
            throw new OperationCanceledException();
        }
        // perform the sort algorithm on a new CPU thread
        return await Task.Run(() => 
        {
            // use LINQ dynamic to sort using the property selected in the sorting field picker
            List<DataRecord> SortedData = RecordsState.Current
                .AsQueryable()
                .OrderBy(SortingProperty)
                .ToList();
            // invert the order (ascending by default) if descending sort was selected
            if (SortOrder == 1) 
            {
                SortedData.Reverse();
            }
            return SortedData;
        });
    }

    /// <summary>
    /// Searches each DataRecord for a match in ANY field (as a continuous string). 
    /// </summary>
    /// <param name="SearchTerm">The term to match.</param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of DataRecords that were match hits for the search.</returns>
    private async Task<List<DataRecord>> SearchAllFieldsAsync(string SearchTerm) 
    {
        if (RecordsState.LastRead == null) 
        {
            throw new OperationCanceledException();
        }
        // perform the search algorithm on a new CPU thread
        return await Task.Run(() => 
        {
            // convert each DataRecord in RecordsState.LastRead to a CSV Line and check for a hit
            List<DataRecord> SearchHits = [];
            foreach (DataRecord _record in RecordsState.LastRead) 
            {
                string _recordString = _record.ToCSV();
                if (_recordString.Contains(SearchTerm)) 
                {
                    SearchHits.Add(_record);
                }
            }
            return SearchHits;
        });
    }

    /// <summary>
    /// Searches each DataRecord for a match in PropertyName field.
    /// </summary>
    /// <param name="SearchTerm">The term to match.</param>
    /// <param name="PropertyName">The name of the Property to search in.</param>
    /// <returns>A List of DataRecords that were match hits for the search.</returns>
    private async Task<List<DataRecord>> SearchSingleFieldAsync(string SearchTerm, string PropertyName) 
    {
        if (RecordsState.LastRead == null) 
        {
            throw new OperationCanceledException();
        }
        return await Task.Run(() => 
        {
            // convert each DataRecord in _records to a CSV Line and check for a hit
            List<DataRecord> SearchHits = [];
            SearchHits = RecordsState.LastRead.Where(x => x.GetType()!
                .GetProperty(PropertyName)!
                .GetValue(x)!
                .ToString()!
                .Contains(SearchTerm))
                .ToList();
            return SearchHits;
        });
    }

    /// <summary>
    /// Runs one of the two Search Algorithms. 
    /// The chosen Algorithm depends on the value of PropertyName.
    /// </summary>
    /// <param name="SearchTerm"></param>
    /// <param name="PropertyName"></param>
    /// <returns></returns>
    private async Task<List<DataRecord>> SearchAsync(string SearchTerm, string PropertyName) 
    {
        return await Task.Run(async () => 
        {
            List<DataRecord> Hits;
            // search in all fields of each DataRecord
            if (PropertyName.Equals("All")) 
            {
                Hits = await SearchAllFieldsAsync(SearchTerm);
            // search in a singular field of each DataRecord
            } 
            else 
            {
                Hits = await SearchSingleFieldAsync(SearchTerm, PropertyName);
            }
            return Hits;
        });
    }

    /// <summary>
    /// Constructs a new DataTable that provides controlled access and manipulation of data in the Database Table located at DataTablePath.
    /// </summary>
    /// <param name="DataTablePath">A full file path to a Database Table file in the LotCoM database.</param>
    public DataTable(string DataTablePath) 
    {
        Path = DataTablePath;
        // calculate the record type from the path string
        if (Path.Contains("data_tables\\prints")) 
        {
            RecordType = typeof(PrintRecord);
        } 
        else if (Path.Contains("data_tables\\scans")) 
        {
            RecordType = typeof(ScanRecord);
        // the path passed isn't a valid Database Table path; throw an exception
        } 
        else 
        {
            throw new ArgumentException($"Could not create a DataTable object from the file at {Path}.");
        }
        // set the Table's Process using the filename
        TableProcess = Path
            .Split("\\")[^1]
            .Replace(".txt", "");
    }

    /// <summary>
    /// Retrieves either the Current or LastRead List of DataRecords in the Table.
    /// </summary>
    /// <exception cref="SystemException"></exception>
    /// <returns>A List of DataRecords.</returns>
    public async Task<List<DataRecord>> RequestRecords() 
    {
        return await Task.Run(() => {
            // return the DataRecords stored in runtime
            if (RecordsState.Current != null)
            {
                return RecordsState.Current;
            }
            else if (RecordsState.LastRead != null)
            {
                return RecordsState.LastRead;
            }
            else 
            {
                return [];
            }
        });
    }

    /// <summary>
    /// Refreshes the Database Table in runtime.
    /// </summary>
    /// <returns>A list of DataRecords currently in the Database Table.</returns>
    /// <exception cref="SystemException"></exception>
    public async Task<List<DataRecord>> ReadRecordsAsync() 
    {
        // read the DataRecord and return the NotifyTaskCompletion object holding the promised list
        try 
        {
            RecordsState.LastRead = await ReadAsync();
            RecordsState.Current = RecordsState.LastRead;
            return RecordsState.LastRead;
        } 
        catch (Exception _ex) 
        {
            throw new SystemException($"Could not complete the read request due to the following exception:\n {_ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronously saves DataRecords to DataTable.Path and updates DataTable.RecordsState.LastRead in runtime.
    /// </summary>
    /// <param name="Records">A List of DataRecords to write to DataTable._path.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    public async Task SaveRecordsAsync(List<DataRecord> Records) 
    {
        // save the passed records to the Database table file
        try 
        {
            await SaveAsync(Records);
        } 
        catch (Exception _ex) 
        {
            throw new FileLoadException($"Failed to save the DataTable to the file '{_path}' due to the following access error:\n{_ex.Message}");
        }
        // update the RecordsState property
        RecordsState.LastRead = await ReadAsync();
        RecordsState.Current = RecordsState.LastRead;
    }

    /// <summary>
    /// Sorts the Table's Records list using SortingProperty as the sort.
    /// Order can be either 0 or 1, where 0 indicates ascending order and 1 indicates descending.
    /// </summary>
    /// <param name="SortingProperty">A Property name applicable to the DataRecord class.</param>
    /// <param name="Order">0 (ascending) or 1 (descending).</param>
    /// <returns></returns>
    public async Task<List<DataRecord>> RequestSort(string SortingProperty, int Order) 
    {
        RecordsState.Current = await SortRecordsAsync(SortingProperty, Order);
        return RecordsState.Current;
    }

    /// <summary>
    /// Performs a search on the current data in _records. 
    /// If All passed as PropertyName, checks for matches in every field of the Data Record.
    /// Otherwise, searches for match hits in the singular field passed as PropertyName.
    /// </summary>
    /// <param name="SearchTerm">The term to match.</param>
    /// <param name="PropertyName">The name of the Property to search in.</param>
    /// <returns>A List of DataRecords that the matching algorithm hits.</returns>
    public async Task<List<DataRecord>> RequestSearch(string SearchTerm, string PropertyName) 
    {
        // perform the search algorithm on a new CPU thread
        RecordsState.Current = await SearchAsync(SearchTerm, PropertyName);
        return RecordsState.Current;
    }
}
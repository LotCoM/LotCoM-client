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

    /// <summary>
    /// Holds a List of strings resulting from the latest file read.
    /// </summary>
    private List<string> LastReadLines = [];

    /// <summary>
    /// Holds a List of strings that are matching results of the latest Search algorithm.
    /// </summary>
    private List<string> SearchResultLines = [];

    /// <summary>
    /// Holds a List of Pages created from this Table's data.
    /// </summary>
    private List<Page> Pages = [];

    /// <summary>
    /// Holds a custom List of Pages created from the latest Search algorithm.
    /// </summary>
    private List<Page> SearchPages = [];

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

    private List<string> _headers = [];
    /// <summary>
    /// Holds the Headers (keys) for each data field that the DataRecords in this Table contain.
    /// </summary>
    public List<string> Headers 
    {
        get {return _headers;}
        private set {_headers = value;}
    }

    private Process? _process = null;
    /// <summary>
    /// The Process producing the Records in this Table.
    /// </summary>
    public Process? Process 
    {
        get {return _process;}
        private set {_process = value;}
    }

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
    /// Reads the data file and formats the text as a List of strings that can later be parsed into DataRecord objects.
    /// Stores the List in Table.LastReadLines.
    /// </summary>
    /// <remarks>
    /// Does not perform any formatting.
    /// </remarks>
    /// <returns>A List of Lines as strings.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    private async Task<List<string>> ReadLinesAsync()
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
        // split the text into a list of Line strings and update the cached Lines list
        LastReadLines = await ParseLinesAsync(Text);
        // reverse list to show newest Lines first
        LastReadLines.Reverse();
        return LastReadLines;
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
        await ReadLinesAsync();
        // parse the text into individual DataRecord objects as a batch of async Tasks
        IEnumerable<Task<DataRecord>>? Tasks = LastReadLines.Select(ParseRecordAsync);
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
    /// Creates a List of Page objects from the Lines in LastReadLines and stores them in Table.Pages.
    /// </summary>
    /// <param name="PageLength">Specifies the maximum number of Lines to include in each Page.</param>
    /// <returns></returns>
    private async Task<List<Page>> PaginateAsync(int PageLength)
    {
        // confirm that the Table has Lines stored in the LastReadProperty
        if (LastReadLines.Count < 1)
        {
            await ReadLinesAsync();
        }
        // create pages of PageLength Lines, starting with the newest lines
        int TakenLines = 0;
        while (TakenLines < LastReadLines.Count)
        {
            // take the first PageLength + TakenLines elements from LastReadLines to create a Page
            Page _page = new Page(PageLength);
            _page.Lines = LastReadLines
                .Skip(TakenLines)
                .Take(PageLength)
                .ToList();
            // increment the amount of lines taken and add the Page to the Table
            TakenLines += PageLength;
            Pages.Add(_page);
        }
        return Pages;
    }

    /// <summary>
    /// Parses DataRecords from every Line in a Page and saves those DataRecords in the Page's Page.DataRecords property.
    /// </summary>
    /// <param name="PageNumber">The Page in Table.Pages to Parse 
    /// (NOTE: Table.Pages is 0-oriented, so Page Numbers must be 1 less than the actual page number.)
    /// </param>
    /// <returns></returns>
    private async Task<Page> ParsePageAsync(int PageNumber)
    {
        // confirm that the Page hasn't already been parsed out
        if (Pages[PageNumber].DataRecords.Count > 0) 
        {
            return Pages[PageNumber];
        }
        // parse a DataRecord from every Line in Page.Lines and save it in Page.DataRecords
        IEnumerable<Task<DataRecord>>? ParseTasks = Pages[PageNumber]
            .Lines
            .Select(ParseRecordAsync);
        DataRecord[]? ParseResults = await Task.WhenAll(ParseTasks);
        // confirm that the Parse was successful and add the Parsed DataRecords to Page.DataRecords
        if (ParseResults is null) 
        {
            Pages[PageNumber].DataRecords = [];
        }
        Pages[PageNumber].DataRecords = ParseResults!.ToList();
        // return the updated Page object
        return Pages[PageNumber];
    }

    /// <summary>
    /// Creates a List of Page objects from the Lines in SearchResultLines and stores them in Table.SearchPages.
    /// </summary>
    /// <param name="PageLength">Specifies the maximum number of Lines to include in each Page.</param>
    /// <returns></returns>
    private async Task<List<Page>> PaginateSearchResultsAsync(int PageLength)
    {
        return await Task.Run(() => 
        {
            // confirm that the Table has Lines stored in the SearchResultLines
            if (SearchResultLines.Count < 1)
            {
                SearchPages = [];
                return SearchPages;
            }
            // create pages of PageLength Lines, starting with the newest lines
            int TakenLines = 0;
            while (TakenLines < SearchResultLines.Count)
            {
                // take the first PageLength + TakenLines elements from SearchResultLines to create a Page
                Page _page = new Page(PageLength);
                _page.Lines = SearchResultLines
                    .Skip(TakenLines)
                    .Take(PageLength)
                    .ToList();
                // increment the amount of lines taken and add the Page to the Table
                TakenLines += PageLength;
                SearchPages.Add(_page);
            }
            return SearchPages;
        });
    }

    /// <summary>
    /// Parses DataRecords from every Line in a Page and saves those DataRecords in the Page's Page.DataRecords property.
    /// </summary>
    /// <param name="PageNumber">The Page in Table.SearchPages to Parse 
    /// (NOTE: Table.SearchPages is 0-oriented, so Page Numbers must be 1 less than the actual page number.)
    /// </param>
    /// <returns></returns>
    private async Task<Page> ParseSearchResultsPageAsync(int PageNumber)
    {
        // confirm that the Page hasn't already been parsed out
        if (SearchPages[PageNumber].DataRecords.Count > 0) 
        {
            return SearchPages[PageNumber];
        }
        // parse a DataRecord from every Line in Page.Lines and save it in Page.DataRecords
        IEnumerable<Task<DataRecord>>? ParseTasks = SearchPages[PageNumber]
            .Lines
            .Select(ParseRecordAsync);
        DataRecord[]? ParseResults = await Task.WhenAll(ParseTasks);
        // confirm that the Parse was successful and add the Parsed DataRecords to Page.DataRecords
        if (ParseResults is null) 
        {
            SearchPages[PageNumber].DataRecords = [];
        }
        SearchPages[PageNumber].DataRecords = ParseResults!.ToList();
        // return the updated Page object
        return SearchPages[PageNumber];
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
    /// Searches each Line in LastReadLines for a match in ANY field (as a continuous CSV string). 
    /// </summary>
    /// <remarks>
    /// Updates LastReadLines to contain the matching Lines.
    /// </remarks>
    /// <param name="SearchTerm">The term to match.</param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of strings that were match hits for the search.</returns>
    private async Task<List<string>> SearchAllFieldsAsync(string SearchTerm) 
    {
        // confirm that there are Lines to search through
        if (LastReadLines is null) 
        {
            try
            {
                await ReadLinesAsync();
            }
            catch
            {
                throw new OperationCanceledException();
            }
        }
        SearchResultLines = LastReadLines!;
        // return all hits for the search term
        SearchResultLines = SearchResultLines!
            .Where(x => x
            .Contains(SearchTerm))
            .ToList();
        return SearchResultLines;
    }

    /// <summary>
    /// Searches each Line in LastReadLines for a match in PropertyName field.
    /// </summary>
    /// <remarks>
    /// Updates LastReadLines to contain the matching Lines.
    /// </remarks>
    /// <param name="SearchTerm">The term to match.</param>
    /// <param name="PropertyName">The name of the Property to search in.</param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of strings that were match hits for the search.</returns>
    private async Task<List<string>> SearchSingleFieldAsync(string SearchTerm, string PropertyName) 
    {
        // first find all Lines with hits in any field
        SearchResultLines = await SearchAllFieldsAsync(SearchTerm);
        // convert all Hits into DataRecords
        IEnumerable<Task<DataRecord>>? ParseTasks = SearchResultLines!
            .Select(ParseRecordAsync);
        DataRecord[]? ParseResults = await Task.WhenAll(ParseTasks);
        // confirm that the Parse was successful
        if (ParseResults is null) 
        {
            return [];
        }
        // now find all DataRecords with a hit in the specific field requested
        List<DataRecord> RecordHits = ParseResults
            .Where(x => x.GetType()!
            .GetProperty(PropertyName)!
            .GetValue(x)!
            .ToString()!
            .Contains(SearchTerm))
            .ToList();
        // convert those DataRecords back to strings and return
        SearchResultLines = RecordHits
            .Select(x => x.ToCSV())
            .ToList();
        return SearchResultLines;
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
            throw new ArgumentException($"Could not create a DataTable object from the file at '{Path}'.");
        }
        // set the Table's Process using the filename
        string ProcessName = Path
            .Split("\\")[^1]
            .Replace(".txt", "");
        try
        {
            Process = new ProcessData().GetIndividualProcess(ProcessName);
        }
        catch
        {
            throw new ArgumentException($"Could not create a DataTable object from the file at '{Path}' because the Process '{ProcessName}' is not defined.");
        }
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
    /// Confirms that there are Pages in the Table and that they are of the correct Length.
    /// Checks if the requested Page has been parsed into DataRecords and does so if not.
    /// </summary>
    /// <param name="PageNumber">The Page in Table.Pages to Parse 
    /// (NOTE: Table.Pages is 0-oriented, so Page Numbers must be 1 less than the actual page number.)
    /// </param>
    /// <param name="PageLength">Specifies the maximum number of Lines to include in each Page.</param>
    /// <returns>A Page object with PageLength DataRecords ready to be displayed.</returns>
    public async Task<Page> RequestPage(int PageNumber, int PageLength) 
    {
        // confirm that there are Pages of the correct size available
        if (Pages.Count < 1 || Pages[0].MaxLength != PageLength)
        {
            await PaginateAsync(PageLength);
        }
        // parse DataRecords out of the requested Page of Lines
        return await ParsePageAsync(PageNumber);
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
    /// Performs a search on all of the current records in the Table.
    /// If All passed as PropertyName, checks for matches in every field of the Data Record.
    /// Otherwise, searches for match hits in the singular field passed as PropertyName.
    /// Builds a new Page set from the search results.
    /// </summary>
    /// <param name="SearchTerm">The term to match.</param>
    /// <param name="PropertyName">The name of the Property to search in.</param>
    /// <returns>A List of DataRecords that the matching algorithm hits.</returns>
    private async Task<Page> SearchAsync(string SearchTerm, string PropertyName, int PageLength) 
    {
        // search in all fields of each DataRecord
        if (PropertyName.Equals("All")) 
        {
            await SearchAllFieldsAsync(SearchTerm);
        // search in a singular field of each DataRecord
        } 
        else 
        {
            await SearchSingleFieldAsync(SearchTerm, PropertyName);
        }
        // paginate the new LastReadLines value and Parse the first Page in that new set
        await PaginateSearchResultsAsync(PageLength);
        await ParseSearchResultsPageAsync(0);
        return SearchPages[0];
    }
}
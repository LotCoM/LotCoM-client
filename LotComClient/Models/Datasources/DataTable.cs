using CommunityToolkit.Mvvm.ComponentModel;
using LotComClient.Models.Exceptions;

namespace LotComClient.Models.Datasources;

/// <summary>
/// Provides controlled access and manipulation of database tables in the LotCom Database.
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
    /// Holds a List of Pages created from this Table's unfiltered data.
    /// </summary>
    private PageSet BasePages;

    /// <summary>
    /// Holds a custom List of Pages created from the latest Search algorithm.
    /// </summary>
    private PageSet SearchPages;

    private string _path = "";
    /// <summary>
    /// The Path of the database table file in the LotCom database filing system.
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

    private PageSet _activePageSet = new PageSet(typeof(DataRecord));
    /// <summary>
    /// Controls this Table's active PageSet, which is the PageSet the Table displays Pages from.
    /// </summary>
    public PageSet ActivePageSet
    {
        get {return _activePageSet;}
        private set 
        {
            _activePageSet = value;
            OnPropertyChanged(nameof(_activePageSet));
            OnPropertyChanged(nameof(ActivePageSet));
        }
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
    private List<string> ParseLines(string Text) {
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
    }

    /// <summary>
    /// Asynchronously parses a bulk string into Lines that can be parsed.
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
    private List<string> ReadLines()
    {
        // read the Database Table at the Path property
        string Text;
        try 
        {
            Text = File.ReadAllText(Path);
        } 
        catch (Exception _ex) 
        {
            throw new FileLoadException($"Failed to read the Database file: '{Path}' due to the following exception:\n{_ex.Message}.");
        }
        // split the text into a list of Line strings and update the cached Lines list
        LastReadLines = ParseLines(Text);
        // reverse list to show newest Lines first
        LastReadLines.Reverse();
        return LastReadLines;
    }

    /// <summary>
    /// Asynchronously reads the data file and formats the text as a List of strings that can later be parsed into DataRecord objects.
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
    /// <param name="DataTablePath">A full file path to a Database Table file in the LotCom database.</param>
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
        // read the file synchronously (once) and set up the Table's PageSets
        ReadLines();
        BasePages = new PageSet(LastReadLines, RecordType);
        SearchPages = new PageSet(RecordType);
        ActivePageSet = BasePages;
    }

    /// <summary>
    /// Jumps to the first Page in the Table's current Pages (the newest Records).
    /// </summary>
    public async Task<Page> GoToFirstPage()
    {
        return await ActivePageSet.GoToFirstPage();
    }

    /// <summary>
    /// Goes to the previous Page in the Table's current Pages (if one exists).
    /// </summary>
    public async Task<Page> GoToPreviousPage()
    {
        return await ActivePageSet.GoToPreviousPage();
    }

    /// <summary>
    /// Jumps to the last Page in the Table's current Pages (the oldest Records).
    /// </summary>
    public async Task<Page> GoToLastPage()
    {
        return await ActivePageSet.GoToLastPage();
    }

    /// <summary>
    /// Goes to the next Page in the Table's current Pages (if one exists).
    /// </summary>
    public async Task<Page> GoToNextPage()
    {
        return await ActivePageSet.GoToNextPage();
    }

    /// <summary>
    /// Swaps the Active Page Set to the last generated SearchPages PageSet.
    /// </summary>
    /// <returns></returns>
    public async Task GoToSearchPages()
    {
        // set the Active Page Set to use Search Pages and set the Active Page to the first page in the set
        ActivePageSet = SearchPages;
        try
        {
            await SearchPages.SetActivePage(0);
        }
        catch
        {
            throw new NullReferenceException("There is no Search Page at the specified index '0'.");
        }
    }

    /// <summary>
    /// Swaps the Active Page Set to the last generated BasePages PageSet.
    /// </summary>
    /// <returns></returns>
    public async Task GoToBasePages()
    {
        // set the Active Page Set to use Base Pages and set the Active Page to the first page in the set
        ActivePageSet = BasePages;
        await BasePages.SetActivePage(0);
    }

    /// <summary>
    /// Creates a new PageSet from the LastReadLines property. Replaces BasePages and SearchPages with this new PageSet.
    /// </summary>
    /// <param name="MaxCount"></param>
    /// <param name="PageLength"></param>
    /// <returns>The currently Active Page of the new PageSet.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<Page> RefreshPages(int MaxCount, int PageLength)
    {
        // confirm that there are Lines to create a PageSet from
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
        // create a new PageSet from LastReadLines using the passed PageLength and MaxCount
        BasePages = new PageSet(LastReadLines!, RecordType, MaxCount, PageLength);
        SearchPages = BasePages;
        ActivePageSet = BasePages;
        return await ActivePageSet.GetActivePage();
    }

    /// <summary>
    /// Confirms that there are Pages in the Table and that they are of the correct Length.
    /// Checks if the requested Page has been parsed into DataRecords and does so if not.
    /// </summary>
    /// <param name="PageNumber">The Page in Table.Pages to Parse 
    /// (NOTE: Table.Pages is 0-oriented, so Page Numbers must be 1 less than the actual page number.)
    /// </param>
    /// <param name="PageLength">Specifies the maximum number of Lines to include in each Page.</param>
    /// <param name="MaxCount">(Optional) Set a limit on the number of Pages allowed in the PageSet.</param>
    /// <returns>A Page object with PageLength DataRecords ready to be displayed.</returns>
    public async Task<Page> RequestPage(int PageNumber, int PageLength, int MaxCount = -1) 
    {
        // confirm that Pages is set to provide Pages of the correct size
        if (ActivePageSet.PageCount < 1 || ActivePageSet.Pages[0].MaxLength != PageLength)
        {
            await RefreshPages(MaxCount, PageLength);
        }
        // get the requested Page
        try
        {
            await ActivePageSet.SetActivePage(PageNumber);
            return await ActivePageSet.GetActivePage();
        }
        catch
        {
            throw new IndexOutOfRangeException();
        }
    }

    /// <summary>
    /// Performs a search on all of the current records in the Table.
    /// If All passed as PropertyName, checks for matches in every field of the Data Record.
    /// Otherwise, searches for match hits in the singular field passed as PropertyName.
    /// Builds a new PageSet from the search results and activates the new SearchPages PageSet.
    /// </summary>
    /// <param name="SearchTerm">The term to match.</param>
    /// <param name="PropertyName">The name of the Property to search in.</param>
    /// <exception cref="NullReferenceException"></exception>
    /// <returns>The ActivePage of the new SearchPages PageSet.</returns>
    public async Task<Page> SearchAsync(string SearchTerm, string PropertyName, int PageLength) 
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
        // create a new PageSet with the new SearchResultLines value and return the first Page in that new set
        if (SearchResultLines.Count < 1)
        {
            throw new NullReferenceException($"There were no Search Results for the query:\n'Term: {SearchTerm}'\n'Field: {PropertyName}'");
        }
        SearchPages = new PageSet(SearchResultLines, RecordType, PageLength: PageLength);
        await GoToSearchPages();
        return await SearchPages.GetActivePage();
    }
}
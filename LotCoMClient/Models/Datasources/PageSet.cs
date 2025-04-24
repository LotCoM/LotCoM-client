namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides a control structure for a set of Page objects that can be used in the same context.
/// </summary>
public class PageSet
{
    /// <summary>
    /// Sets a maximum number of Pages allowed in this PageSet.
    /// </summary>
    private int MaxCount = -1;

    /// <summary>
    /// Sets the type of DataRecord that the Pages in this PageSet can hold.
    /// </summary>
    private Type RecordType;

    private List<Page> _pages = [];
    /// <summary>
    /// The Page objects included in this PageSet.
    /// </summary>
    public List<Page> Pages
    {
        get {return _pages;}
        private set
        {
            _pages = value;
        }
    }

    /// <summary>
    /// Sets the maximum number of Lines allowed in each Page in this PageSet.
    /// </summary>
    public int PageLength = 25;

    /// <summary>
    /// The number of Page objects in this PageSet.
    /// </summary>
    public int Count => Pages.Count;

    /// <summary>
    /// Whether this PageSet can contain more Pages or not.
    /// Always true for unlimited PageSets (MaxCount = -1).
    /// </summary>
    public bool HasSpace
    {
        get 
        {
            if (MaxCount == -1)
            {
                return true;
            }
            return Pages.Count < MaxCount;
        }
    }

    /// <summary>
    /// Creates a List of Page objects from Lines.
    /// </summary>
    /// <param name="Lines">A List of Lines to create Pages from.</param>
    /// <returns></returns>
    private List<Page> Paginate(List<string> Lines)
    {
        // create pages of PageLength Lines, starting with the newest lines
        List<Page> GeneratedPages = [];
        int TakenLines = 0;
        while (TakenLines < Lines.Count)
        {
            // take the first PageLength + TakenLines elements from Lines to create a Page
            Page _page = new Page(PageLength, RecordType);
            _page.Lines = Lines
                .Skip(TakenLines)
                .Take(PageLength)
                .ToList();
            // increment the amount of lines taken and add the Page to the Table
            TakenLines += PageLength;
            GeneratedPages.Add(_page);
        }
        return GeneratedPages;
    }

    /// <summary>
    /// Creates a new, empty PageSet.
    /// </summary>
    /// <param name="MaxCount">(Optional) Constrains the amount of Pages that are allowed in the PageSet.</param>
    /// <param name="PageLength">
    /// (Optional) Sets the maximum number of Lines that are allowed in each Page in the PageSet.
    /// Default is 25 Lines per Page object.
    /// </param>
    public PageSet(Type RecordType, int MaxCount = -1, int PageLength = 25)
    {
        this.MaxCount = MaxCount;
        this.PageLength = PageLength;
        this.RecordType = RecordType;
    }

    /// <summary>
    /// Creates a new PageSet and pre-populates it with Pages created from Lines.
    /// </summary>
    /// <param name="Lines">A List of strings to use as Lines for each Page in the PageSet.</param>
    /// <param name="MaxCount">(Optional) Constrains the amount of Pages that are allowed in the PageSet.</param>
    /// <param name="PageLength">
    /// (Optional) Sets the maximum number of Lines that are allowed in each Page in the PageSet.
    /// Default is 25 Lines per Page object.
    /// </param>
    public PageSet(List<string> Lines, Type RecordType, int MaxCount = -1, int PageLength = 25)
    {
        this.MaxCount = MaxCount;
        this.PageLength = PageLength;
        this.RecordType = RecordType;
        // paginate Lines and set the Pages property
        List<Page> GeneratedPages = Paginate(Lines);
        if (MaxCount != -1)
        {
            Pages = GeneratedPages.Take(MaxCount).ToList();
        }
        else
        {
            Pages = GeneratedPages;
        }
    }

    /// <summary>
    /// Returns a Page with DataRecords.
    /// </summary>
    /// <param name="PageNumber">The index of the requested Page in PageSet.Pages. 0-oriented.</param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <returns></returns>
    public async Task<Page> GetPage(int PageNumber)
    {
        // confirm that a Page exists at the requested Page Number
        if (Count - 1 < PageNumber) 
        {
            throw new IndexOutOfRangeException($"There is no Page at the requested index {PageNumber}.");
        }
        Page RequestedPage = Pages[PageNumber];
        // populate the Page's DataRecords property
        await RequestedPage.GenerateDataRecords();
        return RequestedPage;
    }
}
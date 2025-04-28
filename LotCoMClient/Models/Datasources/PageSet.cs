namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides a control structure for a set of Page objects that can be used in the same context.
/// </summary>
public class PageSet
{
    /// <summary>
    /// Sets a maximum number of Pages allowed in this PageSet.
    /// </summary>
    private readonly int MaxCount = -1;

    /// <summary>
    /// Returns whether or not the Page has a set MaxCount.
    /// </summary>
    private bool IsLimitedSize => MaxCount != -1;

    /// <summary>
    /// Sets the type of DataRecord that the Pages in this PageSet can hold.
    /// </summary>
    private readonly Type RecordType;

    /// <summary>
    /// Sets the currently active Page for this PageSet. 
    /// </summary>
    private int ActivePageIndex = 0;

    /// <summary>
    /// Returns the Page object that is currently active in this PageSet.
    /// </summary>
    private Page ActivePage => Pages[ActivePageIndex];

    /// <summary>
    /// Returns whether or not the PageSet has a Page immediately after the current Active Page.
    /// </summary>
    private bool HasNext => ActivePageIndex + 1 < PageCount - 1;

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
    public int PageCount => Pages.Count;

    /// <summary>
    /// The total number of entries in this PageSet (totalled from each included Page object).
    /// </summary>
    public int EntryCount => GetTotalEntryCount();

    /// <summary>
    /// Whether this PageSet can contain more Pages or not.
    /// Always true for unlimited PageSets (MaxCount = -1).
    /// </summary>
    public bool HasSpace
    {
        get 
        {
            if (!IsLimitedSize)
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
    /// Parses the Lines of the PageSet's Active Page so it can be displayed.
    /// </summary>
    /// <returns></returns>
    private async Task GenerateActivePage()
    {
        // Parse the Active Page so it can be displayed
        if (!ActivePage.IsParsed)
        {
            await ActivePage.GenerateDataRecords();
        }
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
        if (IsLimitedSize)
        {
            Pages = GeneratedPages.Take(MaxCount).ToList();
        }
        else
        {
            Pages = GeneratedPages;
        }
    }

    /// <summary>
    /// Returns the PageSet's current Active Page object.
    /// </summary>
    /// <remarks>
    /// Throws IndexOutOfRangeException if there was no Page at Pages[ActivePageIndex].
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public async Task<Page> GetActivePage() {
        // confirm a Page exists at the ActivePageIndex and return it
        try
        {
            await GenerateActivePage();
            return ActivePage;
        }
        catch
        {
            throw new IndexOutOfRangeException("There is no Page at the set ActivePageIndex.");
        }
    }

    /// <summary>
    /// Sets the PageSet's Active Page by setting ActivePageIndex. 
    /// </summary>
    /// <remarks>
    /// Cannot be set to integer values lower than 0 (negative integers).
    /// Cannot exceed the PageSet's maximum Page count (if set).
    /// Throws IndexOutOfRangeException if there is no Page object at Pages[PageNumber].
    /// </remarks>
    /// <param name="PageNumber"></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public async Task SetActivePage(int PageNumber)
    {
        // bar from setting to negative indexes and exceeding a set MaxCount
        if (PageNumber < 0)
        {
            PageNumber = 0;
        }
        if (IsLimitedSize && PageNumber > MaxCount - 1)
        {
            PageNumber = MaxCount - 1;
        }
        // confirm that there is a Page at the requested index
        try
        {
            _ = Pages[PageNumber];
            // if this line is reached, there was a Page at PageNumber index
            ActivePageIndex = PageNumber;
        }
        catch
        {
            throw new IndexOutOfRangeException("There is no Page at the requested Index.");
        }
        await GenerateActivePage();
    }

    /// <summary>
    /// Jumps to the first Page in the PageSet (the newest Records).
    /// </summary>
    public async Task<Page> GoToFirstPage()
    {
        ActivePageIndex = 0;
        await GenerateActivePage();
        return ActivePage;
    }

    /// <summary>
    /// Goes to the previous Page in the PageSet (if one exists).
    /// </summary>
    public async Task<Page> GoToPreviousPage()
    {
        // bar from going below 0
        if (ActivePageIndex == 0)
        {
            ActivePageIndex = 0;
        }
        else
        {
            ActivePageIndex -= 1;
        }
        await GenerateActivePage();
        return ActivePage;
    }

    /// <summary>
    /// Jumps to the last Page in the PageSet (the oldest Records).
    /// </summary>
    public async Task<Page> GoToLastPage()
    {
        ActivePageIndex = PageCount - 1;
        await GenerateActivePage();
        return ActivePage;
    }

    /// <summary>
    /// Goes to the next Page in the PageSet (if one exists).
    /// </summary>
    public async Task<Page> GoToNextPage()
    {
        // confirm the PageSet has a Page after the current one
        if (HasNext)
        {
            ActivePageIndex += 1;
        }
        await GenerateActivePage();
        return ActivePage;
    }

    /// <summary>
    /// Calculates a Total of all entries in the PageSet's individual Pages.
    /// </summary>
    /// <returns></returns>
    public int GetTotalEntryCount()
    {
        int TotalCount = 0;
        foreach (Page _page in Pages)
        {
            TotalCount += _page.Lines.Count;
        }
        return TotalCount;
    }
}
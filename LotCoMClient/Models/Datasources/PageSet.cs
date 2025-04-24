namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides a control structure for a set of Page objects that can be used in the same context.
/// </summary>
/// <param name="MaxCount">(Optional) Constrains the amount of Pages that are allowed in the PageSet.</param>
public class PageSet(int MaxCount = -1)
{
    /// <summary>
    /// Sets a maximum number of Pages allowed in this PageSet.
    /// </summary>
    private int MaxCount = MaxCount;

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
}
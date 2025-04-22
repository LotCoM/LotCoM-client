namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides controlled access to Part Data from the Process Masterlist data source.
/// </summary>
public class PartData 
{
    /// <summary>
    /// Allows access to the Process Data source file.
    /// </summary>
    private ProcessData ProcessData = new ProcessData();

    /// <summary>
    /// Retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="ProcessFullName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public List<Part> GetProcessParts(string ProcessFullName) 
    {
        // load the Process' data
        Process Process = ProcessData.GetIndividualProcessData(ProcessFullName);
        // no Part data was read
        if (Process.Parts.Count < 1) 
        {
            throw new ArgumentException($"No Part data found for the Process '{ProcessFullName}'.");
        } 
        // return the Part list
        return Process.Parts;
    }

    /// <summary>
    /// Asynchronously retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns>A list of Part objects assigned to the Process.</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<List<Part>> GetProcessPartsAsync(string ProcessFullName) 
    {
        // load the Process' data
        Process Process = await ProcessData.GetIndividualProcessDataAsync(ProcessFullName);
        // no Part data was read
        if (Process.Parts.Count < 1) 
        {
            throw new ArgumentException($"No Part data found for the Process '{ProcessFullName}'.");
        } 
        // return the Part list
        return Process.Parts;
    }

    /// <summary>
    /// Retrieves and formats ProcessFullName's Part list as a list of Displayable strings.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns>A List of strings.</returns>
    public async Task<List<string>> GetDisplayableProcessPartsAsync(string ProcessFullName) 
    {
        // retrieve the Process' parts
        List<Part> ProcessParts = await GetProcessPartsAsync(ProcessFullName);
        // convert each Part Token into a Displayable string
        List<string> PartStrings = ProcessParts
            .Select(x => $"{x.PartNumber}\n{x.PartName}")
            .ToList();
        // return the converted list
        return PartStrings;
    }

    /// <summary>
    /// Queries for a Part matching PartNumber in ProcessFullName's Part data.
    /// </summary>
    /// <param name="ProcessFullName">The FULL Name ("Code-Title") of the Process to query from.</param>
    /// <param name="PartNumber">The Part Number to query for within ProcessFullName's data.</param>
    /// <returns>A JToken object containing the Part data for PartNumber.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    public Part GetPartData(string ProcessFullName, string PartNumber) 
    {
        // retrieve the Process' Part list
        List<Part> ProcessParts = GetProcessParts(ProcessFullName);
        // no Part data for this Process
        if (ProcessParts.Count == 0) 
        {
            throw new ArgumentException($"No Part data has been assigned to Process '{ProcessFullName}'.");
        }
        // attempt to access the specific Part
        Part? SelectedPart;
        try 
        {
            SelectedPart = ProcessParts
                .Where(x => x.PartNumber
                .Equals(PartNumber))
                .First();
        // Part was not found in the Process' Part list
        } 
        catch 
        {
            throw new ArgumentException($"Part '{PartNumber}' not found assigned to Process '{ProcessFullName}'.");
        }
        return SelectedPart;
    }

    /// <summary>
    /// Asynchronously queries for a Part matching PartNumber in ProcessFullName's Part data.
    /// </summary>
    /// <param name="ProcessFullName">The FULL Name ("Code-Title") of the Process to query from.</param>
    /// <param name="PartNumber">The Part Number to query for within ProcessFullName's data.</param>
    /// <returns>A JToken object containing the Part data for PartNumber.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    public async Task<Part> GetPartDataAsync(string ProcessFullName, string PartNumber) 
    {
        // perform the query on a new CPU thread
        Part PartData = await Task.Run(async () => 
        {
            // retrieve the Process' Part list
            List<Part> ProcessParts = await GetProcessPartsAsync(ProcessFullName);
            // no Part data for this Process
            if (ProcessParts.Count == 0) 
            {
                throw new ArgumentException($"No Part data has been assigned to Process '{ProcessFullName}'.");
            }
            // attempt to access the specific Part
            Part? SelectedPart;
            try 
            {
                SelectedPart = ProcessParts
                    .Where(x => x.PartNumber
                    .Equals(PartNumber))
                    .First();
            // Part was not found in the Process' Part list
            } 
            catch 
            {
                throw new ArgumentException($"Part '{PartNumber}' not found assigned to Process '{ProcessFullName}'.");
            }
            return SelectedPart;
        });
        // return the queried Part data
        return PartData;
    }
}
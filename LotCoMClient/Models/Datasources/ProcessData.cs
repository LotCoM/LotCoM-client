using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides controlled access to Process data sources.
/// </summary>
public class ProcessData() 
{
    private const string Path = "\\\\144.133.122.1\\Lot Control Management\\Database\\process_control\\_process_masterlist.json";
        
    /// <summary>
    /// Contains the full JObject object produced by the last LoadDataAsync call. 
    /// </summary>
    private JObject? LastRead = null;

    /// <summary>
    /// Synchronously loads the data from the Process Masterlist data source. Stores this data in the LastRead property.
    /// </summary>
    /// <returns>A JSON dictionary containing the Process Masterlist data.</returns>
    /// <exception cref="JsonException"></exception>
    private void LoadData() 
    {
        // read the masterlist file
        LastRead = JObject.Parse(File.ReadAllText(Path));
    }

    /// <summary>
    /// Asynchronously loads the data from the Process Masterlist data source. Stores this data in the LastRead property.
    /// </summary>
    /// <exception cref="JsonException"></exception>
    private async Task LoadDataAsync() 
    {
        // read the masterlist file
        LastRead = JObject.Parse(await File.ReadAllTextAsync(Path));
    }

    /// <summary>
    /// Attempts to resolve a Part object from the data in Token.
    /// </summary>
    /// <param name="Token">A JToken object containing Part data.</param>
    /// <param name="ParentProcess">The known Process that the Part should belong to.</param>
    /// <returns>A Part object with data resolved from the JToken.</returns>
    /// <exception cref="FormatException"></exception>
    private static Part ResolvePartFromToken(JToken Token, string ParentProcess) 
    {
        // hold variables for each Part object property
        string Number;
        string Name;
        string Model;
        // attempt to pull the needed fields from the passed JToken
        try 
        {
            Number = Token["Number"]!.ToString();
            Name = Token["Name"]!.ToString();
            Model = Token["Model"]!.ToString();
        // one of the needed fields was not accessible
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Part object.");
        }
        // attempt to construct the Part object from the resolved data
        Part ResolvedPart;
        try 
        {
            ResolvedPart = new Part(ParentProcess, Number, Name, Model);
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Part object.");
        }
        // return the resolved Part object
        return ResolvedPart;
    }

    /// <summary>
    /// Attempts to resolve a Process object from the data in Token.
    /// </summary>
    /// <param name="Token">A JToken object containing Part data.</param>
    /// <returns>A Process object with data resolved from the JToken.</returns>
    /// <exception cref="FormatException"></exception>
    private static Process ResolveProcessFromToken(JToken Token) 
    {
        // hold variables for each Process object property
        string LineCode;
        string Line;
        string Title;
        string Type;
        string Serialization;
        JToken Parts;
        JToken Requirements;
        // attempt to access each field of Data from the Process Token
        try 
        {
            LineCode = Token["LineCode"]!.ToString();
            Line = Token["Line"]!.ToString();
            Title = Token["Title"]!.ToString();
            Type = Token["Type"]!.ToString();
            Serialization = Token["Serialization"]!.ToString();
            Parts = Token["Parts"]!;
            Requirements = Token["Requirements"]!;
        // one of the needed fields was not accessible
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Process object.");
        }
        // process and add each part to the parts list individually
        List<Part> PartObjects = [];
        string ProcessName = $"{LineCode}-{Line}-{Title}";
        try 
        {
            PartObjects = Parts
                .Select(x => 
                ResolvePartFromToken(x, ProcessName))
                .ToList();
        // one of the Tokens could not be resolved to a Part
        } 
        catch (Exception _ex) 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Process object, due to the following Part resolution failure: {_ex.Message}");
        }
        // create requirements list; add first set of universal fields
        List<string> RequiredFields = ["SelectedProcess", "SelectedPart", "Quantity"];
        // add variable (process-dependent) fields
        foreach (JToken _field in Requirements) 
        {
            RequiredFields.Add(_field.ToString());
        }
        // add the second set of universal fields
        RequiredFields.AddRange(["ProductionDate", "ProductionShift", "OperatorID"]);
        // attempt to construct the Process object from the resolved data
        Process ResolvedProcess;
        try 
        {
            ResolvedProcess = new Process(LineCode, Line, Title, Type, Serialization, PartObjects, RequiredFields);
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Process object.");
        }
        // return the resolved Process object
        return ResolvedProcess;
    }

    /// <summary>
    /// Attempts to resolve a Department object from the data in Token.
    /// </summary>
    /// <param name="Token">A JToken object containing Department data.</param>
    /// <returns>A Department object with data resolved from the JToken.</returns>
    /// <exception cref="FormatException"></exception>
    private static Department ResolveDepartmentFromToken(JToken Token) 
    {
        // hold variables for each Department object property
        string Title;
        string Code;
        List<string> Lines = [];
        // attempt to access each field of Data from the Department Token
        try 
        {
            Title = Token["Title"]!.ToString();
            Code = Token["Code"]!.ToString();
        // one of the needed fields was not accessible
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Department object.");
        }
        // add each Line to the Lines list individually
        try 
        {
            // resolve a Line from each Token
            Lines = Token["Lines"]!
                .Select(x => x
                .ToString())
                .ToList();
        // one of the Tokens could not be resolved to a Line
        } 
        catch (Exception _ex) 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Department object, due to the following Line resolution failure: {_ex.Message}");
        }
        // attempt to construct the Department object from the resolved data
        Department ResolvedDepartment;
        try 
        {
            ResolvedDepartment = new Department(Title, Code, Lines);
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Department object.");
        }
        // return the resolved Process object
        return ResolvedDepartment;
    }

    /// <summary>
    /// Retrieves the list of Processes, as Process objects, from the Process Masterlist.
    /// </summary>
    /// <returns>A list of Process objects.</returns>
    /// <exception cref="FileLoadException"></exception>
    public List<Process> GetAllProcesses() 
    {
        if (LastRead is null) 
        {
            Console.WriteLine($"LastRead null: {LastRead is null}. Reading from GetIndividualProcessAsync()...");
            // load the data from the Masterlist
            LoadData();
        }
        // return the list of Processes in the Masterlist
        if (LastRead!["Processes"] is null) 
        {
            throw new FileLoadException("Failed to load the Processes from the Process Masterlist data source.");
        }
        // convert the Process tokens into Process objects
        List<Process> ProcessObjects;
        try
        {
            ProcessObjects = LastRead["Processes"]!
                .Select(ResolveProcessFromToken)
                .ToList();
        }
        catch (Exception _ex)
        {
            throw new FormatException($"Failed to load Processes due to the following exception: {_ex.Message}.");
        }
        return ProcessObjects;
    }

    /// <summary>
    /// Synchronously retrieves a list of Process Full Names ("Code-Title").
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllProcessNames() 
    {
        if (LastRead is null) 
        {
            Console.WriteLine($"LastRead null: {LastRead is null}. Reading from GetIndividualProcessAsync()...");
            // load the data from the Masterlist
            LoadData();
        }
        // create a List of all Process Names
        List<string> Processes = LastRead!["Processes"]!
            .Select(x => $"{x["LineCode"]}-{x["Line"]}-{x["Title"]}"!
            .ToString())
            .ToList();
        return Processes;
    }

    /// <summary>
    /// Loads and queries the Process Masterlist data for data connected to ProcessFullName. 
    /// Returns a Process object constructed from the first found match.
    /// </summary>
    /// <param name="ProcessFullName">The FULL name of a Process ("Code-Title") to query for.</param>
    /// <returns>A Process object.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Process GetIndividualProcess(string ProcessFullName) 
    {
        if (LastRead is null) 
        {
            Console.WriteLine($"LastRead null: {LastRead is null}. Reading from GetIndividualProcessAsync()...");
            // load the data from the Masterlist
            LoadData();
        }
        // attempt to access the data for the passed Process
        JToken SelectedData;
        try 
        {
            SelectedData = LastRead!["Processes"]!
                .Where(x => $"{x["LineCode"]}-{x["Line"]}-{x["Title"]}"
                .ToString() == ProcessFullName)
                .First();
        // no processes matched the name
        } 
        catch 
        {
            throw new ArgumentException($"Could not resolve process '{ProcessFullName}'.");
        }
        // resolve the Token to a Process
        Process ResolvedProcess;
        try 
        {
            ResolvedProcess = ResolveProcessFromToken(SelectedData);
        // the Token could not be resolved to a Process
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{SelectedData}' to a Process object.");
        }
        // return the resolved Process object
        return ResolvedProcess;
    }

    /// <summary>
    /// Asynchronously loads and queries the Process Masterlist data for data connected to ProcessFullName. 
    /// Returns a Process object constructed from the first found match.
    /// </summary>
    /// <param name="ProcessFullName">The FULL name of a Process ("Code-Title") to query for.</param>
    /// <returns>A Process object.</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Process> GetIndividualProcessAsync(string ProcessFullName) 
    {
        if (LastRead == null) 
        {
            Console.WriteLine($"LastRead null: {LastRead == null}. Reading from GetIndividualProcessAsync()...");
            // load the data from the Masterlist
            await LoadDataAsync();
        }
        // attempt to access the data for the passed Process
        JToken SelectedData;
        try 
        {
            SelectedData = LastRead!["Processes"]!
                .Where(x => $"{x["LineCode"]}-{x["Line"]}-{x["Title"]}"
                .ToString() == ProcessFullName)
                .First();
        // no processes matched the name
        } 
        catch 
        {
            throw new ArgumentException($"Could not resolve process '{ProcessFullName}'.");
        }
        // resolve the Token to a Process
        Process ResolvedProcess;
        try 
        {
            ResolvedProcess = ResolveProcessFromToken(SelectedData);
        // the Token could not be resolved to a Process
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{SelectedData}' to a Process object.");
        }
        // return the resolved Process object
        return ResolvedProcess;
    }

    /// <summary>
    /// Retrieves a List of all Departments from the Process Masterlist.
    /// </summary>
    /// <returns></returns>
    public List<Department> GetDepartments() 
    {
        if (LastRead is null) 
        {
            Console.WriteLine($"LastRead null: {LastRead is null}. Reading from GetIndividualProcessAsync()...");
            // load the data from the Masterlist
            LoadData();
        }
        // create a List of all Departments
        List<Department> Departments = LastRead!["Departments"]!
            .Select(ResolveDepartmentFromToken)
            .ToList();
        return Departments;
    }

    /// <summary>
    /// Searches for a Department that has a Title the matches DepartmentTitle.
    /// </summary>
    /// <param name="DepartmentTitle"></param>
    /// <returns>A Department object.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Department GetIndividualDepartment(string DepartmentTitle) 
    {
        // retrieve all of the Departments
        List<Department> Departments = GetDepartments();
        // try to find a match for the passed Title
        List<Department> Matches = Departments
            .Where(x => x.Title
            .Equals(DepartmentTitle))
            .ToList();
        if (Matches.Count <= 0) 
        {
            // there was no match, throw an exception
            throw new ArgumentException($"Could not match '{DepartmentTitle}' to a defined Department.");
        }
        // return the first of the Matches
        return Matches[0];
    }

    /// <summary>
    /// Retrieves ProcessFullName's serialization status.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to check.</param>
    /// <returns>"Originator" || "Pass-through".</returns>
    public async Task<string> IsOriginator(string ProcessFullName) 
    {
        // load the Process' data
        Process Data = await GetIndividualProcessAsync(ProcessFullName);
        // check whether the process is an originator or not
        return Data.Type;
    }

    /// <summary>
    /// Retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="ProcessFullName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public List<Part> GetProcessParts(string ProcessFullName) 
    {
        // load the Process' data
        Process Process = GetIndividualProcess(ProcessFullName);
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
        Process Process = await GetIndividualProcessAsync(ProcessFullName);
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
    public Part GetProcessPartData(string ProcessFullName, string PartNumber) 
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
    public async Task<Part> GetProcessPartDataAsync(string ProcessFullName, string PartNumber) 
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
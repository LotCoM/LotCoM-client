using System.Text.RegularExpressions;
using LotCoMClient.Models.Exceptions;
using LotCoMClient.Models.Options;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides parsing methods for DataRecord objects.
/// </summary>
/// <remarks>
/// Supports parsing from CSV.
/// </remarks>
public partial class RecordParser() 
{
    /// <summary>
    /// Allows access to the Process Data source file.
    /// </summary>
    private readonly ProcessData ProcessData = new ProcessData();

    /// <summary>
    /// Call IPAddressRegex.IsMatch on a string to confirm it is a valid IP Address.
    /// </summary>
    private static readonly Regex IPAddressRegex = GenerateIPAddressRegex();

    /// <summary>
    /// Compiles a Regular Expression in the format of an IP Address.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\d\d?\d?\.\d\d?\d?\.\d\d?\d?\.\d\d?\d?$")]
    private static partial Regex GenerateIPAddressRegex();
    
    /// <summary>
    /// Asynchronously checks if the first element of a split CSV Line is an IP address.
    /// </summary>
    /// <param name="SplitCSVLine">A CSV Line that has already been split by commas.</param>
    /// <returns>Null if the first element is not an IP address; the IP address if it is.</returns>
    private async Task<string?> ParseIPAddressAsync(List<string> SplitCSVLine) 
    {
        return await Task.Run(() => 
        {
            // peek the first element and test it as an IP Address using a Regex pattern
            if (IPAddressRegex.IsMatch(SplitCSVLine[0])) 
            {
                // return the first field (confirmed as an IP Address)
                return SplitCSVLine[0];
            } 
            else 
            {
                return null;
            }
        });
    }

    /// <summary>
    /// Asynchronously attempts to parse a valid Process object from the first element of a split CSV Line.
    /// </summary>
    /// <remarks>
    /// Throws RecordParseException if the Process in the CSV Line was not parsable into a defined Process.
    /// </remarks>
    /// <param name="SplitCSVLine">A CSV Line that has already been split by commas.</param>
    /// <returns>A Process object, as parsed from the CSV Line.</returns>
    /// <exception cref="RecordParseException"></exception>
    private async Task<Process> ParseRecordProcessAsync(List<string> SplitCSVLine) 
    {
        // confirm that the Process is a valid process
        try 
        {
            return await ProcessData.GetIndividualProcessAsync(SplitCSVLine[0]);
        } 
        catch 
        {
            throw new RecordParseException($"The Process {SplitCSVLine[0]} is not defined.");
        }
    }

    /// <summary>
    /// Asynchronously attempts to parse a valid Part object from the first element of a split CSV Line.
    /// Uses the Parts associated with ParsedProcess to validate the parse.
    /// </summary>
    /// <remarks>
    /// Throws RecordParseException if the Part in the CSV Line was not parsable into a defined Part on ParsedProcess.
    /// </remarks>
    /// <param name="SplitCSVLine">A CSV Line that has already been split by commas.</param>
    /// <param name="ParsedProcess">A Process Object.</param>
    /// <returns>A Part object, as parsed from the CSV Line.</returns>
    /// <exception cref="RecordParseException"></exception>
    private async Task<Part> ParseRecordPartAsync(List<string> SplitCSVLine, Process ParsedProcess) 
    {
        // confirm that the Part Number and Part Name belong to a valid part
        try 
        {
            return await ProcessData.GetProcessPartDataAsync(ParsedProcess.FullName, SplitCSVLine[1]);
        } 
        catch 
        {
            throw new RecordParseException($"The Part {SplitCSVLine[1]} {SplitCSVLine[2]} is not defined for Process {ParsedProcess.FullName}");
        }
    }

    /// <summary>
    /// Asynchronously attempts to parse each of the variably-required data fields for ParsedProcess from a split CSV Line.
    /// </summary>
    /// <param name="SplitCSVLine">A CSV Line that has already been split by commas.</param>
    /// <param name="ParsedProcess">A Process Object.</param>
    /// <returns></returns>
    private async Task<VariableFieldSet> ParseVariableFieldSetAsync(List<string> SplitCSVLine, Process ParsedProcess) 
    {
        return await Task.Run(() => 
        {
            // parse the required variable data fields
            VariableFieldSet VariableSet = new VariableFieldSet();
            List<string> Requirements = ParsedProcess.RequiredFields;
            // references next parsable index after the static Quantity, position 4 (index 3); 
            // increments when a property is found to be required and is assigned a parsable index
            int _parsingIndex = 0;
            // attempt to parse a JBK number
            if (Requirements.Contains("JBKNumber")) 
            {
                // assign the value of the current parsing index to the JBK Number property
                VariableSet.JBKNumber = SplitCSVLine[4 + _parsingIndex];
                // increment to the next parsable index
                _parsingIndex += 1;
            }
            // attempt to parse a Lot number
            if (Requirements.Contains("LotNumber")) 
            {
                // assign the value of the current parsing index to the Lot Number property
                VariableSet.LotNumber = SplitCSVLine[4 + _parsingIndex];
                // increment to the next parsable index
                _parsingIndex += 1;
            }
            // attempt to parse a Deburr JBK number
            if (Requirements.Contains("DeburrJBKNumber")) 
            {
                // assign the value of the current parsing index to the Deburr JBK Number property
                VariableSet.DeburrJBKNumber = SplitCSVLine[4 + _parsingIndex];
                // increment to the next parsable index
                _parsingIndex += 1;
            }
            // attempt to parse a Die number
            if (Requirements.Contains("DieNumber")) 
            {
                // assign the value of the current parsing index to the Die Number property
                VariableSet.DieNumber = SplitCSVLine[4 + _parsingIndex];
                // increment to the next parsable index
                _parsingIndex += 1;
            }
            // attempt to parse a Model number
            if (Requirements.Contains("ModelNumber")) 
            {
                // assign the value of the current parsing index to the Model Number property
                VariableSet.ModelNumber = SplitCSVLine[4 + _parsingIndex];
                // increment to the next parsable index
                _parsingIndex += 1;
            }
            // attempt to parse a Heat number
            if (Requirements.Contains("HeatNumber")) 
            {
                // assign the value of the current parsing index to the Heat Number property
                VariableSet.HeatNumber = SplitCSVLine[4 + _parsingIndex];
                // increment to the next parsable index
                _parsingIndex += 1;
            }
            return VariableSet;
        });
    }

    /// <summary>
    /// Parses a DataRecord object from a CSV Line that has been split by Comma ','.
    /// </summary>
    /// <param name="SplitCSVLine"></param>
    /// <returns>A DataRecord object that can be further parsed into a ScanRecord or PrintRecord object.</returns>
    /// <exception cref="RecordParseException"></exception>
    private async Task<DataRecord> ParseBaseDataRecord(List<string> SplitCSVLine) {
        // parse out Process and Part objects
        Process RecordProcess;
        Part RecordPart;
        try 
        {
            RecordProcess = await ParseRecordProcessAsync(SplitCSVLine);
            RecordPart = await ParseRecordPartAsync(SplitCSVLine, RecordProcess);
        } 
        catch 
        {
            throw new RecordParseException();
        }
        // parse out universally required data fields
        string Quantity = SplitCSVLine[3];
        List<string> Timestamp = SplitCSVLine[^3]
            .Split("-")
            .ToList();
        string RecordDate = Timestamp[0];
        string RecordTime = Timestamp[1];
        string RecordShift = SplitCSVLine[^2];
        string OperatorID = SplitCSVLine[^1];
        // attempt to parse out any variably-required fields
        VariableFieldSet VariableFields;
        try
        {
            VariableFields = await ParseVariableFieldSetAsync(SplitCSVLine, RecordProcess);
        } 
        catch
        {
            throw new RecordParseException();
        }
        // attempt to create a DataRecord from the parsed data
        try 
        {
            return new DataRecord(RecordProcess, RecordPart, Quantity, VariableFields.JBKNumber, VariableFields.LotNumber, VariableFields.DeburrJBKNumber, VariableFields.DieNumber, VariableFields.ModelNumber, VariableFields.HeatNumber, RecordDate, RecordTime, RecordShift, OperatorID);
        // there was a problem constructing a DataRecord from the parsed data
        } 
        catch 
        {
            throw new RecordParseException();
        }
    }

    /// <summary>
    /// Constructs a complete ScanRecord object from CSVLine.
    /// </summary>
    /// <param name="CSVLine"></param>
    /// <returns>A ScanRecord object.</returns>
    /// <exception cref="RecordParseException"></exception>
    public async Task<ScanRecord> ParseScanRecordFromCSVAsync(string CSVLine) 
    {
        // split the CSV Line by commas
        List<string> SplitLine = CSVLine
            .Split(",")
            .ToList();
        // test for an IP Address
        string? ScanAddress;
        ScanAddress = await ParseIPAddressAsync(SplitLine);
        // remove the IP address from the SplitLine list (if parsed)
        if (ScanAddress is null) 
        {
            throw new RecordParseException();
        }
        else 
        {
            SplitLine.RemoveAt(0);
        }
        // parse a base DataRecord, apply the IP address value, and convert to ScanRecord object
        ScanRecord ParsedRecord;
        try
        {
            DataRecord BaseRecord = await ParseBaseDataRecord(SplitLine);
            BaseRecord.ScanAddress = ScanAddress;
            ParsedRecord = ScanRecord.ConvertFromBase(BaseRecord);
        }
        catch
        {
            throw new RecordParseException();
        }
        return ParsedRecord;
    }

    /// <summary>
    /// Constructs a complete PrintRecord object from CSVLine.
    /// </summary>
    /// <param name="CSVLine"></param>
    /// <returns>A PrintRecord object.</returns>
    /// <exception cref="RecordParseException"></exception>
    public async Task<PrintRecord> ParsePrintRecordFromCSVAsync(string CSVLine) 
    {
        // split the CSV Line by commas
        List<string> SplitLine = CSVLine
            .Split(",")
            .ToList();
        // parse a base DataRecord and convert to PrintRecord object
        PrintRecord ParsedRecord;
        try
        {
            DataRecord BaseRecord = await ParseBaseDataRecord(SplitLine);
            ParsedRecord = PrintRecord.ConvertFromBase(BaseRecord);
        }
        catch
        {
            throw new RecordParseException();
        }
        return ParsedRecord;
    }
}
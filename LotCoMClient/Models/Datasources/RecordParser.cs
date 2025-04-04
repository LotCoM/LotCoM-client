using System.Text.RegularExpressions;
using LotCoMClient.Models.Exceptions;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides parsing methods for DataRecord objects.
/// </summary>
/// <remarks>
/// Supports parsing from CSV.
/// </remarks>
public static class RecordParser {
    /// <summary>
    /// Attempts to parse a DataRecord object from a CSV Line.
    /// </summary>
    /// <remarks>
    /// Throws RecordParseException if the line contains too few fields
    /// or if the Parser fails to construct a DataRecord object from the parsed fields.
    /// </remarks>
    /// <param name="CSVLine"></param>
    /// <returns>A DataRecord object.</returns>
    /// <exception cref="RecordParseException"></exception>
    public static DataRecord ParseFromCSV(string CSVLine) {
        // split the CSV Line by commas
        List<string> SplitLine = CSVLine.Split(",").ToList();
        // confirm that the split list contains 8 universal fields and AT LEAST 1 variable field
        if (SplitLine.Count < 7) {
            throw new RecordParseException();
        }
        // peek the first element and test it as an IP Address using a Regex pattern
        string? ScanAddress = null;
        string AddressPattern = @"^\d\d?\d?\.\d\d?\d?\.\d\d?\d?\.\d\d?\d?$";
        Regex AddressRegex = new Regex(AddressPattern);
        if (AddressRegex.IsMatch(SplitLine[0])) {
            // set the ScanAddress property using this field and pop it off of the front
            ScanAddress = SplitLine[0];
            SplitLine.RemoveAt(0);
        }
        // prepare remaining universal DataRecord properties
        Process RecordProcess;
        Part RecordPart;
        string Quantity = SplitLine[3];
        string JBKNumber = "";
        string LotNumber = "";
        string DeburrJBKNumber = "";
        string DieNumber = "";
        string ModelNumber = "";
        string HeatNumber = "";
        string RecordDate = SplitLine[^3].Split("-")[0];
        string RecordTime = SplitLine[^3].Split("-")[1];
        string RecordShift = SplitLine[^2];
        string OperatorID = SplitLine[^1];
        // confirm that the Process is a valid process
        try {
            RecordProcess = ProcessData.GetIndividualProcessData(SplitLine[0]);
        } catch {
            throw new RecordParseException($"The Process {SplitLine[0]} is not defined.");
        }
        // confirm that the Part Number and Part Name belong to a valid part
        try {
            RecordPart = PartData.GetPartData(RecordProcess.FullName, SplitLine[1]);
        } catch {
            throw new RecordParseException($"The Part {SplitLine[1]} {SplitLine[2]} is not defined for Process {RecordProcess.FullName}");
        }
        // parse the required variable data fields
        List<string> Requirements = RecordProcess.RequiredFields;
        // references next parsable index after the static Quantity, position 4 (index 3); 
        // increments when a property is found to be required and is assigned a parsable index
        int _parsingIndex = 0;
        // attempt to parse a JBK number
        if (Requirements.Contains("JBKNumber")) {
            // assign the value of the current parsing index to the JBK Number property
            JBKNumber = SplitLine[4 + _parsingIndex];
            // increment to the next parsable index
            _parsingIndex += 1;
        }
        // attempt to parse a Lot number
        if (Requirements.Contains("LotNumber")) {
            // assign the value of the current parsing index to the Lot Number property
            LotNumber = SplitLine[4 + _parsingIndex];
            // increment to the next parsable index
            _parsingIndex += 1;
        }
        // attempt to parse a Deburr JBK number
        if (Requirements.Contains("DeburrJBKNumber")) {
            // assign the value of the current parsing index to the Deburr JBK Number property
            DeburrJBKNumber = SplitLine[4 + _parsingIndex];
            // increment to the next parsable index
            _parsingIndex += 1;
        }
        // attempt to parse a Die number
        if (Requirements.Contains("DieNumber")) {
            // assign the value of the current parsing index to the Die Number property
            DieNumber = SplitLine[4 + _parsingIndex];
            // increment to the next parsable index
            _parsingIndex += 1;
        }
        // attempt to parse a Model number
        if (Requirements.Contains("ModelNumber")) {
            // assign the value of the current parsing index to the Model Number property
            ModelNumber = SplitLine[4 + _parsingIndex];
            // increment to the next parsable index
            _parsingIndex += 1;
        }
        // attempt to parse a Heat number
        if (Requirements.Contains("HeatNumber")) {
            // assign the value of the current parsing index to the Heat Number property
            HeatNumber = SplitLine[4 + _parsingIndex];
            // increment to the next parsable index
            _parsingIndex += 1;
        }
        // attempt to create a DataRecord from the parsed data
        try {
            return new DataRecord(RecordProcess, RecordPart, Quantity, JBKNumber, LotNumber, DeburrJBKNumber, DieNumber, ModelNumber, HeatNumber, RecordDate, RecordTime, RecordShift, OperatorID, ScanAddress);
        // there was a problem constructing a DataRecord from the parsed data
        } catch {
            throw new RecordParseException();
        }
    }
}
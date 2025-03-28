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
        // prepare DataRecord properties
        Process RecordProcess;
        Part RecordPart;
        string Quantity = SplitLine[3];
        string JBKNumber = "";
        string LotNumber = "";
        string DeburrJBKNumber = "";
        string DieNumber = "";
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
        List<string> InnerFields = [JBKNumber, LotNumber, DeburrJBKNumber, DieNumber, HeatNumber];
        // use a dual-index loop structure 
        // parsingIndex: reference next parsable index after the static Quantity, position 4 (index 3)
            // only increments when a property is found to be required and is assigned a parsable index
        // propertyIndex: reference next property in InnerFields
            // always increments
        int _parsingIndex = 0;
        int _propertyIndex = 0;
        foreach (string _field in InnerFields) {
            // the current required field name is in the Process requirements
            if (Requirements.Contains(nameof(_field))) {
                // update the variable field at the current index
                InnerFields[_propertyIndex] = SplitLine[4 + _parsingIndex];
                // increment to the next parsable index
                _parsingIndex += 1;
            }
            // increment the property index regardless
            _propertyIndex += 1;
        }
        // attempt to create a DataRecord from the parsed data
        try {
            return new DataRecord(RecordProcess, RecordPart, Quantity, JBKNumber, LotNumber, DeburrJBKNumber, DieNumber, HeatNumber, RecordDate, RecordTime, RecordShift, OperatorID);
        // there was a problem constructing a DataRecord from the parsed data
        } catch {
            throw new RecordParseException();
        }
    }
}
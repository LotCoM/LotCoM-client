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
        // prepare the universal properties
        Process RecordProcess;
        Part RecordPart;
        // TODO: replace these with flexible parsing
        string Quantity = SplitLine[3];
        string RecordDate = SplitLine[^3].Split("-")[0];
        string RecordTime = SplitLine[^3].Split("-")[1];
        string RecordShift = SplitLine[^2];
        string OperatorID = SplitLine[^1];
        // get the variable inner fields
        List<string> InnerValues = SplitLine.GetRange(4, SplitLine.Count - 8);
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
        // attempt to create a DataRecord from the parsed data
        try {
            return new DataRecord(RecordProcess, RecordPart, Quantity, [], InnerValues, RecordDate, RecordTime, RecordShift, OperatorID);
        // there was a problem constructing a DataRecord from the parsed data
        } catch {
            throw new RecordParseException();
        }
    }
}
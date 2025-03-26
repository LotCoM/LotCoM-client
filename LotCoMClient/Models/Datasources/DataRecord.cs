using LotCoMClient.Models.Exceptions;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Base class for all table entry records in LotCoM database tables.
/// </summary>
/// <param name="Process">The Full Name of the Process this record belongs to.</param>
/// <param name="PartNumber">The Part Number assigned to this record.</param>
/// <param name="PartName">The Part Name assigned to this record.</param>
/// <param name="Quantity">The Quantity assigned to this record.</param>
/// <param name="InnerKeys">A List of Keys to apply to the Values in InnerValues.</param>
/// <param name="InnerValues">A List of Inner Values (variable values from Process to Process) assigned to this record.</param>
/// <param name="RecordDate">The Date assigned to this record.</param>
/// <param name="RecordTime">The Time assigned to this record.</param>
/// <param name="RecordShift">The Shift Number assigned to this record.</param>
/// <param name="OperatorID">The Operator ID assigned to this record.</param>
public class DataRecord(string Process, string PartNumber, string PartName, string Quantity, List<string> InnerKeys, List<string> InnerValues, string RecordDate, string RecordTime, string RecordShift, string OperatorID) {
    public string Process = Process;
    public string PartNumber = PartNumber;
    public string PartName = PartName;
    public string Quantity = Quantity; 
    public List<string> InnerKeys = InnerKeys; 
    public List<string> InnerValues = InnerValues; 
    public string RecordDate = RecordDate; 
    public string RecordTime = RecordTime; 
    public string RecordShift = RecordShift; 
    public string OperatorID = OperatorID;

    /// <summary>
    /// Formats the DataRecord as a CSV-formatted line (no newline character).
    /// </summary>
    /// <returns></returns>
    public string ToCSV() {
        // add the front set of universal data
        string CSVLine = $",{Process},{PartNumber},{PartName},{Quantity}";
        // add the inner (variable) data
        foreach (string _value in InnerValues) {
            CSVLine = $",{_value}";
        }
        // add the back set of universal data
        CSVLine = $"{CSVLine},{RecordDate},{RecordTime},{RecordShift},{OperatorID}";
        return CSVLine;
    }

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
        if (SplitLine.Count < 9) {
            throw new RecordParseException();
        }
        // set the universal properties
        string Process = SplitLine[0];
        string PartNumber = SplitLine[1];
        string PartName = SplitLine[2];
        string Quantity = SplitLine[3];
        string RecordDate = SplitLine[^4];
        string RecordTime = SplitLine[^3];
        string RecordShift = SplitLine[^2];
        string OperatorID = SplitLine[^1];
        // get the variable inner fields
        List<string> InnerValues = SplitLine.GetRange(4, SplitLine.Count - 8);
        // attempt to create a DataRecord from the parsed data
        try {
            return new DataRecord(Process, PartNumber, PartName, Quantity, [], InnerValues, RecordDate, RecordTime, RecordShift, OperatorID);
        // there was a problem constructing a DataRecord from the parsed data
        } catch {
            throw new RecordParseException();
        }
    }
}
using LotCoMClient.Models.Exceptions;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Extension of the DataRecord class that represents a Label Print event record.
/// </summary>
/// <param name="Process"></param>
/// <param name="PartNumber"></param>
/// <param name="PartName"></param>
/// <param name="Quantity"></param>
/// <param name="InnerKeys"></param>
/// <param name="InnerValues"></param>
/// <param name="RecordDate"></param>
/// <param name="RecordTime"></param>
/// <param name="RecordShift"></param>
/// <param name="OperatorID"></param>
public partial class PrintRecord(string Process, string PartNumber, string PartName, string Quantity, List<string> InnerKeys, List<string> InnerValues, string RecordDate, string RecordTime, string RecordShift, string OperatorID): DataRecord(Process, PartNumber, PartName, Quantity, InnerKeys, InnerValues, RecordDate, RecordTime, RecordShift, OperatorID) {
    /// <summary>
    /// Converts a DataRecord base class type object into a PrintRecord object (explicit cast).
    /// </summary>
    /// <param name="BaseRecord"></param>
    /// <returns></returns>
    private static PrintRecord ConvertFromBase(DataRecord BaseRecord) {
        return new PrintRecord(BaseRecord.Process, BaseRecord.PartNumber, BaseRecord.PartName, BaseRecord.Quantity, BaseRecord.InnerKeys, BaseRecord.InnerValues, BaseRecord.RecordDate, BaseRecord.RecordTime, BaseRecord.RecordShift, BaseRecord.OperatorID);
    }
    
    /// <summary>
    /// Attempts to parse a DataRecord object from a CSV Line using the RecordParser. 
    /// Casts that DataRecord to a PrintRecord object.
    /// </summary>
    /// <remarks>
    /// Throws RecordParseException if the line contains too few fields, 
    /// if the Parser fails to construct a DataRecord object from the parsed fields, 
    /// of if the parsed DataRecord cannot be cast to a PrintRecord.
    /// </remarks>
    /// <param name="CSVLine"></param>
    /// <returns>A PrintRecord object.</returns>
    /// <exception cref="RecordParseException"></exception>
    public static PrintRecord ParseFromCSV(string CSVLine) {
        // attempt to parse a DataRecord object using the base Parser
        DataRecord BaseRecord;
        try {
            BaseRecord = RecordParser.ParseFromCSV(CSVLine);
        // the CSV Line could not be parsed
        } catch (RecordParseException _ex) {
            throw new RecordParseException($"Failed to parse {CSVLine} due to the following exception:\n{_ex}");
        }
        // cast and return the Parsed DataRecord as a PrintRecord
        try {
            return ConvertFromBase(BaseRecord);
        // the parsed DataRecord object could not be cast to PrintRecord
        } catch (RecordParseException _ex) {
            throw new RecordParseException($"Failed to parse {CSVLine} due to the following exception:\n{_ex}");
        }
    }
}
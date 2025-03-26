using LotCoMClient.Models.Exceptions;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Extension of the DataRecord class that represents a Label Scan event record.
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
public partial class ScanRecord(string Process, string PartNumber, string PartName, string Quantity, List<string> InnerKeys, List<string> InnerValues, string RecordDate, string RecordTime, string RecordShift, string OperatorID): DataRecord(Process, PartNumber, PartName, Quantity, InnerKeys, InnerValues, RecordDate, RecordTime, RecordShift, OperatorID) {
    /// <summary>
    /// Converts a DataRecord base class type object into a ScanRecord object (explicit cast).
    /// </summary>
    /// <param name="BaseRecord"></param>
    /// <returns></returns>
    private static ScanRecord ConvertFromBase(DataRecord BaseRecord) {
        return new ScanRecord(BaseRecord.Process, BaseRecord.PartNumber, BaseRecord.PartName, BaseRecord.Quantity, BaseRecord.InnerKeys, BaseRecord.InnerValues, BaseRecord.RecordDate, BaseRecord.RecordTime, BaseRecord.RecordShift, BaseRecord.OperatorID);
    }
    
    /// <summary>
    /// Attempts to parse a DataRecord object from a CSV Line using the RecordParser. 
    /// Casts that DataRecord to a ScanRecord object.
    /// </summary>
    /// <remarks>
    /// Throws RecordParseException if the line contains too few fields, 
    /// if the Parser fails to construct a DataRecord object from the parsed fields, 
    /// of if the parsed DataRecord cannot be cast to a ScanRecord.
    /// </remarks>
    /// <param name="CSVLine"></param>
    /// <returns>A ScanRecord object.</returns>
    /// <exception cref="RecordParseException"></exception>
    public static ScanRecord ParseFromCSV(string CSVLine) {
        // attempt to parse a DataRecord object using the base Parser
        DataRecord BaseRecord;
        try {
            BaseRecord = RecordParser.ParseFromCSV(CSVLine);
        // the CSV Line could not be parsed
        } catch (RecordParseException _ex) {
            throw new RecordParseException($"Failed to parse {CSVLine} due to the following exception:\n{_ex}");
        }
        // cast and return the Parsed DataRecord as a ScanRecord
        try {
            return ConvertFromBase(BaseRecord);
        // the parsed DataRecord object could not be cast to ScanRecord
        } catch (RecordParseException _ex) {
            throw new RecordParseException($"Failed to parse {CSVLine} due to the following exception:\n{_ex}");
        }
    }
}
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
    /// Attempts to parse a DataRecord object from a CSV Line. Casts that DataRecord to a ScanRecord object.
    /// </summary>
    /// <remarks>
    /// Throws RecordParseException if the line contains too few fields, 
    /// if the Parser fails to construct a DataRecord object from the parsed fields, 
    /// of if the parsed DataRecord cannot be cast to a ScanRecord.
    /// </remarks>
    /// <param name="CSVLine"></param>
    /// <returns>A ScanRecord object.</returns>
    /// <exception cref="RecordParseException"></exception>
    public override ScanRecord ParseFromCSV(string CSVLine) {
        // attempt to parse a DataRecord object using the base Parser
        DataRecord BaseRecord;
        try {
            BaseRecord = base.ParseFromCSV(CSVLine);
        // the CSV Line could not be parsed
        } catch (RecordParseException) {
            throw new RecordParseException();
        }
        // cast and return the Parsed DataRecord as a ScanRecord
        try {
            return (ScanRecord)BaseRecord;
        // the parsed DataRecord object could not be cast to ScanRecord
        } catch {
            throw new RecordParseException();
        }
    }
}
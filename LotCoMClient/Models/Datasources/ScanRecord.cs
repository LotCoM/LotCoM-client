using LotCoMClient.Models.Exceptions;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Extension of the DataRecord class that represents a Label Scan event record.
/// </summary>
/// <param name="RecordProcess">The Process this record belongs to.</param>
/// <param name="RecordPart">The Part assigned to this record.</param>
/// <param name="Quantity">The Quantity assigned to this record.</param>
/// <param name="JBKNumber">The JBK Number assigned to this record (if required for RecordProcess).</param>
/// <param name="LotNumber">The Lot Number assigned to this record (if required for RecordProcess).</param>
/// <param name="DeburrJBKNumber">The Deburr JBK Number assigned to this record (if required for RecordProcess).</param>
/// <param name="DieNumber">The Die Number assigned to this record (if required for RecordProcess).</param>
/// <param name="HeatNumber">The Heat Number assigned to this record (if required for RecordProcess).</param>
/// <param name="RecordDate">The Date assigned to this record.</param>
/// <param name="RecordTime">The Time assigned to this record.</param>
/// <param name="RecordShift">The Shift Number assigned to this record.</param>
/// <param name="OperatorID">The Operator ID assigned to this record.</param>
public partial class ScanRecord(Process RecordProcess, Part RecordPart, string Quantity, string JBKNumber, string LotNumber, string DeburrJBKNumber, string DieNumber, string HeatNumber, string RecordDate, string RecordTime, string RecordShift, string OperatorID): DataRecord(RecordProcess, RecordPart, Quantity, JBKNumber, LotNumber, DeburrJBKNumber, DieNumber, HeatNumber, RecordDate, RecordTime, RecordShift, OperatorID) {
    /// <summary>
    /// Converts a DataRecord base class type object into a ScanRecord object (explicit cast).
    /// </summary>
    /// <param name="BaseRecord"></param>
    /// <returns></returns>
    private static ScanRecord ConvertFromBase(DataRecord BaseRecord) {
        return new ScanRecord(BaseRecord.RecordProcess, BaseRecord.RecordPart, BaseRecord.Quantity, BaseRecord.JBKNumber, BaseRecord.LotNumber, BaseRecord.DeburrJBKNumber, BaseRecord.DieNumber, BaseRecord.HeatNumber, BaseRecord.RecordDate, BaseRecord.RecordTime, BaseRecord.RecordShift, BaseRecord.OperatorID);
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
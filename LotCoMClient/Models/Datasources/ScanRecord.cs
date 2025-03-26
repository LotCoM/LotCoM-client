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
public partial class ScanRecord(string Process, string PartNumber, string PartName, string Quantity, List<string> InnerKeys, List<string> InnerValues, string RecordDate, string RecordTime, string RecordShift, string OperatorID): DataRecord(Process, PartNumber, PartName, Quantity, InnerKeys, InnerValues, RecordDate, RecordTime, RecordShift, OperatorID) {}
using LotComClient.Models.Options;

namespace LotComClient.Models.Datasources;

/// <summary>
/// Extension of the DataRecord class that represents a Label Scan event record.
/// </summary>
/// <param name="RecordProcess">The Process this record belongs to.</param>
/// <param name="RecordPart">The Part assigned to this record.</param>
/// <param name="Quantity">The Quantity assigned to this record.</param>
/// <param name="VariableFields">The VariableFieldSet assigned to this record.</param>
/// <param name="RecordDate">The Date assigned to this record.</param>
/// <param name="RecordTime">The Time assigned to this record.</param>
/// <param name="RecordShift">The Shift Number assigned to this record.</param>
/// <param name="OperatorID">The Operator ID assigned to this record.</param>
/// <param name="ScanAddress">The IP Address of the Scanner producing this record.</param>
/// <param name="ProductionDate">The Production Date of the Record's Label.</param>
/// <param name="ProductionTime">The Production Time of the Record's Label.</param>
public partial class ScanRecord(Process RecordProcess, Part RecordPart, int Quantity, VariableFieldSet VariableFields, string RecordDate, string RecordTime, int RecordShift, string OperatorID, string ScanAddress, string ProductionDate, string ProductionTime): DataRecord(RecordProcess, RecordPart, Quantity, VariableFields, RecordDate, RecordTime, RecordShift, OperatorID, ScanAddress, ProductionDate, ProductionTime) 
{
    /// <summary>
    /// Converts a DataRecord base class type object into a ScanRecord object (explicit cast).
    /// </summary>
    /// <param name="BaseRecord"></param>
    /// <returns></returns>
    public static ScanRecord ConvertFromBase(DataRecord BaseRecord) 
    {
        // confirm there is an IP Address in the base Record object
        if (BaseRecord.ScanAddress is null) 
        {
            throw new ArgumentException($"Cannot convert base DataRecord into a ScanRecord without a non-null 'ScanAddress' property value.");
        }
        return new ScanRecord(BaseRecord.RecordProcess, BaseRecord.RecordPart, BaseRecord.Quantity, BaseRecord.VariableFields, BaseRecord.RecordDate, BaseRecord.RecordTime, BaseRecord.RecordShift, BaseRecord.OperatorID, BaseRecord.ScanAddress, BaseRecord.ProductionDate!, BaseRecord.ProductionTime!);
    }
}
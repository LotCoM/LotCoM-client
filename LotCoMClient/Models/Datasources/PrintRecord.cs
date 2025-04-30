using LotCoMClient.Models.Options;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Extension of the DataRecord class that represents a Label Print event record.
/// </summary>
/// <param name="RecordProcess">The Process this record belongs to.</param>
/// <param name="RecordPart">The Part assigned to this record.</param>
/// <param name="Quantity">The Quantity assigned to this record.</param>
/// <param name="VariableFields">The VariableFieldSet assigned to this record.</param>
/// <param name="RecordDate">The Date assigned to this record.</param>
/// <param name="RecordTime">The Time assigned to this record.</param>
/// <param name="RecordShift">The Shift Number assigned to this record.</param>
/// <param name="OperatorID">The Operator ID assigned to this record.</param>
public partial class PrintRecord(Process RecordProcess, Part RecordPart, int Quantity, VariableFieldSet VariableFields, string RecordDate, string RecordTime, int RecordShift, string OperatorID): DataRecord(RecordProcess, RecordPart, Quantity, VariableFields, RecordDate, RecordTime, RecordShift, OperatorID, null, null, null) 
{
    /// <summary>
    /// Converts a DataRecord base class type object into a PrintRecord object (explicit cast).
    /// </summary>
    /// <param name="BaseRecord"></param>
    /// <returns></returns>
    public static PrintRecord ConvertFromBase(DataRecord BaseRecord) 
    {
        return new PrintRecord(BaseRecord.RecordProcess, BaseRecord.RecordPart, BaseRecord.Quantity, BaseRecord.VariableFields, BaseRecord.RecordDate, BaseRecord.RecordTime, BaseRecord.RecordShift, BaseRecord.OperatorID);
    }
}
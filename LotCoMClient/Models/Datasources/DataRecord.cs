namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Base class for all table entry records in LotCoM database tables.
/// </summary>
/// <param name="RecordProcess">The Process this record belongs to.</param>
/// <param name="RecordPart">The Part assigned to this record.</param>
/// <param name="Quantity">The Quantity assigned to this record.</param>
/// <param name="InnerKeys">A List of Keys to apply to the Values in InnerValues.</param>
/// <param name="InnerValues">A List of Inner Values (variable values from Process to Process) assigned to this record.</param>
/// <param name="RecordDate">The Date assigned to this record.</param>
/// <param name="RecordTime">The Time assigned to this record.</param>
/// <param name="RecordShift">The Shift Number assigned to this record.</param>
/// <param name="OperatorID">The Operator ID assigned to this record.</param>
public class DataRecord(Process RecordProcess, Part RecordPart, string Quantity, List<string> InnerKeys, List<string> InnerValues, string RecordDate, string RecordTime, string RecordShift, string OperatorID) {
    public Process RecordProcess = RecordProcess;
    public Part RecordPart = RecordPart;
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
        string CSVLine = $",{RecordProcess.FullName},{RecordPart.PartNumber},{RecordPart.PartName},{Quantity}";
        // add the inner (variable) data
        foreach (string _value in InnerValues) {
            CSVLine = $",{_value}";
        }
        // add the back set of universal data
        CSVLine = $"{CSVLine},{RecordDate},{RecordTime},{RecordShift},{OperatorID}";
        return CSVLine;
    }
}
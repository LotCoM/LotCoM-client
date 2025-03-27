namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Base class for all table entry records in LotCoM database tables.
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
public class DataRecord(Process RecordProcess, Part RecordPart, string Quantity, string JBKNumber, string LotNumber, string DeburrJBKNumber, string DieNumber, string HeatNumber, string RecordDate, string RecordTime, string RecordShift, string OperatorID) {
    public Process RecordProcess = RecordProcess;
    public Part RecordPart = RecordPart;
    public string Quantity = Quantity;
    public string JBKNumber = JBKNumber;
    public string LotNumber = LotNumber;
    public string DeburrJBKNumber = DeburrJBKNumber;
    public string DieNumber = DieNumber;
    public string HeatNumber = HeatNumber;
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
        // retrieve the RecordProcess' required fields
        List<string> Requirements = RecordProcess.RequiredFields;
        List<string> InnerFields = [JBKNumber, LotNumber, DeburrJBKNumber, DieNumber, HeatNumber];
        // add the inner (variable) data
        foreach (string _field in InnerFields) {
            // only add if the field is required
            if (Requirements.Contains(nameof(_field))) {
                CSVLine = $"{CSVLine},{_field}";
            }
        }
        // add the back set of universal data
        CSVLine = $"{CSVLine},{RecordDate},{RecordTime},{RecordShift},{OperatorID}";
        return CSVLine;
    }
}
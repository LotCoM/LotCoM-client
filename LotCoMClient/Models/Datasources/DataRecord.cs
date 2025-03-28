using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Base class for all table entry records in LotCoM database tables.
/// </summary>
public partial class DataRecord : ObservableObject {
    [ObservableProperty]
    public partial Process RecordProcess {get; set;}
    [ObservableProperty]
    public partial Part RecordPart {get; set;}
    [ObservableProperty]
    public partial string Quantity {get; set;}
    [ObservableProperty]
    public partial string JBKNumber {get; set;}
    [ObservableProperty]
    public partial string LotNumber {get; set;}
    [ObservableProperty]
    public partial string DeburrJBKNumber {get; set;}
    [ObservableProperty]
    public partial string DieNumber {get; set;}
    [ObservableProperty]
    public partial string HeatNumber {get; set;}
    [ObservableProperty]
    public partial string RecordDate {get; set;}
    [ObservableProperty]
    public partial string RecordTime {get; set;}
    [ObservableProperty]
    public partial string RecordShift {get; set;}
    [ObservableProperty]
    public partial string OperatorID {get; set;}

    // "Includes" properties power visibility logic in the DataTablePage View's ListView DataTemplate
    [ObservableProperty]
    public partial bool IncludesJBKNumber {get; set;} = false;
    [ObservableProperty]
    public partial bool IncludesLotNumber {get; set;} = false;
    [ObservableProperty]
    public partial bool IncludesDeburrJBKNumber {get; set;} = false;
    [ObservableProperty]
    public partial bool IncludesDieNumber {get; set;} = false;
    [ObservableProperty]
    public partial bool IncludesHeatNumber {get; set;} = false;

    /// <summary>
    /// Creates a new DataRecord object.
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
    public DataRecord(Process RecordProcess, Part RecordPart, string Quantity, string JBKNumber, string LotNumber, string DeburrJBKNumber, string DieNumber, string HeatNumber, string RecordDate, string RecordTime, string RecordShift, string OperatorID) {
        // set the Record's properties
        this.RecordProcess = RecordProcess;
        this.RecordPart = RecordPart;
        this.Quantity = Quantity;
        this.JBKNumber = JBKNumber;
        this.LotNumber = LotNumber;
        this.DeburrJBKNumber = DeburrJBKNumber;
        this.DieNumber = DieNumber;
        this.HeatNumber = HeatNumber;
        this.RecordDate = RecordDate;
        this.RecordTime = RecordTime;
        this.RecordShift = RecordShift;
        this.OperatorID = OperatorID;
        // configure the Includes flags using the RecordProcess' requirements
        List<string> Requirements = RecordProcess.RequiredFields;
        // configure the JBK flag
        if (Requirements.Contains("JBKNumber")) {
            IncludesJBKNumber = true;
        }
        // configure the Lot flag
        if (Requirements.Contains("LotNumber")) {
            IncludesLotNumber = true;
        }
        // configure the Deburr JBK flag
        if (Requirements.Contains("DeburrJBKNumber")) {
            IncludesDeburrJBKNumber = true;
        }
        // configure the Die flag
        if (Requirements.Contains("DieNumber")) {
            IncludesDieNumber = true;
        }
        // configure the Heat flag
        if (Requirements.Contains("HeatNumber")) {
            IncludesHeatNumber = true;
        }
    }

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
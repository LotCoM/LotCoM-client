using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Base class for all table entry records in LotCoM database tables.
/// </summary>
public partial class DataRecord : ObservableObject 
{
    [ObservableProperty]
    public partial string? ScanAddress {get; set;}
    [ObservableProperty]
    public partial string? ProductionDate {get; set;}
    [ObservableProperty]
    public partial string? ProductionTime {get; set;}
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
    public partial string ModelNumber {get; set;}
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
    public partial bool IncludesModelNumber {get; set;} = false;
    [ObservableProperty]
    public partial bool IncludesHeatNumber {get; set;} = false;
    [ObservableProperty]
    public partial bool IncludesScanAddress {get; set;} = false;
    [ObservableProperty]
    public partial bool IncludesProductionDate {get; set;} = false;

    // these flags control the different display mode for ScanRecords
    [ObservableProperty]
    public partial bool IsScanRecord {get; set;}
    [ObservableProperty]
    public partial bool IsNotScanRecord {get; set;}
    [ObservableProperty]
    public partial string? ProductionTimestamp {get; set;}

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
    /// <param name="ModelNumber">The Model Number assigned to this record (if required for RecordProcess).</param>
    /// <param name="HeatNumber">The Heat Number assigned to this record (if required for RecordProcess).</param>
    /// <param name="RecordDate">The Date assigned to this record.</param>
    /// <param name="RecordTime">The Time assigned to this record.</param>
    /// <param name="RecordShift">The Shift Number assigned to this record.</param>
    /// <param name="OperatorID">The Operator ID assigned to this record.</param>
    /// <param name="ScanAddress">(Optional) the IP Address of the Scanner producing this record. Only applicable to ScanRecords.</param>
    /// <param name="ProductionDate">(Optional) the Production Date of the Record's Label. Only applicable to ScanRecords.</param>
    /// <param name="ProductionTime">(Optional) the Production Time of the Record's Label. Only applicable to ScanRecords.</param>
    public DataRecord(Process RecordProcess, Part RecordPart, string Quantity, string JBKNumber, string LotNumber, string DeburrJBKNumber, string DieNumber, string ModelNumber, string HeatNumber, string RecordDate, string RecordTime, string RecordShift, string OperatorID, string? ScanAddress = null, string? ProductionDate = null, string? ProductionTime = null) 
    {
        // set the Record's properties
        this.RecordProcess = RecordProcess;
        this.RecordPart = RecordPart;
        this.Quantity = Quantity;
        this.JBKNumber = JBKNumber;
        this.LotNumber = LotNumber;
        this.DeburrJBKNumber = DeburrJBKNumber;
        this.DieNumber = DieNumber;
        this.ModelNumber = ModelNumber;
        this.HeatNumber = HeatNumber;
        this.RecordDate = RecordDate;
        this.RecordTime = RecordTime;
        this.RecordShift = RecordShift;
        this.OperatorID = OperatorID;
        this.ScanAddress = ScanAddress;
        this.ProductionDate = ProductionDate;
        this.ProductionTime = ProductionTime;
        // configure the Includes flags using the RecordProcess' requirements
        List<string> Requirements = RecordProcess.RequiredFields;
        IncludesJBKNumber = Requirements.Contains("JBKNumber");
        IncludesLotNumber = Requirements.Contains("LotNumber");
        IncludesDeburrJBKNumber = Requirements.Contains("DeburrJBKNumber");
        IncludesDieNumber = Requirements.Contains("DieNumber");
        IncludesModelNumber = Requirements.Contains("ModelNumber");
        IncludesHeatNumber = Requirements.Contains("HeatNumber");
        IncludesScanAddress = ScanAddress is not null;
        IncludesProductionDate = ProductionDate is not null;
        // configure ScanRecord flags
        IsScanRecord = IncludesScanAddress && IncludesProductionDate;
        if (IsScanRecord)
        {
            ProductionTimestamp = $"{ProductionDate}-{ProductionTime}";
        }
    }

    /// <summary>
    /// Formats the DataRecord as a CSV-formatted line (no newline character).
    /// </summary>
    /// <returns></returns>
    public string ToCSV() 
    {
        // add required Process name
        string CSVLine = $"{RecordProcess.FullName}";
        // add ScanRecord specific data fields
        if (IsScanRecord)
        {
            CSVLine = $"{CSVLine},{RecordDate}-{RecordTime}";
        }
        if (IsScanRecord)
        {
            CSVLine = $"{CSVLine},{ScanAddress}";
        }
        // add universal required fields
        CSVLine = $"{CSVLine},{RecordPart.PartNumber},{RecordPart.PartName},{Quantity}";
        // add the variably-required data fields to the Line
        if (IncludesJBKNumber) 
        {
            CSVLine = $"{CSVLine},{JBKNumber}";
        }
        if (IncludesLotNumber) 
        {
            CSVLine = $"{CSVLine},{LotNumber}";
        }
        if (IncludesDeburrJBKNumber) 
        {
            CSVLine = $"{CSVLine},{DeburrJBKNumber}";
        }
        if (IncludesDieNumber) 
        {
            CSVLine = $"{CSVLine},{DieNumber}";
        }
        if (IncludesModelNumber) 
        {
            CSVLine = $"{CSVLine},{ModelNumber}";
        }
        if (IncludesHeatNumber) 
        {
            CSVLine = $"{CSVLine},{HeatNumber}";
        }
        // add the back set of universal data
        if (IsScanRecord)
        {
            CSVLine = $"{CSVLine},{ProductionDate}-{ProductionTime}";
        }
        else
        {
            CSVLine = $"{CSVLine},{RecordDate}-{RecordTime}";
        }
        CSVLine = $"{CSVLine},{RecordShift},{OperatorID}";
        return CSVLine;
    }
}
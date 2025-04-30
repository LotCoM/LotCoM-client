using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMClient.Models.Options;

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
    public partial int Quantity {get; set;}
    [ObservableProperty]
    public partial VariableFieldSet VariableFields {get; set;}
    [ObservableProperty]
    public partial string RecordDate {get; set;}
    [ObservableProperty]
    public partial string RecordTime {get; set;}
    [ObservableProperty]
    public partial int RecordShift {get; set;}
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
    /// <param name="VariableFields">The VariableFieldSet assigned to this record.</param>
    /// <param name="RecordDate">The Date assigned to this record.</param>
    /// <param name="RecordTime">The Time assigned to this record.</param>
    /// <param name="RecordShift">The Shift Number assigned to this record.</param>
    /// <param name="OperatorID">The Operator ID assigned to this record.</param>
    /// <param name="ScanAddress">(Optional) the IP Address of the Scanner producing this record. Only applicable to ScanRecords.</param>
    /// <param name="ProductionDate">(Optional) the Production Date of the Record's Label. Only applicable to ScanRecords.</param>
    /// <param name="ProductionTime">(Optional) the Production Time of the Record's Label. Only applicable to ScanRecords.</param>
    public DataRecord(Process RecordProcess, Part RecordPart, int Quantity, VariableFieldSet VariableFields, string RecordDate, string RecordTime, int RecordShift, string OperatorID, string? ScanAddress = null, string? ProductionDate = null, string? ProductionTime = null) 
    {
        // set the Record's properties
        this.RecordProcess = RecordProcess;
        this.RecordPart = RecordPart;
        this.Quantity = Quantity;
        this.VariableFields = VariableFields;
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
            CSVLine = $"{CSVLine},{VariableFields.JBKNumber}";
        }
        if (IncludesLotNumber) 
        {
            CSVLine = $"{CSVLine},{VariableFields.LotNumber}";
        }
        if (IncludesDeburrJBKNumber) 
        {
            CSVLine = $"{CSVLine},{VariableFields.DeburrJBKNumber}";
        }
        if (IncludesDieNumber) 
        {
            CSVLine = $"{CSVLine},{VariableFields.DieNumber}";
        }
        if (IncludesModelNumber) 
        {
            CSVLine = $"{CSVLine},{VariableFields.ModelNumber}";
        }
        if (IncludesHeatNumber) 
        {
            CSVLine = $"{CSVLine},{VariableFields.HeatNumber}";
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
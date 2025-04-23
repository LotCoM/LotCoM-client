using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMClient.Models.Datasources;

namespace LotCoMClient.Models.Options;

/// <summary>
/// Encapsulates several options and controls that change the way a DataTablePage looks and functions.
/// </summary>
public partial class DataTablePageOptions() : ObservableObject() 
{
    /// <summary>
    /// Provides default Left Panel widths for Open and Closed states.
    /// </summary>
    public enum LeftPanelWidths 
    {
        Closed = 30,
        Open = 250
    }

    [ObservableProperty]
    /// <summary>
    /// Controls the Page's Title which appears in the top-left corner.
    /// </summary>
    public partial string Title {get; set;} = "";

    [ObservableProperty]
    /// <summary>
    /// Controls the visibility of the Page's Body Header which is between the Page's Title and Body (ListView).
    /// </summary>
    public partial bool IsBodyHeaderLabelShown {get; set;} = false;

    [ObservableProperty]
    /// <summary>
    /// Controls the text of the Page's Body Table Header.
    /// </summary>
    public partial string BodyTableHeaderText {get; set;} = "";

    [ObservableProperty]
    /// <summary>
    /// Controls the visibility of the Page's Navigation Panel which is between the Page's Title and Body (ListView).
    /// </summary>
    public partial bool IsBodyNavigationPanelShown {get; set;} = false;

    [ObservableProperty]
    /// <summary>
    /// Controls the number of DataRecords loaded by the Page's DataTable.
    /// </summary>
    public partial int TotalRecordCount {get; set;} = 0;

    [ObservableProperty]
    /// <summary>
    /// Controls the Number of the currently displayed Page of DataRecords, controlled by the Page's Navigation Panel.
    /// </summary>
    public partial int PageNumber {get; set;} = 0;

    [ObservableProperty]
    /// <summary>
    /// Controls the number of DataRecords displayed by the Page's ListView, controlled by the Page's Navigation Panel.
    /// </summary>
    public partial int DisplayedRecordCount {get; set;} = 0;

    [ObservableProperty]
    /// <summary>
    /// Controls the text shown in the Page's Left Panel Header which appears at the top of the collapsable Left Panel.
    /// </summary>
    public partial string LeftPanelHeaderText {get; set;} = "";

    [ObservableProperty]
    /// <summary>
    /// Controls the text shown in the Page's Left Panel Footer which appears at the bottom of the collapsable Left Panel.
    /// </summary>
    public partial string LeftPanelFooterText {get; set;} = "Click to Collapse";

    [ObservableProperty]
    /// <summary>
    /// Controls the Page's Left Panel Shown state (boolean).
    /// </summary>
    public partial bool IsLeftPanelShown {get; set;} = true;

    [ObservableProperty]
    /// <summary>
    /// Controls the Page's Left Panel Hidden state (boolean).
    /// </summary>
    public partial bool IsLeftPanelHidden {get; set;} = false;

    [ObservableProperty]
    /// <summary>
    /// Controls the width of the Page's Left Panel.
    /// </summary>
    public partial int LeftPanelWidth {get; set;} = (int)LeftPanelWidths.Open;
    
    [ObservableProperty]
    /// <summary>
    /// Controls the data fields that are included in DataRecords for the Page's ListView.
    /// </summary>
    public partial List<string> DataFields {get; set;} = 
    [
        "Part Number", 
        "Part Name", 
        "Quantity", 
        "Production Date", 
        "Production Time", 
        "Production Shift", 
        "Operator ID"
    ];

    [ObservableProperty]
    /// <summary>
    /// Controls the fields that can be used to search the Page's ListView.
    /// </summary>
    public partial List<string> SearchableFields {get; set;} = 
    [
        "All", 
        "Part Number", 
        "Part Name", 
        "Quantity", 
        "Production Date", 
        "Production Time", 
        "Production Shift", 
        "Operator ID"
    ];

    [ObservableProperty]
    /// <summary>
    /// Controls the currently selected index of the Page's SortingField Picker.
    /// </summary>
    public partial int SelectedSortingFieldIndex {get; set;} = -1;

    [ObservableProperty]
    /// <summary>
    /// Controls the currently selected index of the Page's SortingOrder Picker.
    /// </summary>
    public partial int SelectedSortingOrderIndex {get; set;} = -1;

    [ObservableProperty]
    /// <summary>
    /// Controls the currently selected index of the Page's SearchingField Picker.
    /// </summary>
    public partial int SelectedSearchingFieldIndex {get; set;} = -1;

    [ObservableProperty]
    /// <summary>
    /// Controls the currently entered Text value of the Page's ListViewSearchBar.
    /// </summary>
    public partial string SearchTerm {get; set;} = "";

    [ObservableProperty]
    /// <summary>
    /// Controls the Department that defines the Process currently shown on the Page.
    /// </summary>
    public partial Department? Department {get; set;} = null;

    [ObservableProperty]
    /// <summary>
    /// For Selector Pages;
    /// Controls the Processes that are selectable in the Page's PageProcess Picker.
    /// </summary>
    public partial List<Process> DepartmentProcesses {get; set;} = [];

    [ObservableProperty]
    /// <summary>
    /// For Selector Pages;
    /// Controls the currently selected index of the Page's PageProcess Picker.
    /// </summary>
    public partial int SelectedProcessIndex {get; set;} = -1;

    [ObservableProperty]
    /// <summary>
    /// Controls the subclass of DataRecord displayed by the Page's ListView.
    /// </summary>
    public partial Type RecordType {get; set;} = typeof(DataRecord);

    [ObservableProperty]
    /// <summary>
    /// Controls the boolean condition of whether a Process has been assigned to the Page.
    /// </summary>
    public partial bool IsProcessAssigned {get; set;} = false;
}
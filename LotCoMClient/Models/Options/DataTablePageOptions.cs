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

    /// <summary>
    /// Controls the Page's Title which appears in the top-left corner.
    /// </summary>
    [ObservableProperty]
    public partial string Title {get; set;} = "";

    /// <summary>
    /// Controls the visibility of the Page's Body Header which is between the Page's Title and Body (ListView).
    /// </summary>
    [ObservableProperty]
    public partial bool IsBodyHeaderLabelShown {get; set;} = false;

    /// <summary>
    /// Controls the text of the Page's Body Table Header.
    /// </summary>
    [ObservableProperty]
    public partial string BodyTableHeaderText {get; set;} = "";

    /// <summary>
    /// Controls the visibility of the Page's Navigation Panel which is between the Page's Title and Body (ListView).
    /// </summary>
    [ObservableProperty]
    public partial bool IsBodyNavigationPanelShown {get; set;} = false;

    /// <summary>
    /// Controls the Number of the currently displayed Page of DataRecords, controlled by the Page's Navigation Panel.
    /// </summary>
    [ObservableProperty]
    public partial int PageNumber {get; set;} = 0;

    /// <summary>
    /// Controls the currently selected index of the Page's ShownRecordCount Picker.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedShownRecordCountIndex {get; set;} = 1;

    /// <summary>
    /// Controls the number of DataRecords displayed by the Page's ListView, controlled by the Page's Navigation Panel.
    /// </summary>
    [ObservableProperty]
    public partial int ShownRecordCount {get; set;} = 25;

    /// <summary>
    /// Controls the text shown in the Page's Left Panel Header which appears at the top of the collapsable Left Panel.
    /// </summary>
    [ObservableProperty]
    public partial string LeftPanelHeaderText {get; set;} = "";

    /// <summary>
    /// Controls the text shown in the Page's Left Panel Footer which appears at the bottom of the collapsable Left Panel.
    /// </summary>
    [ObservableProperty]
    public partial string LeftPanelFooterText {get; set;} = "Click to Collapse";

    /// <summary>
    /// Controls the Page's Left Panel Shown state (boolean).
    /// </summary>
    [ObservableProperty]
    public partial bool IsLeftPanelShown {get; set;} = true;

    /// <summary>
    /// Controls the Page's Left Panel Hidden state (boolean).
    /// </summary>
    [ObservableProperty]
    public partial bool IsLeftPanelHidden {get; set;} = false;

    /// <summary>
    /// Controls the width of the Page's Left Panel.
    /// </summary>
    [ObservableProperty]
    public partial int LeftPanelWidth {get; set;} = (int)LeftPanelWidths.Open;
    
    /// <summary>
    /// Controls the data fields that are included in DataRecords for the Page's ListView.
    /// </summary>
    [ObservableProperty]
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

    /// <summary>
    /// Controls the fields that can be used to search the Page's ListView.
    /// </summary>
    [ObservableProperty]
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

    /// <summary>
    /// Controls the currently selected index of the Page's SortingField Picker.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedSortingFieldIndex {get; set;} = -1;

    /// <summary>
    /// Controls the currently selected index of the Page's SortingOrder Picker.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedSortingOrderIndex {get; set;} = -1;

    /// <summary>
    /// Controls the currently selected index of the Page's SearchingField Picker.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedSearchingFieldIndex {get; set;} = -1;

    /// <summary>
    /// Controls the currently entered Text value of the Page's ListViewSearchBar.
    /// </summary>
    [ObservableProperty]
    public partial string SearchTerm {get; set;} = "";

    /// <summary>
    /// Controls the Department that defines the Process currently shown on the Page.
    /// </summary>
    [ObservableProperty]
    public partial Department? Department {get; set;} = null;

    /// <summary>
    /// For Selector Pages;
    /// Controls the Processes that are selectable in the Page's PageProcess Picker.
    /// </summary>
    [ObservableProperty]
    public partial List<Process> DepartmentProcesses {get; set;} = [];

    /// <summary>
    /// For Selector Pages;
    /// Controls the currently selected index of the Page's PageProcess Picker.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedProcessIndex {get; set;} = -1;

    /// <summary>
    /// Controls the subclass of DataRecord displayed by the Page's ListView.
    /// </summary>
    [ObservableProperty]
    public partial Type RecordType {get; set;} = typeof(DataRecord);

    /// <summary>
    /// Controls the boolean condition of whether a Process has been assigned to the Page.
    /// </summary>
    [ObservableProperty]
    public partial bool IsProcessAssigned {get; set;} = false;

    /// <summary>
    /// Updates the Options object to use the Page's Body Header Label as the Body Header.
    /// </summary>
    /// <remarks>
    /// Optionally accepts HeaderLabelText which will set the Label's text.
    /// </remarks>
    /// <param name="HeaderLabelText"></param>
    public void SetBodyHeaderModeToLabel(string HeaderLabelText = "")
    {
        IsBodyHeaderLabelShown = true;
        IsBodyNavigationPanelShown = false;
        BodyTableHeaderText = HeaderLabelText;
    }

    /// <summary>
    /// Updates the Options object to use the Page's Navigation Panel as the Body Header.
    /// </summary>
    /// <remarks>
    /// Optionally accepts JumpToPageNumber which will set the current Page Number.
    /// </remarks>
    /// <param name="JumpToPageNumber"></param>
    public void SetBodyHeaderModeToNavigation(int JumpToPageNumber = 1)
    {
        IsBodyHeaderLabelShown = false;
        IsBodyNavigationPanelShown = true;
        PageNumber = JumpToPageNumber;
    }
}
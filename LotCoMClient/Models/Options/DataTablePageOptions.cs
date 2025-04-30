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
    /// Provides default values for most Options.
    /// </summary>
    public static class OptionDefaults
    {
        public const string Title = "";
        public const bool IsBodyHeaderLabelShown = false;
        public const string BodyHeaderText = "";
        public const bool IsBodyNavigationPanelShown = false;
        public const int PageNumber = 1;
        public const int SelectedPageLengthIndex = 1;
        public const int PageLength = 25;
        public const string LeftPanelHeaderText = "";
        public const string LeftPanelFooterText = "Click to Collapse";
        public const bool IsLeftPanelShown = true;
        public const bool IsLeftPanelHidden = false;
        public const int LeftPanelWidth = (int)LeftPanelWidths.Open;
        public const int SelectedSortingFieldIndex = -1;
        public const int SelectedSortingOrderIndex = 1;
        public const int SelectedSearchingFieldIndex = 0;
        public const string SearchTerm = "";
        public const Department? Department = null;
        public const Process? Process = null;
        public const int SelectedProcessIndex = -1;
        public const bool IsProcessAssigned = false;
    }

    /// <summary>
    /// Controls the Page's Title which appears in the top-left corner.
    /// </summary>
    [ObservableProperty]
    public partial string Title {get; set;} = OptionDefaults.Title;

    /// <summary>
    /// Controls the visibility of the Page's Body Header which is between the Page's Title and Body (ListView).
    /// </summary>
    [ObservableProperty]
    public partial bool IsBodyHeaderLabelShown {get; set;} = OptionDefaults.IsBodyHeaderLabelShown;

    /// <summary>
    /// Controls the text of the Page's Body Table Header.
    /// </summary>
    [ObservableProperty]
    public partial string BodyHeaderText {get; set;} = OptionDefaults.BodyHeaderText;

    /// <summary>
    /// Controls the visibility of the Page's Navigation Panel which is between the Page's Title and Body (ListView).
    /// </summary>
    [ObservableProperty]
    public partial bool IsBodyNavigationPanelShown {get; set;} = OptionDefaults.IsBodyNavigationPanelShown;

    /// <summary>
    /// Controls the Number of the currently displayed Page of DataRecords, controlled by the Page's Navigation Panel.
    /// This value is 1-oriented, so Page 1 will refer to index 0 of a PageSet.Pages List.
    /// </summary>
    [ObservableProperty]
    public partial int PageNumber {get; set;} = OptionDefaults.PageNumber;

    /// <summary>
    /// Controls the currently selected index of the Page's PageLength Picker.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedPageLengthIndex {get; set;} = OptionDefaults.SelectedPageLengthIndex;

    /// <summary>
    /// Controls the number of DataRecords displayed by each Page of the DataTablePage's ListView, controlled by the Page's Navigation Panel.
    /// </summary>
    [ObservableProperty]
    public partial int PageLength {get; set;} = OptionDefaults.PageLength;

    /// <summary>
    /// Controls the text shown in the Page's Left Panel Header which appears at the top of the collapsable Left Panel.
    /// </summary>
    [ObservableProperty]
    public partial string LeftPanelHeaderText {get; set;} = OptionDefaults.LeftPanelHeaderText;

    /// <summary>
    /// Controls the text shown in the Page's Left Panel Footer which appears at the bottom of the collapsable Left Panel.
    /// </summary>
    [ObservableProperty]
    public partial string LeftPanelFooterText {get; set;} = OptionDefaults.LeftPanelFooterText;

    /// <summary>
    /// Controls the Page's Left Panel Shown state (boolean).
    /// </summary>
    [ObservableProperty]
    public partial bool IsLeftPanelShown {get; set;} = OptionDefaults.IsLeftPanelShown;

    /// <summary>
    /// Controls the Page's Left Panel Hidden state (boolean).
    /// </summary>
    [ObservableProperty]
    public partial bool IsLeftPanelHidden {get; set;} = OptionDefaults.IsLeftPanelHidden;

    /// <summary>
    /// Controls the width of the Page's Left Panel.
    /// </summary>
    [ObservableProperty]
    public partial int LeftPanelWidth {get; set;} = OptionDefaults.LeftPanelWidth;
    
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
    public partial int SelectedSortingFieldIndex {get; set;} = OptionDefaults.SelectedSortingFieldIndex;

    /// <summary>
    /// Controls the currently selected index of the Page's SortingOrder Picker.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedSortingOrderIndex {get; set;} = OptionDefaults.SelectedSortingOrderIndex;

    /// <summary>
    /// Controls the currently selected index of the Page's SearchingField Picker.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedSearchingFieldIndex {get; set;} = OptionDefaults.SelectedSearchingFieldIndex;

    /// <summary>
    /// Controls the currently entered Text value of the Page's ListViewSearchBar.
    /// </summary>
    [ObservableProperty]
    public partial string SearchTerm {get; set;} = OptionDefaults.SearchTerm;

    /// <summary>
    /// Controls the Department that defines the Process currently shown on the Page.
    /// </summary>
    [ObservableProperty]
    public partial Department? Department {get; set;} = OptionDefaults.Department;

    /// <summary>
    /// For Selector Pages;
    /// Controls the Processes that are selectable in the Page's PageProcess Picker.
    /// </summary>
    [ObservableProperty]
    public partial List<Process> DepartmentProcesses {get; set;} = [];

    /// <summary>
    /// Controls the Process that defines the Parts and Data currently shown on the Page.
    /// </summary>
    [ObservableProperty]
    public partial Process? Process {get; set;} = OptionDefaults.Process;

    /// <summary>
    /// For Selector Pages;
    /// Controls the currently selected index of the Page's PageProcess Picker.
    /// </summary>
    [ObservableProperty]
    public partial int SelectedProcessIndex {get; set;} = OptionDefaults.SelectedProcessIndex;

    /// <summary>
    /// Controls the subclass of DataRecord displayed by the Page's ListView.
    /// </summary>
    [ObservableProperty]
    public partial Type RecordType {get; set;} = typeof(DataRecord);

    /// <summary>
    /// Controls the boolean condition of whether a Process has been assigned to the Page.
    /// </summary>
    [ObservableProperty]
    public partial bool IsProcessAssigned {get; set;} = OptionDefaults.IsProcessAssigned;

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
        BodyHeaderText = HeaderLabelText;
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

    /// <summary>
    /// Updates the Options object's DataFields and SearchableFields properties to provide the Page's Process Requirements.
    /// </summary>
    /// <returns></returns>
    public async Task ConfigurePageFields() 
    {
        await Task.Run(() => 
        {
            // start with the universally included fields
            List<string> Fields = 
            [
                "Part Number", 
                "Part Name", 
                "Quantity", 
                "Production Date", 
                "Production Time", 
                "Production Shift", 
                "Operator ID"
            ];
            if (Process is null)
            {
                return;
            }
            // add the variably-required DataRecord fields from the Page Process requirements
            List<string> Requirements = Process.RequiredFields;
            if (Requirements.Contains("JBKNumber")) 
            {
                Fields.Add("JBK Number");
            }
            if (Requirements.Contains("LotNumber")) 
            {
                Fields.Add("Lot Number");
            }
            if (Requirements.Contains("DeburrJBKNumber")) 
            {
                Fields.Add("Deburr JBK Number");
            }
            if (Requirements.Contains("DieNumber")) 
            {
                Fields.Add("Die Number");
            }
            if (Requirements.Contains("ModelNumber")) 
            {
                Fields.Add("Model Number");
            }
            if (Requirements.Contains("HeatNumber")) 
            {
                Fields.Add("Heat Number");
            }
            // update the DataFields and SearchableFields properties
            DataFields = Fields;
            SearchableFields = Fields
                .Prepend("All")
                .ToList();
        });
    }

    /// <summary>
    /// Configures the Options object's properties to raise and show the Page's Left Panel.
    /// </summary>
    public void RaiseLeftPanel()
    {
        // set the Left Panel properties
        IsLeftPanelShown = false;
        IsLeftPanelHidden = true;
        LeftPanelWidth = (int)LeftPanelWidths.Closed;
    }

    /// <summary>
    /// Configures the Options object's properties to collapse and hide the Page's Left Panel.
    /// </summary>
    public void CollapseLeftPanel() 
    {
        // set the Left Panel properties
    IsLeftPanelShown = true;
    IsLeftPanelHidden = false;
    LeftPanelWidth = (int)LeftPanelWidths.Open;
    }
}
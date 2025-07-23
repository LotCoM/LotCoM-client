using CommunityToolkit.Mvvm.ComponentModel;

namespace LotComClient.ViewModels;

/// <summary>
/// ViewModel (ViewModel Layer) controlling the logic of the HomePage View class.
/// Interacts with the Model Layer to invoke business logic and retrieve data.
/// </summary>
public partial class HomePageViewModel : ObservableObject {
    private string _pageTitle;
    /// <summary>
    /// Serves the Page's Title.
    /// </summary>
    public string PageTitle {
        get {return _pageTitle;}
    }
    private string _leftFramePanelHeader;
    /// <summary>
    /// Serves the Page's Left Frame Panel Header.
    /// </summary>
    public string LeftFramePanelHeader {
        get {return _leftFramePanelHeader;}
    }
    private string _leftFramePanelFooter;
    /// <summary>
    /// Serves the Page's Left Frame Panel Footer.
    /// </summary>
    public string LeftFramePanelFooter {
        get {return _leftFramePanelFooter;}
    }
    
    // UI visual controls
    private bool _leftFrameShown = true;
    /// <summary>
    /// Controls the Shown state of the Left Frame Panel.
    /// </summary>
    public bool LeftFrameShown {
        get {return _leftFrameShown;} 
        set {
            _leftFrameShown = value;
            OnPropertyChanged(nameof(_leftFrameShown));
            OnPropertyChanged(nameof(LeftFrameShown));
        }
    }
    private bool _leftFrameHidden = false;
    /// <summary>
    /// Controls the Hidden state of the Left Frame Panel.
    /// </summary>
    public bool LeftFrameHidden {
        get {return _leftFrameHidden;} 
        set {
            _leftFrameHidden = value;
            OnPropertyChanged(nameof(_leftFrameHidden));
            OnPropertyChanged(nameof(LeftFrameHidden));
        }
    }
    private int _leftFrameWidth = 250;
    /// <summary>
    /// Controls the width of the Left Frame Panel.
    /// 30 = collapsed; 250 = raised.
    /// </summary>
    public int LeftFrameWidth {
        get {return _leftFrameWidth;} 
        set {
            _leftFrameWidth = value;
            OnPropertyChanged(nameof(_leftFrameWidth));
            OnPropertyChanged(nameof(LeftFrameWidth));
        }
    }
    private bool _leftFramePanelBody_PrintingMenuExpanded = false;
    /// <summary>
    /// Controls the expanded state of the Printing Navigation Sub-Menu in the Left Frame Panel.
    /// </summary>
    public bool LeftFramePanelBody_PrintingMenuExpanded {
        get {return _leftFramePanelBody_PrintingMenuExpanded;} 
        set {
            _leftFramePanelBody_PrintingMenuExpanded = value;
            OnPropertyChanged(nameof(_leftFramePanelBody_PrintingMenuExpanded));
            OnPropertyChanged(nameof(LeftFramePanelBody_PrintingMenuExpanded));
        }
    }
    private bool _leftFramePanelBody_ScanningMenuExpanded = false;
    /// <summary>
    /// Controls the expanded state of the Scanning Navigation Sub-Menu in the Left Frame Panel.
    /// </summary>
    public bool LeftFramePanelBody_ScanningMenuExpanded {
        get {return _leftFramePanelBody_ScanningMenuExpanded;} 
        set {
            _leftFramePanelBody_ScanningMenuExpanded = value;
            OnPropertyChanged(nameof(_leftFramePanelBody_ScanningMenuExpanded));
            OnPropertyChanged(nameof(LeftFramePanelBody_ScanningMenuExpanded));
        }
    }
    
    /// <summary>
    /// Creates a ViewModel for the HomePage.
    /// </summary>
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    public HomePageViewModel(string PageTitle) {
        // assign the Page's Title
        _pageTitle = PageTitle;
        // hide the left frame panel by default
        _leftFrameHidden = true;
        _leftFrameShown = false;
        _leftFrameWidth = 30;
        // set the left frame panel's header
        _leftFramePanelHeader = "Menu";
        // set the left frame panel's body content
        // set the left frame panel's footer content
        _leftFramePanelFooter = "Click to Collapse";
    }
}
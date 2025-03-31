using CommunityToolkit.Mvvm.ComponentModel;

namespace LotCoMClient.ViewModels;

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
    public bool LeftFrameShown {
        get {return _leftFrameShown;} 
        set {
            _leftFrameShown = value;
            OnPropertyChanged(nameof(_leftFrameShown));
            OnPropertyChanged(nameof(LeftFrameShown));
        }
    }
    private bool _leftFrameHidden = false;
    public bool LeftFrameHidden {
        get {return _leftFrameHidden;} 
        set {
            _leftFrameHidden = value;
            OnPropertyChanged(nameof(_leftFrameHidden));
            OnPropertyChanged(nameof(LeftFrameHidden));
        }
    }
    private int _leftFrameWidth = 150;
    public int LeftFrameWidth {
        get {return _leftFrameWidth;} 
        set {
            _leftFrameWidth = value;
            OnPropertyChanged(nameof(_leftFrameWidth));
            OnPropertyChanged(nameof(LeftFrameWidth));
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
        _leftFramePanelFooter = "";
    }
}
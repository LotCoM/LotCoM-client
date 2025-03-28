namespace LotCoMClient.Views.Frames;

/// <summary>
/// Code-behind (View Layer) for the PageLeftFrameView custom Control.
/// </summary>
public partial class PageLeftFrameView : ContentView {
    // create a BindableProperty to use as the Frame Panel's Header
    public static readonly BindableProperty FramePanelHeaderProperty = BindableProperty.Create(nameof(FramePanelHeader), typeof(string), typeof(PageTitleFrameView), "Panel Header");
    /// <summary>
    /// The Text shown on the Left Frame Panel's Header.
    /// </summary>
    public string FramePanelHeader {
        get => (string)GetValue(FramePanelHeaderProperty);
    }
    
    // create a BindableProperty to use as the Frame Panel's Footer
    public static readonly BindableProperty FramePanelFooterProperty = BindableProperty.Create(nameof(FramePanelFooter), typeof(string), typeof(PageTitleFrameView), "Panel Footer");
    /// <summary>
    /// The Text shown on the Left Frame Panel's Footer.
    /// </summary>
    public string FramePanelFooter {
        get => (string)GetValue(FramePanelHeaderProperty);
    }

    /// <summary>
    /// Creates a new PageLeftFrame Control.
    /// </summary>
    public PageLeftFrameView() {
        InitializeComponent();
    }
}
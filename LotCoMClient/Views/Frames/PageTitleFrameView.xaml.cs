namespace LotCoMClient.Views.Frames;

/// <summary>
/// Code-behind (View Layer) for the PageTitleFrameView custom Control.
/// </summary>
public partial class PageTitleFrameView : ContentView {
    // create a BindableProperty to use as the Page's Title
    public static readonly BindableProperty PageTitleProperty = BindableProperty.Create(nameof(PageTitle), typeof(string), typeof(PageTitleFrameView), "Page Title");
    /// <summary>
    /// The Title shown on the Page Title Frame.
    /// </summary>
    public string PageTitle {
        get => (string)GetValue(PageTitleProperty);
    }
    // create a BindableProperty to reference the visibility of the Page's Left Frame Panel
    public static readonly BindableProperty PageLeftFrameShownProperty = BindableProperty.Create(nameof(PageLeftFrameShown), typeof(bool), typeof(PageTitleFrameView), false);
    /// <summary>
    /// Allows binding to the visibility of the Page's Left Frame Panel.
    /// </summary>
    public string PageLeftFrameShown {
        get => (string)GetValue(PageLeftFrameShownProperty);
        set => SetValue(PageLeftFrameShownProperty, value);
    }

    /// <summary>
    /// Creates a new PageTitleFrame Control.
    /// </summary>
    public PageTitleFrameView() {
        InitializeComponent();
    }
}
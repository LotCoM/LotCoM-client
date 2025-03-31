namespace LotCoMClient.Views;

/// <summary>
/// Code-behind (View Layer) for the HomePage View.
/// </summary>
public partial class HomePage : ContentPage {
    /// <summary>
    /// ViewModel object controlling the logic of this Page.
    /// </summary>
	private readonly ViewModels.HomePageViewModel _viewModel;

    /// <summary>
    /// Creates a new HomePage.
    /// </summary>
    /// <param name="PageTitle">A string to apply as the Page's Title.</param>
    public HomePage(string PageTitle) {
		// instantiate the ViewModel
        _viewModel = new ViewModels.HomePageViewModel(PageTitle);
        BindingContext = _viewModel;

        // create the page from XAML
		InitializeComponent();

        // invert the collapse left frame panel button's rotation (closed by default)
        PageLeftFrameCollapseButton.Rotation += 180;
    }

    /// <summary>
    /// Handler for the Clicked event from the PageLeftFrameCollapseButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnPageLeftFrameCollapseButtonClicked(object sender, EventArgs e) {
        // the Panel needs to collapse
        if (_viewModel.LeftFrameShown) {
            // set the Left Panel properties in the ViewModel
            _viewModel.LeftFrameShown = false;
            _viewModel.LeftFrameHidden = true;
            // 12 frame animation (150 -> 30 by increments of 10)
            while (_viewModel.LeftFrameWidth > 30) {
                // animate the panel shrinking
                _viewModel.LeftFrameWidth -= 10;
                // animate the collapse button rotating
                PageLeftFrameCollapseButton.Rotation += 15;
                await Task.Delay(1);
            }
        // the Panel needs to raise
        } else {
            // 12 frame animation (30 -> 150 by increments of 10)
            while (_viewModel.LeftFrameWidth < 150) {
                // animate the panel raising
                _viewModel.LeftFrameWidth += 10;
                // animate the collapse button rotating
                PageLeftFrameCollapseButton.Rotation += 15;
                await Task.Delay(1);
            }
            // set the Left Panel properties in the ViewModel
            _viewModel.LeftFrameShown = true;
            _viewModel.LeftFrameHidden = false;
        }
    }

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_PrintingMenuButton control. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_PrintingMenuButtonClicked(object sender, EventArgs e) {
        // invert the Printing Menu Shown property
        if (_viewModel.LeftFramePanelBody_PrintingMenuExpanded) {
            _viewModel.LeftFramePanelBody_PrintingMenuExpanded = false;
        } else {
            _viewModel.LeftFramePanelBody_PrintingMenuExpanded = true;
        }
        await Task.Delay(0);
    }
}
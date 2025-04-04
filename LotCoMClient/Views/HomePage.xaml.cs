using LotCoMClient.Models.Datasources;

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
        await Task.Delay(0);
        // the Panel needs to collapse
        if (_viewModel.LeftFrameShown) {
            // set the Left Panel properties in the ViewModel
            _viewModel.LeftFrameShown = false;
            _viewModel.LeftFrameHidden = true;
            // non-animated collapse
            _viewModel.LeftFrameWidth = 30;
            PageLeftFrameCollapseButton.Rotation += 180;
            // // 12 frame animation (150 -> 30 by increments of 10)
            // while (_viewModel.LeftFrameWidth > 30) {
            //     // animate the panel shrinking
            //     _viewModel.LeftFrameWidth -= 10;
            //     // animate the collapse button rotating
            //     PageLeftFrameCollapseButton.Rotation += 15;
            //     await Task.Delay(1);
            // }
        // the Panel needs to raise
        } else {
            // set the Left Panel properties in the ViewModel
            _viewModel.LeftFrameShown = true;
            _viewModel.LeftFrameHidden = false;
            // non-animated raise
            _viewModel.LeftFrameWidth = 150;
            PageLeftFrameCollapseButton.Rotation += 180;
            // // 12 frame animation (30 -> 150 by increments of 10)
            // while (_viewModel.LeftFrameWidth < 150) {
            //     // animate the panel raising
            //     _viewModel.LeftFrameWidth += 10;
            //     // animate the collapse button rotating
            //     PageLeftFrameCollapseButton.Rotation += 15;
            //     await Task.Delay(1);
            // }
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

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_DiecastPrintingDataButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_DiecastPrintingDataButtonClicked(object sender, EventArgs e) {
        // create a new DataTablePage instance for the Diecast Database Table
        DataTablePage DiecastPage = new DataTablePage("\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints\\4420-DC-Diecast.txt", "Diecast Printing Data", typeof(PrintRecord));
        // push the new Diecast Data Page to the Navigation Stack
        await Navigation.PushAsync(DiecastPage);
    }

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_DeburrPrintingDataButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_DeburrPrintingDataButtonClicked(object sender, EventArgs e) {
        // create a new DataTablePage instance for the Deburr Database Table
        DataTablePage DeburrPage = new DataTablePage("\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\prints\\4470-DC-Deburr.txt", "Deburr Printing Data", typeof(PrintRecord));
        // push the new Deburr Data Page to the Navigation Stack
        await Navigation.PushAsync(DeburrPage);
    }

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_SteelPrintingDataButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_SteelPrintingDataButtonClicked(object sender, EventArgs e) {
        // create a new DataTablePage instance for the Steel Database Table
        DataTableSelectorPage SteelPage = new DataTableSelectorPage("", "Steel Printing Data", "Steel", typeof(PrintRecord), false);
        // push the new Steel Data Page to the Navigation Stack
        await Navigation.PushAsync(SteelPage);
    }

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_AluminumPrintingDataButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_AluminumPrintingDataButtonClicked(object sender, EventArgs e) {
        // create a new DataTablePage instance for the Aluminum Database Table
        DataTableSelectorPage AluminumPage = new DataTableSelectorPage("", "Aluminum Printing Data", "Aluminum", typeof(PrintRecord), false);
        // push the new Aluminum Data Page to the Navigation Stack
        await Navigation.PushAsync(AluminumPage);
    }

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_ScanningMenuButton control. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_ScanningMenuButtonClicked(object sender, EventArgs e) {
        // invert the Scanning Menu Shown property
        if (_viewModel.LeftFramePanelBody_PrintingMenuExpanded) {
            _viewModel.LeftFramePanelBody_PrintingMenuExpanded = false;
        } else {
            _viewModel.LeftFramePanelBody_PrintingMenuExpanded = true;
        }
        await Task.Delay(0);
    }

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_DiecastScanningDataButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_DiecastScanningDataButtonClicked(object sender, EventArgs e) {
        // create a new DataTablePage instance for the Diecast Database Table
        DataTablePage DiecastPage = new DataTablePage("\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\scans\\4420-DC-Diecast.txt", "Diecast Scanning Data", typeof(ScanRecord));
        // push the new Diecast Data Page to the Navigation Stack
        await Navigation.PushAsync(DiecastPage);
    }

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_DeburrScanningDataButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_DeburrScanningDataButtonClicked(object sender, EventArgs e) {
        // create a new DataTablePage instance for the Deburr Database Table
        DataTablePage DeburrPage = new DataTablePage("\\\\144.133.122.1\\Lot Control Management\\Database\\data_tables\\scans\\4470-DC-Deburr.txt", "Deburr Scanning Data", typeof(ScanRecord));
        // push the new Deburr Data Page to the Navigation Stack
        await Navigation.PushAsync(DeburrPage);
    }

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_SteelScanningDataButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_SteelScanningDataButtonClicked(object sender, EventArgs e) {
        // create a new DataTablePage instance for the Steel Database Table
        DataTableSelectorPage SteelPage = new DataTableSelectorPage("", "Steel Scanning Data", "Steel", typeof(ScanRecord), false);
        // push the new Steel Data Page to the Navigation Stack
        await Navigation.PushAsync(SteelPage);
    }

    /// <summary>
    /// Handler for the Clicked event from the LeftFramePanelBody_AluminumScanningDataButton control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnLeftFramePanelBody_AluminumScanningDataButtonClicked(object sender, EventArgs e) {
        // create a new DataTablePage instance for the Aluminum Database Table
        DataTableSelectorPage AluminumPage = new DataTableSelectorPage("", "Aluminum Scanning Data", "Aluminum", typeof(ScanRecord), false);
        // push the new Aluminum Data Page to the Navigation Stack
        await Navigation.PushAsync(AluminumPage);
    }
}
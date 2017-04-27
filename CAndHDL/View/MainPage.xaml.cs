using CAndHDL.ViewModel;
using Windows.UI.Xaml.Controls;

namespace CAndHDL
{
    /// <summary>
    /// Main page
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>Page ViewModel</summary>
        public MainPageViewModel MainPageViewModel { get; set; } = new MainPageViewModel();

        /// <summary>Default constructor</summary>
        public MainPage()
        {
            this.InitializeComponent();
            MainPageViewModel.Init();
        }
    }
}

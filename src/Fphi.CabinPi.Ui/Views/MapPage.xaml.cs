using System;

using Fphi.CabinPi.Ui.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fphi.CabinPi.Ui.Views
{
    public sealed partial class MapPage : Page
    {
        public MapViewModel ViewModel => ViewModelLocator.Current.Map;

        public MapPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.InitializeAsync(mapControl);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Cleanup();
        }
    }
}

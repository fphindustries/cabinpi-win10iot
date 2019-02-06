using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Fphi.CabinPi.Ui.ViewModels;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Uwp;

using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Fphi.CabinPi.Ui.Views
{
    public sealed partial class MainPage : Page
    {

        public MainViewModel ViewModel => this.DataContext as MainViewModel;
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }

        private void UpdateOnclick(object sender, RoutedEventArgs e)
        {
            TimePowerChart.Update(true);
        }

    }
}


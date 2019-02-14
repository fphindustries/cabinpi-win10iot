using System;

using Fphi.Cabin.Pi.UI2.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fphi.Cabin.Pi.UI2.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return ViewModelLocator.Current.MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        private void UpdateOnclick(object sender, RoutedEventArgs e)
        {
            //TimePowerChart.Update(true);
        }
    }
}

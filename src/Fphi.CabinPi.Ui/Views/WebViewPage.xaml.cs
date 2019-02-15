using System;

using Fphi.CabinPi.Ui.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Fphi.CabinPi.Ui.Views
{
    public sealed partial class WebViewPage : Page
    {
        public WebViewViewModel ViewModel { get; } = new WebViewViewModel();

        public WebViewPage()
        {
            InitializeComponent();
            ViewModel.Initialize(webView);
        }
    }
}

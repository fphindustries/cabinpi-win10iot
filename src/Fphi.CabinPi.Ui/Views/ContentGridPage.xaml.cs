using System;

using Fphi.CabinPi.Ui.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Fphi.CabinPi.Ui.Views
{
    public sealed partial class ContentGridPage : Page
    {
        public ContentGridViewModel ViewModel { get; } = new ContentGridViewModel();

        public ContentGridPage()
        {
            InitializeComponent();
        }
    }
}

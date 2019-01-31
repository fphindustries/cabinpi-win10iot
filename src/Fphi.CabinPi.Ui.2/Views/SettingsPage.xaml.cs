using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Fphi.CabinPi.Ui.Helpers;
using Fphi.CabinPi.Ui.Services;
using Fphi.CabinPi.Ui.ViewModels;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fphi.CabinPi.Ui.Views
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel => this.DataContext as SettingsViewModel;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }
    }
}

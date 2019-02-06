﻿using System;
using System.Threading.Tasks;

using Fphi.Cabin.Pi.UI2.Services;
using Fphi.Cabin.Pi.UI2.ViewModels;

using Windows.ApplicationModel.Activation;

namespace Fphi.Cabin.Pi.UI2.Activation
{
    internal class DefaultLaunchActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private readonly string _navElement;

        public NavigationServiceEx NavigationService => ViewModelLocator.Current.NavigationService;

        public DefaultLaunchActivationHandler(Type navElement)
        {
            _navElement = navElement.FullName;
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            // When the navigation stack isn't restored, navigate to the first page and configure
            // the new page by passing required information in the navigation parameter
            NavigationService.Navigate(_navElement, args.Arguments);

            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return NavigationService.Frame.Content == null;
        }
    }
}

using Fphi.CabinPi.Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace Fphi.CabinPi.Ui.Services
{
    public class AppService
    {
        private AppServiceConnection _backgroundAppService;

        public AppService()
        {

        }


        public async Task SetupAppServiceAsync()
        {
            // find the installed application(s) that expose the app service PerimeterBreachService
            var listing = await AppServiceCatalog.FindAppServiceProvidersAsync("CabinPiAppService");
            var packageName = "";
            // there may be cases where other applications could expose the same App Service Name, in our case
            // we only have the one
            if (listing.Count == 1)
            {
                packageName = listing[0].PackageFamilyName;
            }
            _backgroundAppService = new AppServiceConnection();
            _backgroundAppService.AppServiceName = "CabinPiAppService";
            _backgroundAppService.PackageFamilyName = packageName;
            //open app service connection
            var status = await _backgroundAppService.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                //something went wrong
                Debug.WriteLine("Could not connect to the App Service: " + status.ToString());
            }
            else
            {
                //add handler to receive app service messages (Perimiter messages)
                _backgroundAppService.RequestReceived += BackgroundServiceRequestReceived;
            }
        }


        private async void BackgroundServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {

            foreach (var messageKey in args.Request.Message.Keys)
            {
                Debug.WriteLine($"UI Task RequestReceived: {messageKey}");
                switch (messageKey)
                {
                    case AppServiceMessages.Configuration:
                        //Send current configuration
                        var configuration = JsonConvert.DeserializeObject<BackgroundConfiguration>(args.Request.Message[messageKey].ToString());
                        //TODO: Put the configuration somewhere where it can be bound to the settings page
                        break;

                    default:
                        break;
                }
            }
        }

        public async Task RequestConfigurationAsync()
        {
            ValueSet message = new ValueSet();
            message.Add(AppServiceMessages.RequestConfiguration, null);
            await _backgroundAppService.SendMessageAsync(message);
        }
    }
}

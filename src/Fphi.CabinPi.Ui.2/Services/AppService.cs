using Fphi.CabinPi.Common;
using Fphi.CabinPi.Ui.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Core;

namespace Fphi.CabinPi.Ui.Services
{
    public class AppService : Observable
    {
        private AppServiceConnection _backgroundAppService;
        private readonly DataService _dataService;

        public AppService(DataService dataService)
        {
            _dataService = dataService;
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
                        await ValidateConfigurationAsync(configuration);
                        break;

                    default:
                        break;
                }
            }
        }

        public async Task SendConfigurationAsync()
        {
            var config = new BackgroundConfiguration();
            config.Sensors = _dataService.SensorConfigurations.ToList();

            ValueSet message = new ValueSet();
            message.Add(AppServiceMessages.Configuration, JsonConvert.SerializeObject(config));
            await _backgroundAppService.SendMessageAsync(message);
        }

        public async Task RequestConfigurationAsync()
        {
            ValueSet message = new ValueSet();
            message.Add(AppServiceMessages.RequestConfiguration, null);
            await _backgroundAppService.SendMessageAsync(message);
        }

        /// <summary>
        /// Verifies that the configuration contains all known sensors. During first start up
        /// or after new sensors are added the configuration will be out of date.
        /// </summary>
        /// <param name="configuration"></param>
        private async Task ValidateConfigurationAsync(BackgroundConfiguration configuration)
        {
            List<SensorConfiguration> knownSensors = new List<SensorConfiguration>()
            {
                new SensorConfiguration{ Enabled=false, SensorId= SensorId.Sht31d, SensorReading= SensorReading.InteriorTemperaturAndHumidity },
                new SensorConfiguration{ Enabled=false, SensorId= SensorId.FakeSht31d, SensorReading= SensorReading.InteriorTemperaturAndHumidity }
            };

            foreach (var knownSensor in knownSensors)
            {
                if (!configuration.Sensors.Any(s => s.SensorId == knownSensor.SensorId))
                {
                    configuration.Sensors.Add(knownSensor);
                }
            }

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 _dataService.SensorConfigurations.Clear();
                 foreach (var sensorConfig in configuration.Sensors)
                 {
                     _dataService.SensorConfigurations.Add(sensorConfig);
                 }
             });
        }

    }
}

using Fphi.Cabin.Pi.Common.Models;
using Fphi.Cabin.Pi.Common.Services;
using Fphi.CabinPi.Common;
using Fphi.CabinPi.Common.Helpers;
using Fphi.CabinPi.Common.Models;
using Fphi.CabinPi.Ui.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Fphi.CabinPi.Common.Services;

namespace Fphi.CabinPi.Ui.Services
{


    public class SensorService : Observable, ISensorService
    {
        private AppServiceConnection _backgroundAppService;
        private readonly DataService _dataService;

        private ObservableCollection<SensorConfiguration> _sensorConfigurations = new ObservableCollection<SensorConfiguration>();

        public ObservableCollection<SensorConfiguration> SensorConfigurations
        {
            get { return _sensorConfigurations; }
            set { Set(ref _sensorConfigurations, value); }
        }

        private bool _connected;

        public bool Connected
        {
            get { return _connected; }
            set { Set(ref _connected, value); }
        }

        private SensorReading _interiorTemperatureF = new SensorReading()
            {Sensor = SensorId.FakeSht31d, Type = SensorType.InteriorTemperatureF, Value = 0};

        public SensorReading InteriorTemperatureF
        {
            get { return _interiorTemperatureF; }
            set { Set(ref _interiorTemperatureF, value); }
        }

        private SensorReading _interiorHumidity;


        public SensorReading InteriorHumidity
        {
            get { return _interiorHumidity; }
            set { Set(ref _interiorHumidity, value); }
        }



        public SensorService(DataService dataService)
        {
            _dataService = dataService;
            _connected = false;
        }

        public event EventHandler<SensorReadingReceivedEventArgs> SensorReadingReceived;

        protected virtual void OnSensorReadingReceived(SensorReadingReceivedEventArgs e)
        {
            SensorReadingReceived?.Invoke(this, e);
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
                Connected = true;
            }
        }

        public double GetReading(SensorType type)
        {
            switch(type)
            {
                case SensorType.InteriorTemperatureF:
                    return  InteriorTemperatureF.Value;
                default:
                    return 50;
                    
            }
        }

        private async void BackgroundServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {

            foreach (var messageKey in args.Request.Message.Keys)
            {
                Debug.WriteLine($"UI Task RequestReceived: {messageKey}");
                switch (messageKey)
                {
                    case SensorServiceMessages.Configuration:
                        //Send current configuration
                        var configuration = JsonConvert.DeserializeObject<BackgroundConfiguration>(args.Request.Message[messageKey].ToString());
                        await ValidateConfigurationAsync(configuration);
                        break;
                    case SensorServiceMessages.SensorReading:
                        var sensorReading = JsonConvert.DeserializeObject<SensorReading>(args.Request.Message[messageKey].ToString());
                        switch (sensorReading.Type)
                        {
                            case SensorType.InteriorTemperatureC:
                                break;
                            case SensorType.InteriorTemperatureF:
                                InteriorTemperatureF = sensorReading;
                                break;
                            case SensorType.InteriorHumidity:
                                InteriorHumidity = sensorReading;
                                break;
                            default:
                                break;
                        }
                        OnSensorReadingReceived(new SensorReadingReceivedEventArgs(sensorReading));
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task SendConfigurationAsync()
        {
            if (Connected)
            {
                var config = new BackgroundConfiguration();
                config.Sensors = SensorConfigurations.ToList();

                ValueSet message = new ValueSet();
                message.Add(SensorServiceMessages.Configuration, JsonConvert.SerializeObject(config));
                await _backgroundAppService.SendMessageAsync(message);
            }
        }

        public async Task RequestConfigurationAsync()
        {
            if (Connected)
            {
                ValueSet message = new ValueSet();
                message.Add(SensorServiceMessages.RequestConfiguration, null);
                await _backgroundAppService.SendMessageAsync(message);
            }
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
                new SensorConfiguration{ Enabled=false, SensorId= SensorId.Sht31d, SensorCapability= SensorCapability.InteriorTemperatureAndHumidity },
                new SensorConfiguration{ Enabled=false, SensorId= SensorId.FakeSht31d, SensorCapability= SensorCapability.InteriorTemperatureAndHumidity },
                new SensorConfiguration{ Enabled=false, SensorId= SensorId.INA219, SensorCapability= SensorCapability.PowerConsumption }
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
                SensorConfigurations.Clear();
                foreach (var sensorConfig in configuration.Sensors)
                {
                    SensorConfigurations.Add(sensorConfig);
                }
            });
        }

    }
}

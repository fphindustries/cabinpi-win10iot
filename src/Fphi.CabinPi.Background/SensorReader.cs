using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fphi.CabinPi.Background.Fakes;
using Fphi.CabinPi.Background.Sensors;
using Fphi.CabinPi.Common.Models;

namespace Fphi.CabinPi.Background
{
    /// <summary>
    /// Responsible for reading sensor data and writing it to the data repository
    /// </summary>
    class SensorReader
    {
        private readonly ISensorFactory _sensorFactory;
        private readonly ISensorDataStore _dataStore;
        private readonly ConfigurationService _configurationService;
        private List<ISensor> _sensors;
        private object reading;

        public SensorReader(ISensorFactory sensorFactory, ISensorDataStore dataStore, ConfigurationService configurationService)
        {
            _sensorFactory = sensorFactory;
            _dataStore = dataStore;
            _configurationService = configurationService;
            //_sensors = _sensorFactory.GetSensors();

            _configurationService.PropertyChanged += ConfigurationChanged;
        }

        public async Task InitializeAsync()
        {
            await BuildSensorsFromConfiguration();
        }

        private async Task BuildSensorsFromConfiguration()
        {
            _sensors = new List<ISensor>();
            foreach(var sensorConfiguration in _configurationService.BackgroundConfiguration.Sensors.Where(s => s.Enabled))
            {
                ISensor newSensor = null;
                switch (sensorConfiguration.SensorId)
                {
                    case Common.SensorId.Sht31d:
                        newSensor = new SHT31dSensor() { Name = sensorConfiguration.Name };
                        break;
                    case Common.SensorId.FakeSht31d:
                        newSensor = new FakeSHT31d() { Name = sensorConfiguration.Name };
                        break;
                    case Common.SensorId.INA219:
                        newSensor = new INA219Sensor(.1) { Name = sensorConfiguration.Name };
                        break;
                    default:
                        break;
                }
                if(newSensor != null)
                {
                    await newSensor.InitializeAsync();
                    _sensors.Add(newSensor);
                }
            }
        }

        private async void ConfigurationChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine("Configuration Changed");
            await BuildSensorsFromConfiguration();
        }

        public async Task ReadSensors()
        {
            foreach(var sensor in _sensors)
            {
                var readings = await sensor.GetReadingsAsync();

                foreach(var reading in readings)
                {
                    _dataStore.WriteSensorReading(reading);
                }
            }
        }

        public IEnumerable<IEnumerable<SensorReading>> GetAllReadings()
        {
            foreach (var sensor in _sensors)
            {
                yield return sensor.GetReadingsAsync().Result;
            }
        }
    }
}

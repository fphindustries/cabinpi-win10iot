using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Background
{
    /// <summary>
    /// Responsible for reading sensor data and writing it to the data repository
    /// </summary>
    class SensorReader
    {
        private readonly ISensorFactory _sensorFactory;
        private readonly ISensorDataStore _dataStore;
        private readonly IEnumerable<ISensor> _sensors;

        public SensorReader(ISensorFactory sensorFactory, ISensorDataStore dataStore)
        {
            _sensorFactory = sensorFactory;
            _dataStore = dataStore;

            _sensors = _sensorFactory.GetSensors();
        }

        public async Task ReadSensors()
        {
            foreach(var sensor in _sensors)
            {
                var readings = await sensor.GetReadings();

                foreach(var reading in readings)
                {
                    _dataStore.WriteSensorReading(reading);
                }
            }
        }
    }
}

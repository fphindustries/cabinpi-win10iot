using System;
using System.Collections.Generic;
using System.Text;

namespace Fphi.Cabin.Pi.Common.Models
{
    public class Sensors
    {
        public enum SensorId
        {
            Sht31d = 1,
            FakeSht31d = 100
        }

        public enum SensorReadingTypes
        {
            InteriorTemperaturAndHumidity,
            ExteriorTemperature,
            InteriorTemperatureAndPressure,
            PowerConsumption,
            SolarPower
        }
    }
}

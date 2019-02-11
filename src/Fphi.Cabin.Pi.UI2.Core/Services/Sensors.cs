using System;
using System.Collections.Generic;
using System.Text;

namespace Fphi.Cabin.Pi.UI2.Core.Models
{
    public class Sensors
    {
        public enum SensorId
        {
            Sht31d = 1,
            FakeSht31d = 100
        }

        public enum SensorReading
        {
            InteriorTemperaturAndHumidity,
            ExteriorTemperature,
            InteriorTemperatureAndPressure,
            PowerConsumption,
            SolarPower
        }
    }
}

using Fphi.CabinPi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.Cabin.Pi.Common.Models
{
    public class SensorConfiguration
    {
        public SensorId SensorId { get; set; }
        public SensorReading SensorReading { get; set; }
        public bool Enabled { get; set; }

        public string Name
        {
            get
            {
                switch (SensorId)
                {
                    case SensorId.Sht31d:
                        return "SHT31d";
                    case SensorId.FakeSht31d:
                        return "Fake SHT31d";
                    default:
                        return "Unknown Sensor";
                }
            }
        }

        public string SensorReadingName
        {
            get
            {
                switch (SensorReading)
                {
                    case SensorReading.InteriorTemperaturAndHumidity:
                        return "Interior Temperature and Humidity";
                    case SensorReading.ExteriorTemperature:
                        return "Exterior Temperature";
                    case SensorReading.InteriorTemperatureAndPressure:
                        return "Interior Temperature and Pressure";
                    case SensorReading.PowerConsumption:
                        return "Power Consumption";
                    case SensorReading.SolarPower:
                        return "Solar Power";
                    default:
                        return "Unknown Reading";
                }
            }
        }
    }
}

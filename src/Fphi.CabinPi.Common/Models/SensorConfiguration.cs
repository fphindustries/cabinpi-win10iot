using Fphi.CabinPi.Common;
using Fphi.CabinPi.Common.Models;
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
        public SensorCapability SensorCapability { get; set; }
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
                    case SensorId.INA219:
                        return "INA219";
                    case SensorId.FakeINA219:
                        return "Fake INA219";
                    case SensorId.BMP388:
                        return "BMP388";
                    default:
                        return "Unknown Sensor";
                }
            }
        }

        public string SensorCapabilityName
        {
            get
            {
                switch (SensorCapability)
                {
                    case SensorCapability.InteriorTemperatureAndHumidity:
                        return "Interior Temperature and Humidity";
                    case SensorCapability.ExteriorTemperature:
                        return "Exterior Temperature";
                    case SensorCapability.InteriorTemperatureAndPressure:
                        return "Interior Temperature and Pressure";
                    case SensorCapability.PowerConsumption:
                        return "Power Consumption";
                    case SensorCapability.SolarPower:
                        return "Solar Power";
                    default:
                        return "Unknown Reading";
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Common
{
    public enum SensorId
    {
        Sht31d=1,
        FakeSht31d=100
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

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
        FakeSht31d=101,
        INA219=2,
        FakeINA219=102,
        BMP388=3
    }

    public enum SensorType
    {
        InteriorTemperatureC,
        InteriorTemperatureF,
        InteriorHumidity,
        PressurePascals,
        PressureInHg,
        BusVoltage,
        Current,
        Power,
        ShuntVoltage,
        SupplyVoltage
    }

    public enum SensorCapability
    {
        InteriorTemperatureAndHumidity,
        ExteriorTemperature,
        InteriorTemperatureAndPressure,
        PowerConsumption,
        SolarPower
    }
}

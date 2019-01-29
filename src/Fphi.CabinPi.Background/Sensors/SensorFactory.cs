using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Background.Sensors
{
    class SensorFactory : ISensorFactory
    {
        public IEnumerable<ISensor> GetSensors()
        {
            return new ISensor[] { new SHT31dSensor() };
        }
    }
}

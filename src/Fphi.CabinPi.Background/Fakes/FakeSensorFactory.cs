using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Background.Fakes
{
    class FakeSensorFactory : ISensorFactory
    {
        public IEnumerable<ISensor> GetSensors()
        {
            return new ISensor[] { new FakeSHT31d() };
        }
    }
}

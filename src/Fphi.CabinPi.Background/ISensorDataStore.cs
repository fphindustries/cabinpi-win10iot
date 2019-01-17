using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Background
{
    interface ISensorDataStore
    {
        void WriteSensorReading(SensorReading reading);
    }
}

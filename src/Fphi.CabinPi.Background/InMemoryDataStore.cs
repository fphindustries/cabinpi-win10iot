using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Background
{
    class InMemoryDataStore : ISensorDataStore
    {
        public void WriteSensorReading(SensorReading reading)
        {
            Debug.WriteLine(reading);
        }
    }
}

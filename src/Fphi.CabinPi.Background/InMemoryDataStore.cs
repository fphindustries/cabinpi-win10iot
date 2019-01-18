using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace Fphi.CabinPi.Background
{
    class InMemoryDataStore : ISensorDataStore
    {
        public async void WriteSensorReading(SensorReading reading)
        {
            Debug.WriteLine(reading);
            var messages = new ValueSet();
            messages.Add(reading.Name, reading.Value);
            await AppServiceTask.BroadcastMessage(messages);
        }
    }
}

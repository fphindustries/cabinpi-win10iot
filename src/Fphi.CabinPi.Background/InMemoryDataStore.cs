using Fphi.CabinPi.Common;
using Fphi.CabinPi.Common.Models;
using Newtonsoft.Json;
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
            messages.Add(SensorServiceMessages.SensorReading, JsonConvert.SerializeObject(reading));
            await AppServiceTask.BroadcastMessage(messages);
        }
    }
}

using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Background.Fakes
{
    class FakeINA219 : ISensor
    {
        public string Name { get; set; }
        public async Task<IEnumerable<SensorReading>> GetReadingsAsync()
        {
            Random rng = new Random();
            return new SensorReading[] {
                new SensorReading
                {
                    Type= Common.SensorType.BusVoltage,
                     Sensor= Common.SensorId.FakeINA219,
                     Time=DateTime.UtcNow,
                     Value=rng.NextDouble() + 2.5D
                },
                new SensorReading
                {
                    Type= Common.SensorType.Current,
                     Sensor= Common.SensorId.FakeINA219,
                     Time=DateTime.UtcNow,
                     Value=rng.NextDouble() + 1D
                },
                new SensorReading
                {
                    Type= Common.SensorType.Power,
                     Sensor= Common.SensorId.FakeINA219,
                     Time=DateTime.UtcNow,
                     Value=rng.NextDouble() + 1D
                },
                new SensorReading
                {
                    Type= Common.SensorType.ShuntVoltage,
                     Sensor= Common.SensorId.FakeINA219,
                     Time=DateTime.UtcNow,
                     Value=rng.NextDouble() + 1D
                },
                new SensorReading
                {
                    Type= Common.SensorType.SupplyVoltage,
                     Sensor= Common.SensorId.FakeINA219,
                     Time=DateTime.UtcNow,
                     Value=rng.NextDouble() + 2.5D
                }
            };
        }

        public async Task InitializeAsync()
        {
        }
    }
}

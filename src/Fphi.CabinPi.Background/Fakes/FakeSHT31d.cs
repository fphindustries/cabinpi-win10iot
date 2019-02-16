using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Background.Fakes
{
    class FakeSHT31d : ISensor
    {
        public string Name { get; set; }
        public async Task<IEnumerable<SensorReading>> GetReadings()
        {
            Random rng = new Random();
            return new SensorReading[] {
                new SensorReading
                {
                    Sensor= Common.SensorId.FakeSht31d,
                    SensorReadingType= Common.SensorReadingType.InteriorTemperatureF,
                    Value=rng.NextDouble() * 20D + 50D,
                    Time = DateTimeOffset.UtcNow
                },
                new SensorReading
                {
                    Sensor= Common.SensorId.FakeSht31d,
                    SensorReadingType= Common.SensorReadingType.InteriorTemperatureC,
                    Value=rng.NextDouble() * 20D + 50D,
                    Time = DateTimeOffset.UtcNow
                },
                new SensorReading
                {
                    Sensor= Common.SensorId.FakeSht31d,
                    SensorReadingType= Common.SensorReadingType.InteriorHumidity,
                    Value=rng.NextDouble() * 20D + 50D,
                    Time = DateTimeOffset.UtcNow
                }
            };
        }
    }
}

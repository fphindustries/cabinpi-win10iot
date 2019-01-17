using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Background.Fakes
{
    class FakeSHT31d : ISensor
    {
        public IEnumerable<SensorReading> GetReadings()
        {
            Random rng = new Random();
            return new SensorReading[] {
                new SensorReading
                {
                    Sensor=nameof(FakeSHT31d),
                    Name="InteriorTempF",
                    Value=rng.NextDouble() * 20D + 50D,
                    Time = DateTimeOffset.UtcNow
                }
            };
        }
    }
}

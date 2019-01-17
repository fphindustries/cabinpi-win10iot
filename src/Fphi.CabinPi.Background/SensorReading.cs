using System;

namespace Fphi.CabinPi.Background
{
    class SensorReading
    {
        public string Sensor { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public DateTimeOffset Time { get; set; }


        public override string ToString()
        {
            return $"Sensor {Sensor} : {Name}={Value} @ {Time}";
        }
    }
}
using System;


namespace Fphi.CabinPi.Common.Models
{
    public class SensorReading
    {
        public SensorId Sensor { get; set; }
        public SensorReadingType SensorReadingType { get; set; }
        public double Value { get; set; }
        public DateTimeOffset Time { get; set; }


        public override string ToString()
        {
            return $"Sensor {Sensor} : {SensorReadingType}={Value} @ {Time}";
        }
    }
}

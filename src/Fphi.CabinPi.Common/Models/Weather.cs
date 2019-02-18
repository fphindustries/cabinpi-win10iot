using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fphi.Cabin.Pi.Common.Models;

namespace Fphi.CabinPi.Common.Models
{
    public enum WeatherType
    {
        Raining,
        Clouding,
        Snowing,
        Sunning
    }

    public class Weather
    {
        public WeatherType Type { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public override string ToString()
        {
            return base.ToString();
        }
        public static Weather GetTemperature(WeatherType type)
        {
            var t = new Weather
            {
                Description = GetDescription(type),
                Type = type,
                Image = GetImage(type)
            };
            return t;
        }

        public static string GetDescription(WeatherType type)
        {
            switch (type)
            {
                default:
                    return "Crap...I don't know sorry";
            }
        }


        public static string GetImage(WeatherType type)
        {

            string imagePath;
            if (type == WeatherType.Sunning)
                imagePath = Images.SunOutside;
            else
                imagePath = Images.CatInside;

            return imagePath;

        }

    }
}

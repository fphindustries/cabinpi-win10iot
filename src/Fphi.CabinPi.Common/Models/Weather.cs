using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fphi.Cabin.Pi.Common.Models;
using Fphi.Cabin.Pi.Common.Services;

namespace Fphi.CabinPi.Common.Models
{


    public class Weather
    {
        public DarkSkyDataPoint DataPoint { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Location { get; set; }
        public double Temp { get; set; }
        public override string ToString()
        {
            return $"{Temp}°F";
        }

        public static Weather GetWeather(DarkSkyDataPoint dataPoint)
        {
            var t = new Weather
            {
                Description = dataPoint.Summary,
                Image = GetImage(dataPoint.Icon),
                Location = "Outside",
                Temp = dataPoint.Temperature.Value

            };
            return t;
        }


        public static string GetImage(string icon)
        {
            switch (icon)
            {
                case "clear-day":
                    return "wi-day-sunny";
                case "clear-night":
                    return "wi-night-clear";
                case "rain":
                    return "wi-rain";
                case "snow":
                    return "wi-snow";
                case "sleet":
                    return "wi-sleet";
                case "wind":
                    return "wi-strong-wind";
                case "fog":
                    return "wi-fog";
                case "cloudy":
                    return "wi-cloudy";
                case "partly-cloudy-day":
                    return "wi-day-cloudy";
                case "partly-cloudy-night":
                    return "wi-night-alt-cloudy";

                default:
                    return "wi-na";
            }
            

        }

    }
}

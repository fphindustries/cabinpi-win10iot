using Fphi.Cabin.Pi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Common.Models
{
    public enum TemperatureLocation
    {
        Inside,
        Outside
    }

    public class Temperature
    {

        public int Temp { get; set; }
        public string Description{ get; set; }
        public string Image { get; set; }
        public TemperatureLocation TempLocation { get; set; }
        public string TempString => $"{Temp}°F";

        public static Temperature GetTemperature(int temp, TemperatureLocation location)
        {
            var t = new Temperature();
            t.Temp = temp;
            t.Description = GetDescriptionBasedOnTemp(temp);
            t.Image = GetImageBasedOnTemp(temp);
            t.TempLocation = location;
            return t;
        }

        public static string GetDescriptionBasedOnTemp(int temp)
        {
            if (temp > 50)
                return "Ah yeah...it's cozy in here";
            else
                return "Ok campers, rise and shine! — and don't forget your booties 'cause it's cooooooold out there today.";
        }

        public static string GetImageBasedOnTemp(int temp)
        {

            string imagePath;
            if (temp > 50)
                imagePath = Images.CatInside;
            else
                imagePath = Images.SunOutside;

            return imagePath;
            //return new BitmapImage(new Uri(imagePath, UriKind.Absolute));

        }

    }


}

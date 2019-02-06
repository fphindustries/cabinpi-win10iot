using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fphi.CabinPi.Ui.Models;

using Windows.UI.Xaml.Media.Imaging;

namespace Fphi.CabinPi.Ui2.Models
{
    public enum TempatureLocation
    {
        Inside,
        Outside
    }

    public class Tempature
    {

        public int Temp { get; set; }
        public string Description{ get; set; }
        public BitmapImage Image { get; set; }
        public TempatureLocation  TempLocation { get; set; }
        public string TempString => $"{Temp}°F";

        public static Images Images = new Images();
        public static Tempature GetTempature(int temp, TempatureLocation location)
        {
            var t = new Tempature();
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

        public static BitmapImage GetImageBasedOnTemp(int temp)
        {

            string imagePath;
            if (temp > 50)
                imagePath = Images.CatInside;
            else
                imagePath = Images.SunOutside;

            return new BitmapImage(new Uri(imagePath, UriKind.Absolute));
        }

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Fphi.CabinPi.Ui2.Models
{
    public class Tempature
    {
        public int Temp { get; set; }
        public string Description{ get; set; }
        public BitmapImage Image { get; set; }
        public string  Location { get; set; }
        public string TempString => $"{Temp}°F";

    }

    public static class TempatureHelper
    {
        public static Images Images = new Images();
        public static Tempature GetTempature(int temp)
        {
            var t = new Tempature();
            t.Temp = temp;

            return t;
        }

        public static string GetDescriptionBasedOnTemp(int temp)
        {
            return "This is the description";
        }

        public static BitmapImage GetImageBasedOnTemp(int temp)
        {
            string icon = "ms-appx:///Assets/{0}";
            if (temp > 50)
                icon = string.Format(icon, "Weather/01d.png");
            else
                icon = string.Format(icon, "Weather/01d.png");

            return new BitmapImage(new Uri(icon, UriKind.Absolute));
        }
    }

    public class Images
    {
        string icon = String.Format("ms-appx:///Assets/Weather/{0}.png", "01d");
        string iconCat = String.Format("ms-appx:///Assets/Inside/{0}.png", "sleepingcat");
    }
}

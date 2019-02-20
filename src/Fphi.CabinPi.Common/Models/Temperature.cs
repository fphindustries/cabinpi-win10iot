using Fphi.Cabin.Pi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Common.Models
{
    public class Temperature
    {

        public double Temp { get; set; }
        public string Description{ get; set; }
        public string Image { get; set; }
        public string Location { get; set; }
        public override string ToString()
        {
            return $"{Temp}°F";
        }

        public static Temperature GetTemperature(double temp)
        {
            var t = new Temperature
            {
                Description = GetDescriptionBasedOnInsideTemp(temp),
                Temp = temp,
                Image = GetImageBasedOnTemp(temp),
                Location = "Inside"

            };
            return t;
        }

        public static string GetDescriptionBasedOnInsideTemp(double temp)
        {
            switch (temp)
            {
                case double n when (n <= 0):
                    return
                        "The weather became so intensely cold that we sent for all the hunters who had remained out with captain Clarke's party, and they returned in the evening several of them frostbitten. Meriwether Lewis";
                case double n when (n > 0 && n <= 20):
                    return "Ok campers, rise and shine! — and don't forget your booties 'cause it's cooooooold out there today.";
                case double n when (n > 20 && n <= 32):
                    return "The cold never bothered me anyway";
                case double n when (n > 32 && n <= 40):
                    return " I just want to tell you both,good luck.We're all counting on you. ";
                case double n when (n > 40 && n <= 60):
                    return "I'd keep playing. I don't think the heavy stuff's going to come down for quite a while";
                case double n when (n > 60 && n <= 70):
                    return "What did one shepherd say to the other shepherd?  Let’s get the flock out of here.";
                case double n when (n > 70 && n <= 90):
                    return "You fight like a dairy farmer";
                case double n when (n > 90 && n <= 99):
                    return "I mean, that’s what life is: a series of down endings. All Jedi had was a bunch of Muppets.";
                case double n when (n >= 100):
                    return "Looks like I picked the wrong week to quit smoking.";
                default:
                    return "Crap...I don't know sorry";
            }
        }
        
        public static string GetImageBasedOnTemp(double temp)
        {

            string imagePath;
            if (temp > 50)
                imagePath = Images.CatInside;
            else
                imagePath = Images.SunOutside;

            return imagePath;

        }

    }


}

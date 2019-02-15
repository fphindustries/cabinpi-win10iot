using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Fphi.CabinPi.Common.Helpers;
using Newtonsoft.Json;


namespace Fphi.Cabin.Pi.Common.Services
{
    public interface IWeatherService
    {
        Task<Forecast> GetForecast();
    }


    public class DarkSkyService : Observable,IWeatherService
    {

        private const string BaseUrl = "https://api.darksky.net/forecast/";
        private readonly ISettings _settings;

        public DarkSkyService(ISettings settings)
        {
            _settings = settings;
        }

        public async Task<Forecast> GetForecast()
        {
            var httpClient = new HttpClient();
            string url = $"{BaseUrl}{_settings.DarkSkyApiKey}/{_settings.Latitude},{_settings.Longitude}?exclude=minutely";

            var response = await httpClient.GetStringAsync(new Uri(url));

            CurrentForecast = JsonConvert.DeserializeObject<Forecast>(response);
            return CurrentForecast;
        }

        private Forecast _forecast;

        public Forecast CurrentForecast
        {
            get { return _forecast; }
            set { Set(ref _forecast,value); }
        }
        
    }


    /// <summary>
    /// API responses consist of a UTF-8-encoded, JSON-formatted object that this class wraps.
    /// </summary>

    /// <summary>
    /// A data point object contains various properties, each representing the average (unless
    /// otherwise specified) of a particular weather phenomenon occurring during a period of time: an
    /// instant in the case of <see cref="Forecast.Currently"/>, a minute for <see
    /// cref="Forecast.Minutely"/>, an hour for <see cref="Forecast.Hourly"/>, and a day for <see cref="Forecast.Daily"/>.
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// The apparent (or “feels like”) temperature in degrees Fahrenheit.
        /// </summary>
        /// <remarks>optional, not on daily.</remarks>
        [JsonProperty(PropertyName = "apparentTemperature")]
        public double? ApparentTemperature { get; set; }

        /// <summary>
        /// The daytime high apparent temperature.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "apparentTemperatureHigh")]
        public double? ApparentTemperatureHigh { get; set; }

        /// <summary>
        /// The time of when <see cref="ApparentTemperatureHigh"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        public DateTimeOffset? ApparentTemperatureHighDateTime => ApparentTemperatureHighTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The overnight low apparent temperature.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "apparentTemperatureLow")]
        public double? ApparentTemperatureLow { get; set; }

        /// <summary>
        /// The time of when <see cref="ApparentTemperatureLow"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        public DateTimeOffset? ApparentTemperatureLowDateTime => ApparentTemperatureLowTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The maximum value of <see cref="ApparentTemperature"/> during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use apparentTemperatureHigh instead.")]
        [JsonProperty(PropertyName = "apparentTemperatureMax")]
        public double? ApparentTemperatureMax { get; set; }

        /// <summary>
        /// The time of when <see cref="ApparentTemperatureMax"/> occurs during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use apparentTemperatureHighDateTime instead.")]
        public DateTimeOffset? ApparentTemperatureMaxDateTime => ApparentTemperatureMaxTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The minimum value of <see cref="ApparentTemperature"/> during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use apparentTemperatureLow instead.")]
        [JsonProperty(PropertyName = "apparentTemperatureMin")]
        public double? ApparentTemperatureMin { get; set; }

        /// <summary>
        /// The time of when <see cref="ApparentTemperatureMin"/> occurs during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use apparentTemperatureLowDateTime instead.")]
        public DateTimeOffset? ApparentTemperatureMinDateTime => ApparentTemperatureMinTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The percentage of sky occluded by clouds, between 0 and 1, inclusive.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "cloudCover")]
        public double? CloudCover { get; set; }

        /// <summary>
        /// The time at which this data point begins. minutely data point are always aligned to the
        /// top of the minute, hourly data point objects to the top of the hour, and daily data point
        /// objects to midnight of the day, all according to the local time zone.
        /// </summary>
        public DateTimeOffset DateTime => TimeUnix.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The dew point in degrees Fahrenheit.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "dewPoint")]
        public double? DewPoint { get; set; }

        /// <summary>
        /// The relative humidity, between 0 and 1, inclusive.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "humidity")]
        public double? Humidity { get; set; }

        /// <summary>
        /// A machine-readable text summary of this data point, suitable for selecting an icon for display.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        /// <summary>
        /// The fractional part of the lunation number during the given day: a value of 0 corresponds
        /// to a new moon, 0.25 to a first quarter moon, 0.5 to a full moon, and 0.75 to a last
        /// quarter moon.
        /// <para>
        /// (The ranges in between these represent waxing crescent, waxing gibbous, waning gibbous,
        /// and waning crescent moons, respectively).
        /// </para>
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "moonPhase")]
        public double? MoonPhase { get; set; }

        /// <summary>
        /// The approximate direction of the nearest storm in degrees, with true north at 0° and
        /// progressing clockwise.
        /// <para>(If <see cref="NearestStormDistance"/> is zero, then this value will not be defined).</para>
        /// </summary>
        /// <remarks>optional, only on currently.</remarks>
        [JsonProperty(PropertyName = "nearestStormBearing")]
        public int? NearestStormBearing { get; set; }

        /// <summary>
        /// The approximate distance to the nearest storm in miles.
        /// <para>
        /// (A storm distance of 0 doesn’t necessarily refer to a storm at the requested location,
        /// but rather a storm in the vicinity of that location).
        /// </para>
        /// </summary>
        /// <remarks>optional, only on currently.</remarks>
        [JsonProperty(PropertyName = "nearestStormDistance")]
        public double? NearestStormDistance { get; set; }

        /// <summary>
        /// The columnar density of total atmospheric ozone at the given time in Dobson units.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "ozone")]
        public double? Ozone { get; set; }

        /// <summary>
        /// The amount of snowfall accumulation expected to occur, in inches.
        /// <para>(If no snowfall is expected, this property will not be defined).</para>
        /// </summary>
        /// <remarks>optional, only on hourly and daily.</remarks>
        [JsonProperty(PropertyName = "precipAccumulation")]
        public double? PrecipAccumulation { get; set; }

        /// <summary>
        /// The intensity (in inches of liquid water per hour) of precipitation occurring at the
        /// given time. This value is conditional on probability (that is, assuming any precipitation
        /// occurs at all) for minutely data points, and unconditional otherwise.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "precipIntensity")]
        public double? PrecipIntensity { get; set; }

        /// <summary>
        /// Undocumented, presumably was a value representing the potential error in the <see cref="PrecipIntensity"/>.
        /// </summary>
        /// <remarks>deprecated.</remarks>
        [Obsolete("PrecipIntensityError is no longer provided by the DarkSky API.")]
        [JsonProperty(PropertyName = "precipIntensityError")]
        public double? PrecipIntensityError { get; set; }

        /// <summary>
        /// The maximum value of <see cref="PrecipIntensity"/> during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "precipIntensityMax")]
        public double? PrecipIntensityMax { get; set; }

        /// <summary>
        /// The time of when <see cref="PrecipIntensityMax"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        public DateTimeOffset? PrecipIntensityMaxDateTime => PrecipIntensityMaxTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The probability of precipitation occurring, between 0 and 1, inclusive.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "precipProbability")]
        public double? PrecipProbability { get; set; }

        /// <summary>
        /// The type of precipitation occurring at the given time.
        /// <para>(If precipIntensity is zero, then this property will not be defined).</para>
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "precipType")]
        public string PrecipType { get; set; }

        /// <summary>
        /// The sea-level air pressure in millibars.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "pressure")]
        public double? Pressure { get; set; }

        /// <summary>
        /// A human-readable text summary of this data point.
        /// <para>
        /// (This property has millions of possible values, so don’t use it for automated purposes:
        /// use the <see cref="Icon"/> property, instead!).
        /// </para>
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        /// <summary>
        /// The time of when the sun will rise during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        public DateTimeOffset? SunriseDateTime => SunriseTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The time of when the sun will set during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        public DateTimeOffset? SunsetDateTime => SunsetTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The air temperature in degrees Fahrenheit.
        /// </summary>
        /// <remarks>optional, not on minutely.</remarks>
        [JsonProperty(PropertyName = "temperature")]
        public double? Temperature { get; set; }

        public int TemperatureRounded => Convert.ToInt32(Math.Round(Temperature.GetValueOrDefault(), 0));

        /// <summary>
        /// The daytime high temperature.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "temperatureHigh")]
        public double? TemperatureHigh { get; set; }

        public int TemperatureHighRounded => Convert.ToInt32(Math.Round(TemperatureHigh.GetValueOrDefault(), 0));
        /// <summary>
        /// The time of when <see cref="TemperatureHigh"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        public DateTimeOffset? TemperatureHighDateTime => TemperatureHighTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The overnight low temperature.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "temperatureLow")]
        public double? TemperatureLow { get; set; }

        public int TemperatureLowRounded => Convert.ToInt32(Math.Round(TemperatureLow.GetValueOrDefault(), 0));

        /// <summary>
        /// The time of when <see cref="TemperatureLow"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        public DateTimeOffset? TemperatureLowDateTime => TemperatureLowTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The maximum value of <see cref="Temperature"/> during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use temperatureHigh instead.")]
        [JsonProperty(PropertyName = "temperatureMax")]
        public double? TemperatureMax { get; set; }

        /// <summary>
        /// The time of when <see cref="TemperatureMax"/> occurs during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use temperatureHighDateTime instead.")]
        public DateTimeOffset? TemperatureMaxDateTime => TemperatureMaxTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The minimum value of <see cref="Temperature"/> during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use temperatureLow instead.")]
        [JsonProperty(PropertyName = "temperatureMin")]
        public double? TemperatureMin { get; set; }

        /// <summary>
        /// The time of when <see cref="TemperatureMin"/> occurs during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use temperatureLowDateTime instead.")]
        public DateTimeOffset? TemperatureMinDateTime => TemperatureMinTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The UV index.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "uvIndex")]
        public int? UvIndex { get; set; }

        /// <summary>
        /// The time of when the maximum <see cref="UvIndex"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        public DateTimeOffset? UvIndexDateTime => UvIndexTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The average visibility in miles, capped at 10 miles.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "visibility")]
        public double? Visibility { get; set; }

        /// <summary>
        /// The direction that the wind is coming from in degrees, with true north at 0° and
        /// progressing clockwise.
        /// <para>(If <see cref="WindSpeed"/> is zero, then this value will not be defined).</para>
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "windBearing")]
        public int? WindBearing { get; set; }

        /// <summary>
        /// The wind gust speed in miles per hour.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "windGust")]
        public double? WindGust { get; set; }

        /// <summary>
        /// The time of when the maximum <see cref="WindGust"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        public DateTimeOffset? WindGustDateTime => WindGustTimeUnix?.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// The wind speed in miles per hour.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "windSpeed")]
        public double? WindSpeed { get; set; }

        /// <summary>
        /// The UNIX time of when <see cref="ApparentTemperatureHigh"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "apparentTemperatureHighTime")]
        internal long? ApparentTemperatureHighTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when <see cref="ApparentTemperatureLow"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "apparentTemperatureLowTime")]
        internal long? ApparentTemperatureLowTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when <see cref="ApparentTemperatureMax"/> occurs during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use apparentTemperatureHighTimeUnix instead.")]
        [JsonProperty(PropertyName = "apparentTemperatureMaxTime")]
        internal long? ApparentTemperatureMaxTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when <see cref="ApparentTemperatureMin"/> occurs during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use apparentTemperatureLowTimeUnix instead.")]
        [JsonProperty(PropertyName = "apparentTemperatureMinTime")]
        internal long? ApparentTemperatureMinTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when <see cref="PrecipIntensityMax"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "precipIntensityMaxTime")]
        internal long? PrecipIntensityMaxTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when the sun will rise during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "sunriseTime")]
        internal long? SunriseTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when the sun will set during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "sunsetTime")]
        internal long? SunsetTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when <see cref="TemperatureHigh"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "temperatureHighTime")]
        internal long? TemperatureHighTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when <see cref="TemperatureLow"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "temperatureLowTime")]
        internal long? TemperatureLowTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when <see cref="TemperatureMax"/> occurs during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use temperatureHighTimeUnix instead.")]
        [JsonProperty(PropertyName = "temperatureMaxTime")]
        internal long? TemperatureMaxTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when <see cref="TemperatureMin"/> occurs during a given day.
        /// </summary>
        /// <remarks>deprecated, optional, only on daily.</remarks>
        [Obsolete("Use temperatureLowTimeUnix instead.")]
        [JsonProperty(PropertyName = "temperatureMinTime")]
        internal long? TemperatureMinTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time at which this data point begins. minutely data point are always aligned to
        /// the top of the minute, hourly data point objects to the top of the hour, and daily data
        /// point objects to midnight of the day, all according to the local time zone.
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        internal long TimeUnix { get; set; }

        /// <summary>
        /// TimeZone from the parent Forecast object.
        /// </summary>
        internal string TimeZone { get; set; }

        /// <summary>
        /// The UNIX time of when the maximum <see cref="UvIndex"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "uvIndexTime")]
        internal long? UvIndexTimeUnix { get; set; }

        /// <summary>
        /// The UNIX time of when the maximum <see cref="WindGust"/> occurs during a given day.
        /// </summary>
        /// <remarks>optional, only on daily.</remarks>
        [JsonProperty(PropertyName = "windGustTime")]
        internal long? WindGustTimeUnix { get; set; }

        public string WeatherIconClass
        {
            get
            {
                switch (Icon)
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


    /// <summary>
    /// A data block object represents the various weather phenomena occurring over a period of time.
    /// </summary>
    public class DataBlock
    {
        /// <summary>
        /// An List of <see cref="DataPoint"/>, ordered by time, which together describe the weather
        /// conditions at the requested location over time.
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public List<DataPoint> Data { get; set; }

        /// <summary>
        /// A machine-readable text summary of this data block.
        /// <para>(May take on the same values as the <see cref="Icon"/> property of data points).</para>
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        /// <summary>
        /// A human-readable summary of this data block.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }
    }

    /// <summary>
    /// The flags object contains various metadata information related to the request.
    /// </summary>
    public class Flags
    {
        /// <summary>
        /// Undocumented, presumably a list of stations.
        /// </summary>
        [JsonProperty(PropertyName = "darksky-stations")]
        public List<string> DarkskyStations { get; set; }

        /// <summary>
        /// The presence of this property indicates that the Dark Sky data source supports the given
        /// location, but a temporary error (such as a radar station being down for maintenance) has
        /// made the data unavailable.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "darksky-unavailable")]
        public string DarkskyUnavailable { get; set; }

        /// <summary>
        /// Undocumented.
        /// </summary>
        [JsonProperty(PropertyName = "isd-stations")]
        public List<string> IsdStations { get; set; }

        /// <summary>
        /// Undocumented.
        /// </summary>
        [JsonProperty(PropertyName = "lamp-stations")]
        public List<string> LampStations { get; set; }

        /// <summary>
        /// Undocumented.
        /// </summary>
        [JsonProperty(PropertyName = "madis-stations")]
        public List<string> MadisStations { get; set; }

        /// <summary>
        /// Undocumented.
        /// </summary>
        [JsonProperty(PropertyName = "metno-license")]
        public string MetnoLicense { get; set; }

        /// <summary>
        /// Undocumented.
        /// </summary>
        [JsonProperty(PropertyName = "nearest-station")]
        public double? NearestStation { get; set; }

        /// <summary>
        /// This property contains an array of IDs for each <a
        /// href="https://darksky.net/dev/docs/sources">data source</a> utilized in servicing this request.
        /// </summary>
        [JsonProperty(PropertyName = "sources")]
        public List<string> Sources { get; set; }

        /// <summary>
        /// Indicates the units which were used for the data in this request.
        /// </summary>
        [JsonProperty(PropertyName = "units")]
        public string Units { get; set; }
    }
    public class Forecast
    {
        /// <summary>
        /// An <see cref="Alert"/> list, which, if present, contains any severe weather alerts
        /// pertinent to the requested location.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "alerts")]
        public List<Alert> Alerts { get; set; }

        /// <summary>
        /// A <see cref="DataPoint"/> containing the current weather conditions at the requested location.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "currently")]
        public DataPoint Currently { get; set; }

        /// <summary>
        /// A <see cref="DataBlock"/> containing the weather conditions day-by-day for the next week.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "daily")]
        public DataBlock Daily { get; set; }

        /// <summary>
        /// A <see cref="Models.Flags"/> containing miscellaneous metadata about the request.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "flags")]
        public Flags Flags { get; set; }

        /// <summary>
        /// A <see cref="DataBlock"/> containing the weather conditions hour-by-hour for the next two days.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "hourly")]
        public DataBlock Hourly { get; set; }

        /// <summary>
        /// The requested latitude.
        /// </summary>
        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// The requested longitude.
        /// </summary>
        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// A <see cref="DataBlock"/> containing the weather conditions minute-by-minute for the next hour.
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "minutely")]
        public DataBlock Minutely { get; set; }

        /// <summary>
        /// The current timezone offset in hours. (Use of this property will almost certainly result
        /// in Daylight Saving Time bugs. Please use <see cref="TimeZone"/>, instead).
        /// </summary>
        [Obsolete("Use of this property will almost certainly result in Daylight Saving Time bugs. Please use timezone, instead.")]
        [JsonProperty(PropertyName = "offset")]
        public string Offset { get; set; }

        /// <summary>
        /// The IANA timezone name for the requested location. This is used for text summaries and
        /// for determining when <see cref="Hourly"/> and <see cref="Daily"/><see cref="DataBlock"/>
        /// objects begin.
        /// </summary>
        [JsonProperty(PropertyName = "timezone")]
        public string TimeZone { get; set; }
    }

    /// <summary>
    /// The alerts array contains objects representing the severe weather warnings issued for the
    /// requested location by a governmental authority (please see our <a
    /// href="https://darksky.net/dev/docs/sources">data sources page</a> for a list of sources).
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// The time at which the alert was issued.
        /// </summary>
        public DateTimeOffset DateTime => UnixTime.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// A detailed description of the alert.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        public string DescriptionFormatted => Description.Replace("\n", "<br />");

        /// <summary>
        /// The time at which the alert will expire. (Some alerts sources, unfortunately, do not
        /// define expiration time, and in these cases this parameter will not be defined).
        /// </summary>
        public DateTimeOffset ExpiresDateTime => UnixExpires.ToDateTimeFromUnixTimestamp(TimeZone);

        /// <summary>
        /// A <see cref="List{T}"/> of strings representing the names of the regions covered by this
        /// weather alert.
        /// </summary>
        [JsonProperty(PropertyName = "regions")]
        public List<string> Regions { get; set; }

        /// <summary>
        /// The severity of the weather alert. Will take one of the following values: "advisory" (an
        /// individual should be aware of potentially severe weather), "watch" (an individual should
        /// prepare for potentially severe weather), or "warning" (an individual should take
        /// immediate action to protect themselves and others from potentially severe weather).
        /// </summary>
        [JsonProperty(PropertyName = "severity")]
        public string Severity { get; set; }

        /// <summary>
        /// A brief description of the alert.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// An HTTP(S) URI that one may refer to for detailed information about the alert.
        /// </summary>
        [JsonProperty(PropertyName = "uri")]
        public Uri Uri { get; set; }

        /// <summary>
        /// TimeZone from the parent Forecast object.
        /// </summary>
        internal string TimeZone { get; set; }

        /// <summary>
        /// The UNIX time at which the alert will expire. (Some alerts sources, unfortunately, do not
        /// define expiration time, and in these cases this parameter will not be defined).
        /// </summary>
        /// <remarks>optional.</remarks>
        [JsonProperty(PropertyName = "expires")]
        internal long UnixExpires { get; set; }

        /// <summary>
        /// The UNIX time at which the alert was issued.
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        internal long UnixTime { get; set; }

    }

}

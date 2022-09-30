using System.ComponentModel;

namespace CloudWeather.Report.Models;

public enum WeatherTypes
{
    [Description("snow")]
    SNOW,
    [Description("rain")]
    RAIN
}
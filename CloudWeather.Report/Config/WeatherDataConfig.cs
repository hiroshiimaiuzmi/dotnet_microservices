namespace CloudWeather.Report.Config;

public class WeatherDataConfig
{
    public string PrecipitationDataProtocol { get; set; } = string.Empty;
    public string PrecipitationDataHost { get; set; } = string.Empty;
    public string PrecipitationDataPort { get; set; } = string.Empty;
    public string TemperatureDataProtocol { get; set; } = string.Empty;
    public string TemperatureDataHost { get; set; } = string.Empty;
    public string TemperatureDataPort { get; set; } = string.Empty;

    public string BuildPrecipitationServiceEndpoint(string zip, int days)
    {
        var protocol = PrecipitationDataProtocol;
        var host = PrecipitationDataHost;
        var port = PrecipitationDataPort;
        return $"{protocol}://{host}:{port}/observation/{zip}?days={days}";
    }

    public string BuildTemperatureServiceEndpoint(string zip, int days)
    {
        var protocol = TemperatureDataProtocol;
        var host = TemperatureDataHost;
        var port = TemperatureDataPort;
        return $"{protocol}://{host}:{port}/observation/{zip}?days={days}";
    }
}
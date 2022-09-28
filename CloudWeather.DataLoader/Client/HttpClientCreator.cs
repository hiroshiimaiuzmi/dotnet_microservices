using Microsoft.Extensions.Configuration;

namespace CloudWeather.DataLoader.Client;

public class HttpClientCreator
{
    private static readonly IConfigurationSection _servicesConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build()
            .GetSection("services");


    public static HttpClient CreateTemperatureClient()
    {
        var tempConfig = _servicesConfig.GetSection("Temperature");
        var tempHost = tempConfig["Host"];
        var tempPort = tempConfig["Port"];

        var precipitationConfig = _servicesConfig.GetSection("Precipitation");
        var precipitationHost = precipitationConfig["Host"];
        var precipitationPort = precipitationConfig["Port"];

        var temperatureHttpClient = new HttpClient();
        temperatureHttpClient.BaseAddress = new Uri($"http://{tempHost}:{tempPort}");

        return temperatureHttpClient;
    }

    public static HttpClient CreatePrecipitationClient()
    {
        var tempConfig = _servicesConfig.GetSection("Temperature");
        var tempHost = tempConfig["Host"];
        var tempPort = tempConfig["Port"];

        var precipitationConfig = _servicesConfig.GetSection("Precipitation");
        var precipitationHost = precipitationConfig["Host"];
        var precipitationPort = precipitationConfig["Port"];

        var precipitationHttpClient = new HttpClient();
        precipitationHttpClient.BaseAddress = new Uri($"http://{precipitationHost}:{precipitationPort}");

        return precipitationHttpClient;
    }

}
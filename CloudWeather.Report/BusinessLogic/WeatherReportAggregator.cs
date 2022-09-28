using System.Text.Json;
using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using CloudWeather.Report.Models;
using Microsoft.Extensions.Options;

namespace CloudWeather.Report.BusinessLogic;

public interface IWeatherReportAggregator
{
    public Task<WeatherReport> BuildReport(string zip, int days);
}
public class WeatherReportAggregator : IWeatherReportAggregator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WeatherReportAggregator> _logger;
    private readonly WeatherDataConfig _config;
    private readonly WeatherReportDbContext _db;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public WeatherReportAggregator(
        IHttpClientFactory httpClientFactory,
        ILogger<WeatherReportAggregator> logger,
        IOptions<WeatherDataConfig> config,
        WeatherReportDbContext db)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config.Value;
        _db = db;
    }

    public async Task<WeatherReport> BuildReport(string zip, int days)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var precipitationData = await FetchPrecipitationData(httpClient, zip, days);
        var totalSnow = GetTotal(precipitationData, WeatherTypes.SNOW);
        var totalRain = GetTotal(precipitationData, WeatherTypes.RAIN);
        _logger.LogInformation(
            $"zip: {zip} over last {days} days: " +
            $"total snow: {totalSnow}, rain: {totalRain}"
        );

        var temperatureData = await FetchTemperatureData(httpClient, zip, days);
        var averageHigh = temperatureData.Average(t => t.TempHighF);
        var averageLow = temperatureData.Average(t => t.TempLowF);
        _logger.LogInformation(
            $"zip: {zip} over last {days} days: " +
            $"high temperature: {averageHigh}, low temperature: {averageLow}"
        );

        var weatherReport = new WeatherReport
        {
            AverageHighF = Math.Round(averageHigh, 1),
            AverageLowF = Math.Round(averageLow, 1),
            RainfallTotalInches = totalRain,
            SnowTotalInches = totalSnow,
            ZipCode = zip,
            CreatedOn = DateTime.UtcNow,
        };

        _db.Add(weatherReport);
        await _db.SaveChangesAsync();

        return weatherReport;
    }

    private static decimal GetTotal(List<PrecipitationModel> precipitationData, WeatherTypes weatherType)
    {
        var total = precipitationData
            .Where(d => d.WeatherType == weatherType.ToString())
            .Sum(d => d.AmountInches);
        return Math.Round(total, 1);
    }

    private async Task<List<TemperatureModel>> FetchTemperatureData(HttpClient httpClient, string zip, int days)
    {
        var endpoint = _config.BuildTemperatureServiceEndpoint(zip, days);
        var temperatureRecords = await httpClient.GetAsync(endpoint);
        var temperatureData = await temperatureRecords
            .Content
            .ReadFromJsonAsync<List<TemperatureModel>>(_jsonSerializerOptions);

        return temperatureData ?? new List<TemperatureModel>();
    }

    private async Task<List<PrecipitationModel>> FetchPrecipitationData(HttpClient httpClient, string zip, int days)
    {
        var endpoint = _config.BuildPrecipitationServiceEndpoint(zip, days);
        var precipitationRecords = await httpClient.GetAsync(endpoint);
        var precipitationData = await precipitationRecords
            .Content
            .ReadFromJsonAsync<List<PrecipitationModel>>(_jsonSerializerOptions);

        return precipitationData ?? new List<PrecipitationModel>();
    }
}
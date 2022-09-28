using System.Net.Http.Json;
using CloudWeather.DataLoader.Client;
using CloudWeather.DataLoader.Models;

var zipCodes = new List<string>{
    "73026",
    "68104",
    "04401",
    "32808",
    "19717",
};

Console.WriteLine("Starting Data Load");


foreach (var zip in zipCodes)
{
    Console.WriteLine($"Processing Zip Code: {zip}");

    var from = DateTime.Now.AddYears(-2);
    var thru = DateTime.Now;

    for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
    {
        var temps = PostTemp(zip, day, HttpClientCreator.CreateTemperatureClient());
        PostPrecipitation(temps[0], zip, day, HttpClientCreator.CreatePrecipitationClient());
    }
}

void PostPrecipitation(int lowTemp, string zip, DateTime day, HttpClient precipitationHttpClient)
{
    var rand = new Random();
    var isPrecipitation = rand.Next(2) < 1;
    var precipitation = new PrecipitationModel
    {
        AmountInches = 0,
        WeatherType = "none",
        ZipCode = zip,
        CreatedOn = day
    };

    if (isPrecipitation)
    {
        var precipitationInches = rand.Next(1, 16);
        precipitation = new PrecipitationModel
        {
            AmountInches = precipitationInches,
            WeatherType = lowTemp < 32 ? "snow" : "rain",
            ZipCode = zip,
            CreatedOn = day
        };
    }

    var response = precipitationHttpClient
                    .PostAsJsonAsync("observation", precipitation)
                    .Result;

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted Precipitation: Date: {day:d}" +
        $"Zip: {zip}" +
        $"Type: {precipitation.WeatherType}" +
        $"Amount (in.): {precipitation.AmountInches}");
    }

}

List<int> PostTemp(string zip, DateTime day, HttpClient temperatureHttpClient)
{
    var rand = new Random();
    var t1 = rand.Next(0, 100);
    var t2 = rand.Next(0, 100);
    var hiLoTemps = new List<int> { t1, t2 };
    hiLoTemps.Sort();

    var temperatureObservation = new TemperatureModel
    {
        TempLowF = hiLoTemps[0],
        TempHighF = hiLoTemps[1],
        ZipCode = zip,
        CreatedOn = day
    };

    var response = temperatureHttpClient
                    .PostAsJsonAsync("observation", temperatureObservation)
                    .Result;

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine(
        $"Posted Precipitation: Date: {day:d}" +
        $"Zip: {zip}" +
        $"Lo(F): {hiLoTemps[0]}" +
        $"Hi(F): {hiLoTemps[1]}");
    }
    else
    {
        Console.WriteLine(response.ToString());
    }

    return hiLoTemps;
}

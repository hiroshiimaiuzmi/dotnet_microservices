using CloudWeather.Report.BusinessLogic;
using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IWeatherReportAggregator, WeatherReportAggregator>();
builder.Services.AddOptions();
builder.Services.Configure<WeatherDataConfig>(builder.Configuration.GetSection("WeatherDataConfig"));

builder.Services.AddDbContext<WeatherReportDbContext>(options => {
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient
);

var app = builder.Build();

app.MapGet("/weather-report/{zip}", async (string zip, [FromQuery] int? days, IWeatherReportAggregator weatherAggregator) => {
    if (days is null || days < 1 || days > 30)
    {
        return Results.BadRequest("Please provide a days query parameter between 1 and 30");
    }

    var report = await weatherAggregator.BuildReport(zip, days.Value);
    return Results.Ok(report);
});

app.Run();

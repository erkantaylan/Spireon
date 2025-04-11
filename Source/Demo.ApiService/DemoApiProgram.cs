using System.Reflection;
using Core.Framework;

namespace Demo.ApiService;

internal static class DemoApiProgram
{
    private static readonly string[] Summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    public static void Main(string[] args)
    {
        var micro = new SpireonApp(args);

        micro.RegisterApiDefaults();
        micro.RegisterCors();
        micro.RegisterTransient(Assembly.GetExecutingAssembly());

        micro.Register(
            builder => { },
            app =>
            {
                app.MapGet(
                    "/weatherforecast",
                    () =>
                    {
                        WeatherForecast[] forecast = Enumerable.Range(1, 5)
                                                               .Select(
                                                                    index =>
                                                                        new WeatherForecast(
                                                                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                                                            Random.Shared.Next(-20, 55),
                                                                            Summaries[Random.Shared.Next(Summaries.Length)]
                                                                        ))
                                                               .ToArray();

                        return forecast;
                    });
            });

        micro.Run();
    }
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

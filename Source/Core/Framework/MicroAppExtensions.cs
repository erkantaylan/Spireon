using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace Core.Framework;

public static class MicroAppExtensions
{
    public static void RegisterApiDefaults(this SpireonApp webApp)
    {
        webApp.RegisterDefaultConfiguration();
        webApp.RegisterControllers();
        webApp.RegisterOtlp();
        webApp.RegisterOpenApi();

        webApp.RegisterBuilder(builder => builder.Services.AddProblemDetails());
    }

    public static void RegisterOpenApi(this SpireonApp webApp)
    {
        webApp.Register(
            builder => { builder.Services.AddOpenApi(); },
            (app, _) =>
            {
                app.MapOpenApi();
                app.MapScalarApiReference(
                    options =>
                    {
                        options.WithTitle("Demo Api")
                               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                    });
            });
    }

    public static void RegisterControllers(this SpireonApp webApp)
    {
        webApp.Register(
            builder => { builder.Services.AddControllers(); },
            (app, _) => { app.MapControllers(); });
    }

    public static void RegisterDefaultConfiguration(this SpireonApp webApp)
    {
        webApp.RegisterBuilder(builder => { builder.Services.AddSingleton<IConfiguration>(builder.Configuration); });
    }

    public static void RegisterOtlp(this SpireonApp webApp)
    {
        webApp.Register(
            builder => { builder.AddServiceDefaults(); },
            (app, _) => { app.MapDefaultEndpoints(); });
    }
}
using System.Reflection;
using Core.Framework;
using Demo.Web.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Demo.Web;

internal static class DemoWebProgram
{
    public static void Main(string[] args)
    {
        var micro = new SpireonApp(args);

        micro.RegisterApiDefaults();
        micro.RegisterCors();
        micro.RegisterTransient(Assembly.GetExecutingAssembly());

        micro.Register(
            builder =>
            {
                builder.AddRedisOutputCache("demo-cache");

                builder.Services
                       .AddRazorComponents()
                       .AddInteractiveServerComponents();
                builder.Services.AddHttpClient<WeatherApiClient>(client => client.BaseAddress = new Uri("http://demo-api"));
                builder.Services.AddFluentUIComponents();
            },
            app =>
            {
                app.UseStaticFiles();
                app.UseAntiforgery();

                app.UseOutputCache();

                app.MapRazorComponents<App>()
                   .AddInteractiveServerRenderMode();
            });

        micro.Run();
    }
}

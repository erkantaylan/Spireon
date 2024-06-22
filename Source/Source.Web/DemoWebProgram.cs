using System.Reflection;
using Core.Framework;
using Source.Web.Components;

namespace Source.Web;

internal static class DemoWebProgram
{
    public static void Main(string[] args)
    {
        var micro = new MicroApp(args);

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

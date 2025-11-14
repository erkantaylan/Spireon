using Core.Framework;
using Demo.BlazorDemo.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using _Imports = Demo.BlazorDemo.Client._Imports;

namespace Demo.BlazorDemo;

public class Program
{
    public static void Main(string[] args)
    {
        var micro = new SpireonApp(args);

        micro.RegisterControllers();
        micro.RegisterOpenApi();
        micro.RegisterCors();
        micro.RegisterDefaultConfiguration();

        micro.Register(
            builder =>
            {
                builder.Services.AddFluentUIComponents();
                
                builder.Services
                       .AddRazorComponents()
                       .AddInteractiveServerComponents()
                       .AddInteractiveWebAssemblyComponents();
            },
            (app, _) =>
            {
                if (app.Environment.IsDevelopment())
                {
                    app.UseWebAssemblyDebugging();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseAntiforgery();

                app.MapStaticAssets();
                app.MapRazorComponents<App>()
                   .AddInteractiveServerRenderMode()
                   .AddInteractiveWebAssemblyRenderMode()
                   .AddAdditionalAssemblies(typeof(_Imports).Assembly);
            });


        micro.Run();
    }
}
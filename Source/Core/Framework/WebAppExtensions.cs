using Core.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Framework;

public static class WebAppExtensions
{
    public static void RegisterApiDefaults(this MicroApp webApp)
    {
        webApp.RegisterDefaultConfiguration();
        webApp.RegisterControllers();
        webApp.RegisterOtlp();
        webApp.RegisterSwagger();

        webApp.RegisterBuilder(builder => builder.Services.AddProblemDetails());
    }

    public static void RegisterControllers(this MicroApp webApp)
    {
        webApp.Register(
            builder => { builder.Services.AddControllers(); },
            app => { app.MapControllers(); });
    }

    public static void RegisterDefaultConfiguration(this MicroApp webApp)
    {
        webApp.RegisterBuilder(
            builder => { builder.Services.AddSingleton<IConfiguration>(builder.Configuration); });
    }

    public static void RegisterOtlp(this MicroApp webApp)
    {
        webApp.Register(
            builder => { builder.AddServiceDefaults(); },
            app => { app.MapDefaultEndpoints(); });
    }

    public static void RegisterSwagger(this MicroApp webApp)
    {
        webApp.Register(
            builder =>
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(
                    options =>
                    {
                        //Add padlocks to auth endpoints
                        options.OperationFilter<AuthResponsesOperationFilter>();
                    });
                //add buton to Authorize
                builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            },
            app =>
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            });
    }
    
}
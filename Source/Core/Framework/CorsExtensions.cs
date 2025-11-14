using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Framework;

public static class CorsExtensions
{
    public static void RegisterCors(this SpireonApp spireonApp)
    {
        spireonApp.Register(
            builder =>
            {
                builder.Services.AddCors(
                    options =>
                    {
                        options.AddPolicy(
                            "AllowAllPolicy",
                            policy =>
                            {
                                policy.AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowAnyOrigin();
                            });
                    });
            },
            (app, _) => { app.UseCors("AllowAllPolicy"); });
    }
}

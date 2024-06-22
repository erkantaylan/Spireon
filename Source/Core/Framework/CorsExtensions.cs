using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Framework;

public static class CorsExtensions
{
    public static void RegisterCors(this MicroApp microApp)
    {
        microApp.Register(
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
            app => { app.UseCors("AllowAllPolicy"); });
    }
}

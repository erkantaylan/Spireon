// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Hosting;

internal static class MigrateDbContextExtensions
{
    private static readonly string ActivitySourceName = "DbMigrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static IServiceCollection AddMigration<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        return services.AddMigration<TContext>((_, _) => Task.CompletedTask);
    }

    public static IServiceCollection AddMigration<TContext>(
        this IServiceCollection services,
        Func<TContext, IServiceProvider, Task> seeder
    )
        where TContext : DbContext
    {
        // Enable migration tracing
        services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(ActivitySourceName));

        return services.AddHostedService(sp => new MigrationHostedService<TContext>(sp, seeder));
    }

    public static IServiceCollection AddMigration<TContext, TDbSeeder>(this IServiceCollection services)
        where TContext : DbContext
        where TDbSeeder : class, IDbSeeder<TContext>
    {
        services.AddScoped<IDbSeeder<TContext>, TDbSeeder>();
        return services.AddMigration<TContext>(
            (context, sp) => sp.GetRequiredService<IDbSeeder<TContext>>().SeedAsync(context));
    }

    private static async Task MigrateDbContextAsync<TContext>(
        this IServiceProvider services,
        Func<TContext, IServiceProvider, Task> seeder
    ) where TContext : DbContext
    {
        using IServiceScope scope = services.CreateScope();
        IServiceProvider scopeServices = scope.ServiceProvider;
        var logger = scopeServices.GetRequiredService<ILogger<TContext>>();
        var context = scopeServices.GetService<TContext>();

        using Activity? activity = ActivitySource.StartActivity($"Migration operation {typeof(TContext).Name}");

        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            await strategy.ExecuteAsync(() => InvokeSeeder(seeder, context, scopeServices));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while migrating the database used on context {DbContextName}",
                typeof(TContext).Name);

            activity.SetExceptionTags(ex);

            throw;
        }
    }

    private static async Task InvokeSeeder<TContext>(
        Func<TContext, IServiceProvider, Task> seeder,
        TContext context,
        IServiceProvider services
    )
        where TContext : DbContext
    {
        using Activity? activity = ActivitySource.StartActivity($"Migrating {typeof(TContext).Name}");

        try
        {
            await context.Database.MigrateAsync();
            await seeder(context, services);
        }
        catch (Exception ex)
        {
            activity.SetExceptionTags(ex);

            throw;
        }
    }

    private class MigrationHostedService<TContext>
    (
        IServiceProvider serviceProvider,
        Func<TContext, IServiceProvider, Task> seeder
    )
        : BackgroundService where TContext : DbContext
    {
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return serviceProvider.MigrateDbContextAsync(seeder);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}

public interface IDbSeeder<in TContext> where TContext : DbContext
{
    Task SeedAsync(TContext context);
}

using Projects;

namespace Startup;

public class AspireStartup
{
    public static void Main(string[] args)
    {
        IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

        AddDemos(builder);

        builder.Build().Run();
    }

    private static void AddDemos(IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<RedisResource> cache = builder.AddRedis("demo-cache");

        IResourceBuilder<ProjectResource> apiService = builder.AddProject<Source_ApiService>("demo-api");

        builder.AddProject<Source_Web>("demo-web")
               .WithExternalHttpEndpoints()
               .WithReference(cache)
               .WithReference(apiService);
    }
}

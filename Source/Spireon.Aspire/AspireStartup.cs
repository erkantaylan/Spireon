using Projects;

namespace Spireon.Aspire;

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

        IResourceBuilder<ProjectResource> apiService = builder.AddProject<Demo_ApiService>("demo-api");

        builder.AddProject<Demo_Web>("demo-web")
               .WithExternalHttpEndpoints()
               .WithReference(cache)
               .WithReference(apiService);
    }
}
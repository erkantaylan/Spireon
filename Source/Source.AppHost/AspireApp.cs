using Projects;

namespace Source.AppHost;

public static class Program
{
    public static void Main(string[] args)
    {
        IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

        IResourceBuilder<RedisResource> cache = builder.AddRedis("cache");

        IResourceBuilder<ProjectResource> apiService = builder.AddProject<Source_ApiService>("apiservice");

        builder.AddProject<Source_Web>("webfrontend")
               .WithExternalHttpEndpoints()
               .WithReference(cache)
               .WithReference(apiService);

        builder.Build().Run();
    }
}

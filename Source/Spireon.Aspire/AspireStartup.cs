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
        builder.AddProject<Demo_BlazorDemo>("demo-blazor")
               .WithExternalHttpEndpoints();
    }
}
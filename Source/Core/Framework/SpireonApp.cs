using Microsoft.AspNetCore.Builder;

namespace Core.Framework;

public class SpireonApp(string[] args)
{
    private readonly List<Action<WebApplication, WebApplicationBuilder>> appActions = [];
    private readonly List<Action<WebApplicationBuilder>> webActions = [];
    private readonly WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

    public SpireonApp RegisterBuilder(Action<WebApplicationBuilder> webAction)
    {
        webActions.Add(webAction);

        return this;
    }

    public SpireonApp RegisterApp(Action<WebApplication, WebApplicationBuilder> appAction)
    {
        appActions.Add(appAction);

        return this;
    }

    public SpireonApp Register(Action<WebApplicationBuilder> builder, Action<WebApplication, WebApplicationBuilder> app)
    {
        RegisterBuilder(builder);
        RegisterApp(app);

        return this;
    }

    public void Run()
    {
        foreach (Action<WebApplicationBuilder> action in webActions)
        {
            action.Invoke(webApplicationBuilder);
        }

        WebApplication app = webApplicationBuilder.Build();

        foreach (Action<WebApplication, WebApplicationBuilder> action in appActions)
        {
            action.Invoke(app, webApplicationBuilder);
        }

        app.Run();
    }
}
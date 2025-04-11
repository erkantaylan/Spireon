using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Framework;

public static class Extensions
{
    public static void RegisterTransient(this SpireonApp webApp, Assembly assembly)
    {
        List<Type> types = assembly.GetTypes()
                                   .Where(o => o.GetCustomAttributes<MercuryAutoRegisteredAttribute>().Any())
                                   .ToList();

        foreach (Type type in types)
        {
            var attribute = type.GetCustomAttribute<MercuryAutoRegisteredAttribute>();

            webApp.RegisterBuilder(Action);

            continue;

            void Action(IHostApplicationBuilder builder)
            {
                if (attribute!.Type == null)
                {
                    builder.Services.AddTransient(type);
                }
                else
                {
                    builder.Services.AddTransient(attribute.Type, type);
                }
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class MercuryBridgeAttribute : MercuryAutoRegisteredAttribute
{
    public MercuryBridgeAttribute(Type? type = null) : base(type) { }
}

[AttributeUsage(AttributeTargets.Class)]
public class MercuryRepositoryAttribute : MercuryAutoRegisteredAttribute
{
    public MercuryRepositoryAttribute(Type? type = null) : base(type) { }
}

public abstract class MercuryAutoRegisteredAttribute : Attribute
{
    protected MercuryAutoRegisteredAttribute(Type? type = null)
    {
        Type = type;
    }

    public Type? Type { get; }
}

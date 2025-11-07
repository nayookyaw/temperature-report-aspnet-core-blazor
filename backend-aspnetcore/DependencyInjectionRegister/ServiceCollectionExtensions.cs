using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BackendAspNetCore.DependencyInjectionRegister;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection BindApplicationServices(this IServiceCollection services)
    {
        // 1) Scan all app assemblies that start with your root name
        var root = "BackendAspNetCore"; // <-- root namespace
        var entry = Assembly.GetEntryAssembly()!;
        var assemblies = new List<Assembly> { entry };

        // Load referenced assemblies with the same prefix
        foreach (var name in entry.GetReferencedAssemblies())
        {
            if (name.Name is not null && name.Name.StartsWith(root, StringComparison.Ordinal))
            {
                assemblies.Add(Assembly.Load(name));
            }
        }

        // 2) Limit to Services/Repositories namespaces (and their sub-namespaces)
        bool IsTargetNamespace(string? ns) =>
            ns is not null &&
            (ns == $"{root}.Services" || ns.StartsWith($"{root}.Services.") ||
             ns == $"{root}.Repositories" || ns.StartsWith($"{root}.Repositories."));

        // 3) Register classes whose interface matches I{ClassName}
        foreach (var asm in assemblies)
        {
            
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.OfType<Type>().ToArray();   // filters nulls, returns Type[]
            }

            var impls = types.Where(t =>
                    t is { IsClass: true, IsAbstract: false, IsPublic: true } &&
                    IsTargetNamespace(t!.Namespace))
                .ToArray();

            foreach (var impl in impls)
            {
                var match = impl.GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"I{impl.Name}");

                if (match != null)
                {
                    services.AddScoped(match, impl);
                }
            }
        }

        return services;
    }
}
using System.Reflection;
using System.Runtime.Loader;

namespace Host;

/// <summary>
/// Carga un módulo (.dll) de forma aislada, pero redirige los assemblies
/// compartidos (Core, System.CommandLine) al contexto del host.
/// Esto garantiza que los tipos sean los mismos instancias en memoria.
/// </summary>
class ModuleLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    // Assemblies que el módulo comparte con el host → no recargar
    private static readonly HashSet<string> SharedAssemblies = new()
    {
        "Core",
        "System.CommandLine",
    };

    public ModuleLoadContext(string dllPath) : base(isCollectible: false)
    {
        _resolver = new AssemblyDependencyResolver(dllPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // null = "usá el contexto default (el del host)"
        if (SharedAssemblies.Contains(assemblyName.Name ?? string.Empty))
            return null;

        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        return path is not null ? LoadFromAssemblyPath(path) : null;
    }
}
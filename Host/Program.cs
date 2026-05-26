using Host;
using Host.Mainloop;
using Core;
using System;
using System.Reflection;
using Spectre.Console;

namespace Host;

public static class Program
{
    static async Task Main(string[] args)
    {
        var modulesPath = Path.Combine(AppContext.BaseDirectory, "modules");
        Directory.CreateDirectory(modulesPath);

        var modules = new List<IModule>();

        foreach (var dllPath in Directory.GetFiles(modulesPath, "*.dll"))
        {
            try
            {
                var ctx = new ModuleLoadContext(dllPath);
                var assembly = ctx.LoadFromAssemblyPath(dllPath);

                var moduleTypes = assembly.GetTypes()
                    .Where(t => typeof(IModule).IsAssignableFrom(t)
                             && !t.IsInterface
                             && !t.IsAbstract);

                foreach (var type in moduleTypes)
                {
                    var module = (IModule)Activator.CreateInstance(type)!;
                    modules.Add(module);
                    Console.WriteLine($"[+] {module.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[!] {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        var db = new Database("/home/oem/Documentos/Repos/Vida/Core/Database/life.db");
        var repo = new Repository(db);
        DatabaseInit.Initialize(repo);

        foreach (IModule mod in modules)
        {
            mod.GetDatabase(repo);
        }

        await MainLoop.Run(modules);
    }
}
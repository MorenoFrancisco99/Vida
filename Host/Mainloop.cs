using Spectre.Console;
using Core;
using System.Runtime.InteropServices;
using static SpectreConsoleUtilities;

namespace Host.Mainloop;

public static class MainLoop
{
    public async static Task<int> Run(List<IModule> modules)
    {
        if (modules.Count() == 0)
        {
            AnsiConsole.MarkupLine("[bold red]✗ Error:[/]: No Modules detected ");
            WaitForUserInput();
        }
        while (true)
        {

            Console.Clear();

            AnsiConsole.MarkupLine("[green]> Hola[/]");
            AnsiConsole.MarkupLine("[grey]?: Help - 0: Exit[/]");

            var input = AnsiConsole.Ask<string>("");

            if (input == "?")
            {
                ListModules(modules);
            }

            else if (input == "0")
                Environment.Exit(1);
            else
            {
                var modulo = modules.FirstOrDefault(m => m.Name == input);
                if (modulo == null)
                    continue;
                await modulo.RunAsync();
            }
        }

        return 1;
    }


    private static void ListModules(List<IModule> modules)
    {
        Console.Clear();
        AnsiConsole.MarkupLine("Listando modulos");

        foreach (IModule modulo in modules)
        {
            AnsiConsole.MarkupLine($"-[green]{modulo.Name}[/]");
        }
        AnsiConsole.Markup("[grey]Presione cualquier tecla...[/]");

        WaitForUserInput();
    }

    


}
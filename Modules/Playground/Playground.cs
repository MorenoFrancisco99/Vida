using Core;
using static SpectreConsoleUtilities;
using Spectre.Console;

namespace Playground;

public class Playground : IModule
{
    public string Name => "Playground";

    public string Description => "Module meant to  be for testing";

    public Repository DB { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void GetDatabase(Repository db)
    {

    }

    public async Task RunAsync()
    {
        var name = new TextPrompt<string>("What's your [green]name[/]?")
            .DefaultValue("Anonymous")
            .ShowDefaultValue();

        var result = AnsiConsole.Prompt(name);

        AnsiConsole.MarkupLine($"Hello, [blue]{result}[/]!");


        var color = new TextPrompt<string>("What's your favorite [green]color[/]?")
    .AddChoice("red")
    .AddChoice("green")
    .AddChoice("blue")
    .AddChoice("yellow")
    .ShowChoices();

        var asd = AnsiConsole.Prompt(color);

        AnsiConsole.MarkupLine($"You chose: [blue]{asd}[/]");

        var city = new TextPrompt<string>("What [yellow]city[/] do you live in?")
        .PromptStyle(new Style(Color.Yellow))
        .DefaultValue("London")
        .DefaultValueStyle(new Style(Color.Green, decoration: Decoration.Italic))
        .AddChoice("London")
        .AddChoice("New York")
        .AddChoice("Tokyo")
        .AddChoice("Paris")
        .ChoicesStyle(new Style(Color.Cyan));

        var dd = AnsiConsole.Prompt(city);

        AnsiConsole.MarkupLine($"City: [blue]{dd}[/]");

        WaitForUserInput();

        return;
    }
}

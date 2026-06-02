namespace Pomodoro;

using static SpectreConsoleUtilities;
using Core;
using Core.Types;
using Spectre.Console;
using System.Timers;
using Spectre.Console.Rendering;
using System.Diagnostics;

public class Pomodoro : IModule
{
    private static System.Timers.Timer Clock;

    public string Name => "Pomodoro";

    public string Description => "Manages pomodoro settings";

    public Repository DB { get; set; }

    public void GetDatabase(Repository db)
    {
        DB = db;
    }

    public async Task RunAsync()
    {
        TaskType taskType = null;
        int totalSeconds = 25 * 60;
        CancellationTokenSource? cts = null;

        while (true)
        {
            Console.Clear();

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Seleccione opcion")
                    .AddChoices("Begin", "Set Timer", "Salir"));


            if (taskType == null)
            {
                PromptForType(out taskType);
            }
            switch (action)
            {
                case "Begin":

                    cts = new CancellationTokenSource();
                    await RunTimerAsync(totalSeconds, cts.Token);
                    DB.Execute("INSERT INTO Pomodoro (Date, TypeId, Time) VALUES (@Date, @TypeId, @Time)",
                    new { Date = DateTime.Now.ToString(), TypeId = taskType.Id, Time = totalSeconds });
                    break;

                case "Set Timer":
                    totalSeconds = PromptForTime();
                    break;

                case "Salir":
                    return;
            }
        }
    }

    private async Task RunTimerAsync(int totalSeconds, CancellationToken ct)
    {
        await AnsiConsole.Live(BuildTimerPanel(totalSeconds))
            .StartAsync(async ctx =>
            {
                while (totalSeconds > 0 && !ct.IsCancellationRequested)
                {
                    await Task.Delay(1000, ct).ContinueWith(_ => { });
                    if (ct.IsCancellationRequested) break;

                    totalSeconds--;
                    ctx.UpdateTarget(BuildTimerPanel(totalSeconds));
                }

                if (totalSeconds == 0)
                {
                    await PlayFinishedAsync(ctx);
                }
            });
    }

    private async Task PlayFinishedAsync(LiveDisplayContext ctx)
    {
        bool visible = true;

        while (!Console.KeyAvailable)
        {
            // Parpadeo
            ctx.UpdateTarget(visible
                ? BuildTimerPanel(0, finished: true)
                : new Align(
                        new Panel(new FigletText("0:00")
                        {
                            Color = Color.Orange1,
                            Justification = Justify.Center
                        })
                        {
                            Border = BoxBorder.Double,
                            BorderStyle = new Style(Color.Orange1),
                            Width = 100
                        },
                        HorizontalAlignment.Center));

            visible = !visible;

            // Sonido cada ciclo visible (cada ~1 segundo efectivo)
            if (visible)
            {
                _ = Task.Run(() =>
                {
                    try
                    {
                        using var proc = Process.Start(new ProcessStartInfo
                        {
                            FileName = "paplay",
                            Arguments = "/usr/share/sounds/freedesktop/stereo/complete.oga",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        });
                        proc?.WaitForExit();
                    }
                    catch { }
                });
            }
            await Task.Delay(200);
            if (visible)
            {
                _ = Task.Run(() =>
                {
                    try
                    {
                        using var proc = Process.Start(new ProcessStartInfo
                        {
                            FileName = "paplay",
                            Arguments = "/usr/share/sounds/freedesktop/stereo/complete.oga",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        });
                        proc?.WaitForExit();
                    }
                    catch { }
                });
            }
            await Task.Delay(200);
             if (visible)
            {
                _ = Task.Run(() =>
                {
                    try
                    {
                        using var proc = Process.Start(new ProcessStartInfo
                        {
                            FileName = "paplay",
                            Arguments = "/usr/share/sounds/freedesktop/stereo/complete.oga",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        });
                        proc?.WaitForExit();
                    }
                    catch { }
                });
            }

            await Task.Delay(500);
        }

        Console.ReadKey(intercept: true);
    }

    private IRenderable BuildTimerPanel(int totalSeconds, bool finished = false)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        string timeStr = $"{minutes}:{seconds:D2}";

        var color = finished ? Color.Green : Color.Orange1;

        var figlet = new FigletText(timeStr)
        {
            Color = color,
            Justification = Justify.Center
        };

        var panel = new Panel(figlet)
        {
            Border = BoxBorder.Double,
            BorderStyle = new Style(color),
            Width = 100
        };

        return new Align(panel, HorizontalAlignment.Center);
    }

    private int PromptForTime()
    {
        int minutes = AnsiConsole.Prompt(
            new TextPrompt<int>("Minutos:")
                .ValidationErrorMessage("Número inválido")
                .Validate(v => v >= 0));

        int seconds = AnsiConsole.Prompt(
            new TextPrompt<int>("Segundos:")
                .ValidationErrorMessage("Número inválido")
                .Validate(v => v is >= 0 and < 60));

        return minutes * 60 + seconds;
    }

    private TaskType PromptForType(out TaskType tasktype)
    {

        var types = DB.Query<TaskType>("SELECT * FROM TaskType");
        var labels = types.Select(x => x.Name);
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Seleccione tipo de tarea")
                .AddChoices(labels)
            );

        return tasktype = types.FirstOrDefault(x => x.Name == selection)!;
    }


}

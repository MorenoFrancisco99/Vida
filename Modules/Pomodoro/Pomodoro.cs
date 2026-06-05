namespace Pomodoro;

using static SpectreConsoleUtilities;
using Core;
using Core.Types;
using Spectre.Console;
using System.Timers;
using Spectre.Console.Rendering;
using System.Diagnostics;
using System.Text;

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


    SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    bool isPaused = false;

    public async Task RunAsync()
    {
        TaskType taskType = null;
        int totalSeconds = 25 * 60;

        while (true)
        {
            Console.Clear();

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Seleccione opcion")
                    .AddChoices("Begin", "Set Timer", "Salir"));



            switch (action)
            {
                case "Begin":
                    PromptForType(out taskType);
                    await RunTimerAsync(totalSeconds, taskType);

                    break;

                case "Set Timer":
                    totalSeconds = PromptForTime();
                    break;

                case "Salir":
                    return;
            }
        }
    }

    private async Task RunTimerAsync(int totalSeconds, TaskType tType)
    {
        CancellationTokenSource? cts = new CancellationTokenSource();

        await AnsiConsole.Live(BuildTimerPanel(totalSeconds, taskType: tType))
            .StartAsync(async ctx =>
            {
                var CountdownTask = RunCountdownAsync(cts.Token, ctx, totalSeconds, tType);
                var InputTask = Task.Run(() =>
                {
                    while (true)
                    {
                        var key = Console.ReadKey(intercept: true).Key;
                        switch (key)
                        {
                            case ConsoleKey.P: Pause(); break;
                            case ConsoleKey.R: Resume(); break;
                            case ConsoleKey.C: cts.Cancel(); return;
                        }
                    }
                });

                await Task.WhenAny(CountdownTask, InputTask);

                //If CountDownTask ended we have to extract the value, otherwise resumes with the same value if necessary
                totalSeconds = CountdownTask.IsCompletedSuccessfully ? totalSeconds : totalSeconds - CountdownTask.Result;


                DB.Execute("INSERT INTO Pomodoro (Date, TypeId, Time) VALUES (@Date, @TypeId, @Time)",
                        new { Date = DateTime.Now.ToString(), TypeId = tType.Id, Time = totalSeconds });
                await PlayFinishedAsync(ctx, tType);

            });
    }

    private async Task<int> RunCountdownAsync(CancellationToken ct, LiveDisplayContext ctx, int totalSeconds, TaskType ttype)
    {
        while (totalSeconds > 0 && !ct.IsCancellationRequested)
        {
            //Check if the semaphore is down
            await semaphore.WaitAsync(ct);
            semaphore.Release();


            await Task.Delay(1000, ct).ContinueWith(_ => { });
            if (ct.IsCancellationRequested) return totalSeconds;

            totalSeconds--;
            ctx.UpdateTarget(BuildTimerPanel(totalSeconds, taskType: ttype));
        }
        return totalSeconds;
    }

    private async Task PlayFinishedAsync(LiveDisplayContext ctx, TaskType ttype)
    {
        bool visible = true;

        while (!Console.KeyAvailable)
        {
            // Parpadeo
            ctx.UpdateTarget(visible
                ? BuildTimerPanel(0, ttype, finished: true)
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

    private IRenderable BuildTimerPanel(int totalSeconds, TaskType taskType, bool finished = false)
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
        //.DoubleBorder()
        //.BorderColor(color)
        //.Width(100)
        //
        {
            Border = BoxBorder.Double,
            BorderStyle = new Style(color),
            Width = 100,
            Header = new PanelHeader($"Actualmente en: {taskType.Name}")
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

    private void Pause()
    {
        if (!isPaused)
        {
            semaphore.Wait();
            isPaused = true;
        }
    }

    private void Resume()
    {
        if (isPaused)
        {
            semaphore.Release();
            isPaused = false;
        }
    }
}

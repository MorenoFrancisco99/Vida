using Core;
using static SpectreConsoleUtilities;
namespace Tiempo;

using Spectre.Console;

public class Tiempo : IModule
{
    public string Name => "Schedule";
    public string Description => "Administra funciones relacionadas a rutina y deadlines";
    public Repository DB { get; set; }

    private record WeeklyCourse
    {
        public long Id { get; init; }
        public long Done { get; init; }
        public string BeginDate { get; init; }
        public string EndDate { get; init; }
        public string CourseName { get; init; }
        public string CourseFaculty { get; init; }

        public bool IsDone => Done == 1;
        public DateTime Begin => DateTime.Parse(BeginDate);
        public DateTime End => DateTime.Parse(EndDate);

        public override string ToString() => $"{CourseName} [{(IsDone ? "green]Hecho" : "red]Pendiente")}[/]";
    }

    private record PendingTask
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string EndDate { get; init; }
        public DateTime End => DateTime.Parse(EndDate);
        public override string ToString() => $"{Name} — vence {End:dd/MM/yyyy}";
    }

    // Tipo unificado para el selector
    private record Selectable(string Label, Action MarkDone);

    public void GetDatabase(Repository db)
    {
        if (DB != null) throw new Exception("Database for Schedule module already set");
        DB = db;
    }

    public async Task RunAsync()
    {
        var weekStatus = LoadWeekStatus();
        weekStatus = CheckAndResetWeek(weekStatus);

        while (true)
        {
            Console.Clear();
            RenderWeekStatus(weekStatus);
            RenderPendingTasks();

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Seleccione opcion")
                    .AddChoices("Marcar como hecho", "Salir"));

            switch (action)
            {
                case "Marcar como hecho":
                    weekStatus = SetDone(weekStatus);
                    break;
                case "Salir":
                    return;
            }

            WaitForUserInput();
        }
    }

    private IEnumerable<WeeklyCourse> LoadWeekStatus() => DB.Query<WeeklyCourse>("""
        SELECT 
            cw.Id,
            cw.Done,
            cw.BeginDate,
            cw.EndDate,
            c.Name  AS CourseName,
            c.Faculty AS CourseFaculty
        FROM CyberdefenseWeekly cw
        JOIN Course c ON cw.CourseId = c.Id
    """);

    private IEnumerable<WeeklyCourse> CheckAndResetWeek(IEnumerable<WeeklyCourse> weekStatus)
    {
        var today = DateTime.Today;
        var currentMonday = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));

        if (!weekStatus.Any() || weekStatus.First().Begin >= currentMonday)
            return weekStatus;

        foreach (var item in weekStatus.Where(x => !x.IsDone))
        {
            DB.Execute(
                "INSERT INTO Task (Name, BeginDate, EndDate, TypeId, Done) VALUES (@Name, @BeginDate, @EndDate, @TypeId, 0)",
                new { Name = $"Semanal pendiente: {item.CourseName}", BeginDate = item.BeginDate, EndDate = item.EndDate, TypeId = 1 });
        }

        DB.Execute("""
            UPDATE CyberdefenseWeekly 
            SET Done = 0, BeginDate = @BeginDate, EndDate = @EndDate
        """, new { BeginDate = currentMonday, EndDate = currentMonday.AddDays(7) });

        return LoadWeekStatus();
    }

    private void RenderWeekStatus(IEnumerable<WeeklyCourse> weekStatus)
    {
        AnsiConsole.MarkupLine("[yellow bold]── Ciberdefensa semanal ──[/]");
        foreach (var item in weekStatus)
            AnsiConsole.MarkupLine($"  {item}");

        AnsiConsole.WriteLine();
    }

    private void RenderPendingTasks()
    {
        var tasks = DB.Query<PendingTask>("""
            SELECT Id, Name, EndDate FROM Task WHERE Done = 0
        """);

        AnsiConsole.MarkupLine("[red bold]── Tareas pendientes ──[/]");
        if (!tasks.Any())
        {
            AnsiConsole.MarkupLine("[green]  Sin tareas pendientes[/]");
        }
        else
        {
            foreach (var task in tasks)
                AnsiConsole.MarkupLine($"  {task}");
        }

        AnsiConsole.WriteLine();
    }

    private IEnumerable<WeeklyCourse> SetDone(IEnumerable<WeeklyCourse> weekStatus)
    {
        var pendingWeek = weekStatus.Where(x => !x.IsDone).ToList();
        var pendingTasks = DB.Query<PendingTask>("SELECT Id, Name, EndDate FROM Task WHERE Done = 0").ToList();

        if (!pendingWeek.Any() && !pendingTasks.Any())
        {
            AnsiConsole.MarkupLine("[green]Todo está hecho.[/]");
            return weekStatus;
        }

        var choices = new List<(string Label, Action Act)>();

        foreach (var w in pendingWeek)
            choices.Add(($"[blue]Semanal[/] {w.CourseName}", () =>
                DB.Execute("UPDATE CyberdefenseWeekly SET Done = 1 WHERE Id = @Id", new { w.Id })));

        foreach (var t in pendingTasks)
            choices.Add(($"[red]Task[/] {t.Name}", () =>
                DB.Execute("UPDATE Task SET Done = 1 WHERE Id = @Id", new { t.Id })));

        var labels = choices.Select(x => x.Label).ToList();

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("¿Qué marcás como hecho?")
                .AddChoices(labels));

        choices.First(x => x.Label == selected).Act();

        return LoadWeekStatus();
    }

    

}
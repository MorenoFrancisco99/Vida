using Core;
using static SpectreConsoleUtilities;
namespace Tiempo;

using System.Collections.Immutable;
using Spectre.Console;

public class Tiempo : IModule
{
    public string Name => "Schedule";

    public string Description => "Administra funciones relacionadas a rutina y deadlines";

    public Repository DB { get; set; }

    private record Course(int Id, string Name, string Faculty);
    public record WeeklyCyberdefenseStatus
    {
        public int Id { get; init; }
        public int Done { get; init; }
        public string BeginDate { get; init; }
        public string EndDate { get; init; }
        public string CourseName { get; init; }
        public string CourseFaculty { get; init; }

        public bool IsDone => Done == 1;
        public DateTime Begin => DateTime.Parse(BeginDate);
        public DateTime End => DateTime.Parse(EndDate);
    };

    private record UndoneTask
    {
        public int? Id { get; init; }
        public string Content { get; init; }
        public string BeginDate { get; init; }
        public string EndDate { get; init; }

        public DateTime Begin => DateTime.Parse(BeginDate);
        public DateTime End => DateTime.Parse(EndDate);
    };


    public void GetDatabase(Repository db)
    {
        if (DB != null) throw new Exception($"Database for Schedule module already settle");
        DB = db;
    }


    public async Task RunAsync()
    {
        var CBWeekStatus = DB.Query<WeeklyCyberdefenseStatus>("""
                SELECT 
                    cw.Id,
                    cw.Done,
                    cw.BeginDate,
                    cw.EndDate,
                    c.Name AS CourseName,
                    c.Faculty AS CourseFaculty
                FROM CyberdefenseWeekly cw
                JOIN Course c ON cw.CourseId = c.Id
            """);

        var today = DateTime.Today;
        var currentMonday = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));

        var isNewWeek = CBWeekStatus.Any() && CBWeekStatus.First().Begin < currentMonday;

        if (isNewWeek)
        {
            foreach (var task in CBWeekStatus.Where(x => !x.IsDone)) //save undone tasks
            {
                DB.Execute(
                    "INSERT INTO UndoneTask (Content, BeginDate, EndDate) VALUES (@Content, @BeginDate, @EndDate)",
                    new { Content = task.CourseName, task.BeginDate, task.EndDate }
                );
            }

            //weekly cyberdefense reset
            DB.Execute("""
                UPDATE CyberdefenseWeekly 
                SET Done = 0, BeginDate = @BeginDate, EndDate = @EndDate
             """, new { BeginDate = currentMonday, EndDate = currentMonday.AddDays(7) });


            //refresh week status
            CBWeekStatus = DB.Query<WeeklyCyberdefenseStatus>(""" 
                SELECT 
                    cw.Id,
                    cw.Done,
                    cw.BeginDate,
                    cw.EndDate,
                    c.Name AS CourseName,
                    c.Faculty AS CourseFaculty
                FROM CyberdefenseWeekly cw
                JOIN Course c ON cw.CourseId = c.Id
            """);
        }


        var undoneStatus = DB.Query<UndoneTask>("SELECT * FROM UndoneTask");


        while (true)
        {
            AnsiConsole.MarkupLine("[yellow]Weekly Tasks[/]");
            foreach (WeeklyCyberdefenseStatus item in CBWeekStatus)
            {
                AnsiConsole.MarkupLine($"[blue{item.CourseName}[/]  {item.IsDone} |");
            }


            AnsiConsole.MarkupLine("[bold red]Weekly Tasks[/]");
            if (undoneStatus.Count() == 0)
            {
                AnsiConsole.MarkupLine("[bold green]Nothing to show[/]");
            }
            else
            {
                foreach (UndoneTask task in undoneStatus)
                {
                    AnsiConsole.Markup($"{task.Content}");
                    AnsiConsole.Markup($"Expired: {task.End}");

                }
            }

            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Seleccione opcion")
                        .AddChoices("Marcar hecha"));


            switch (prompt)
            {
                case "Marcar hecha":

                    var cbWeek = CBWeekStatus.Where(x => !x.IsDone);
                    var uTasks = undoneStatus;
                    SetDone(cbWeek, uTasks);
                    break;
            }


            WaitForUserInput();
            Console.Clear();
        }

    }
    private static void SetDone(IEnumerable<WeeklyCyberdefenseStatus> cbWeek, IEnumerable<UndoneTask> undoneTasks)
    {
        foreach (WeeklyCyberdefenseStatus item in cbWeek)
        {
            
        }
    }









}

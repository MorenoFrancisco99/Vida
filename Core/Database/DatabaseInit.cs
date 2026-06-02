public static class DatabaseInit
{
    public static void Initialize(Repository repo)
    {

        // Crear tablas base
        CreateTables(repo);

        // Aplicar migraciones pendientes
        Migrate(repo);


        SetInitialValues(repo);
    }

    private static void Migrate(Repository repo)
    {
        //var version = repo.QuerySingle<int?>("SELECT Version FROM SchemaVersion") ?? 0;

        //if (version < 1)
        //{
        //    repo.Execute("ALTER TABLE Courses ADD COLUMN Description TEXT");
        //    SetVersion(repo, 1);
        //}

        // Mañana agregás otro campo:
        // if (version < 2)
        // {
        //     repo.Execute("ALTER TABLE Tasks ADD COLUMN Priority INTEGER NOT NULL DEFAULT 0");
        //     SetVersion(repo, 2);
        // }
    }

    private static void SetVersion(Repository repo, int version)
    {
        repo.Execute("""
            CREATE TABLE IF NOT EXISTS SchemaVersion (
                Version INTEGER NOT NULL
            )
        """);

        var exists = repo.QuerySingle<int>("SELECT COUNT(*) FROM SchemaVersion") > 0;

        if (exists)
            repo.Execute("UPDATE SchemaVersion SET Version = @Version", new { Version = version });
        else
            repo.Execute("INSERT INTO SchemaVersion (Version) VALUES (@Version)", new { Version = version });
    }


    public static void CreateTables(Repository repo)
    {
        repo.Execute("""
            CREATE TABLE IF NOT EXISTS Course (
                Id      INTEGER PRIMARY KEY AUTOINCREMENT,
                Name    TEXT NOT NULL,
                Faculty TEXT NOT NULL
            )
        """);

        repo.Execute("""
            CREATE TABLE IF NOT EXISTS CyberdefenseWeekly (
                Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                CourseId  INTEGER NOT NULL,
                Done      INTEGER NOT NULL DEFAULT 0,
                BeginDate TEXT NOT NULL,
                EndDate   TEXT NOT NULL,
                FOREIGN KEY (CourseId) REFERENCES Course(Id)
            )
        """);


        repo.Execute("""
            CREATE TABLE IF NOT EXISTS TaskType (
                Id   INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            )
        """);

        repo.Execute("""
            CREATE TABLE IF NOT EXISTS Task (
                Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                Name      TEXT,
                BeginDate TEXT NOT NULL,
                EndDate   TEXT,
                CourseId  INTEGER,
                TypeId    INTEGER NOT NULL,
                Done      INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (CourseId) REFERENCES Course(Id),
                FOREIGN KEY (TypeId)   REFERENCES TaskType(Id)
            )
        """);
    
    
        repo.Execute("""
            CREATE TABLE IF NOT EXISTS Pomodoro (
                Id      INTEGER PRIMARY KEY AUTOINCREMENT,
                Date    TEXT NOT NULL,
                TypeId  INTEGER NOT NULL,
                Time    TEXT,
                FOREIGN KEY (TypeId)    REFERENCES TaskType(Id)
            )
        """);
    }

    private static void SetInitialValues(Repository repo)
    {
        var exists = repo.QuerySingle<int>("SELECT COUNT(*) FROM Course") > 0;
        if (exists) return;

        string[] courses = [
            "Tecnologia Operativa",
        "Ingles II",
        "Analisis Matematico II",
        "Algebra II",
        "Probabilidad y Estadistica"
        ];
        string[] types =
        {
            "Facultry",
            "DailyLife"
        };

        foreach (string course in courses)
        {
            var courseId = repo.QuerySingle<int>(
                "INSERT INTO Course (Name, Faculty) VALUES (@Name, @Faculty) RETURNING Id",
                new { Name = course, Faculty = "FADENA" });

            repo.Execute(
                """
                INSERT INTO CyberdefenseWeekly (CourseId, Done, BeginDate, EndDate)
                VALUES (@CourseId, @Done, @BeginDate, @EndDate)
                """,
                new
                {
                    CourseId = courseId,
                    Done = true,
                    BeginDate = new DateTime(2026, 5, 11),
                    EndDate = new DateTime(2026, 5, 18)//  intentionally expired so a new task is genereated this week
                });
        }

        foreach (string type in types)
        {
            repo.Execute(
                "INSERT INTO TaskType (Name) VALUES (@Name)",
                new { Name = type }
            );
        }

    }
}

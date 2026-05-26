using Spectre.Console;
namespace Cheatsheet.Spectre;

public static class SpectreConsole
{
    public static void ShowSpectre()
    {
        Section("MARKUP — ESTILOS DE TEXTO");
        ShowTextStyles();

        Section("MARKUP — COLORES NOMBRADOS");
        ShowNamedColors();

        Section("MARKUP — HEX · RGB · FONDOS · COMBINADOS");
        ShowColorFormats();

        Section("TABLAS — TODOS LOS BORDES (18 estilos)");
        ShowAllTableBorders();

        Section("TABLAS — CARACTERÍSTICAS");
        ShowTableFeatures();

        Section("PANEL — TODOS LOS BORDES (7 estilos)");
        ShowAllPanelBorders();

        Section("PANEL — PADDING · COLOR · EXPAND");
        ShowPanelFeatures();

        Section("TREE — ESTILOS DE GUÍA (4 estilos)");
        ShowTreeGuides();

        Section("RULE — SEPARADORES");
        ShowRules();

        Section("FIGLET — ASCII ART");
        ShowFiglet();

        Section("GRID — KEY/VALUE");
        ShowGrid();

        Section("COLUMNS — DISTRIBUCIÓN AUTOMÁTICA");
        ShowColumns();

        Section("ROWS — APILAR RENDERABLES");
        ShowRows();

        Section("PADDER · ALIGN");
        ShowPadderAndAlign();

        Section("BARCHART");
        ShowBarChart();

        Section("BREAKDOWNCHART");
        ShowBreakdownChart();

        Section("CALENDAR");
        ShowCalendar();

        Section("CANVAS — PIXEL ART");
        ShowCanvas();

        Section("TEXTPATH");
        ShowTextPath();

        Section("PROMPTS — REPRESENTACIÓN ESTÁTICA");
        ShowPromptsStatic();

        Section("EXCEPCIONES — WriteException real");
        ShowException();
    }

    // ────────────────────────────────────────────────────────────────────────
    // Helpers
    // ────────────────────────────────────────────────────────────────────────

    private static void Section(string title)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[bold yellow on default] {title} [/]")
        {
            Justification = Justify.Left,
            Style = Style.Parse("yellow"),
        });
        AnsiConsole.WriteLine();
    }

    private static void Sub(string title) =>
        AnsiConsole.MarkupLine($"\n[bold dim]▸ {title}[/]");

    private static Table SampleTable(TableBorder border) =>
        new Table()
            .Border(border)
            .AddColumn("Producto")
            .AddColumn("Precio")
            .AddColumn("Stock")
            .AddRow("Manzana", "$1.50", "120")
            .AddRow("Banana", "$0.80", "45")
            .AddRow("Kiwi", "$2.00", "8");

    // ────────────────────────────────────────────────────────────────────────
    // MARKUP
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowTextStyles()
    {
        AnsiConsole.MarkupLine("[bold]          bold[/]           → negrita");
        AnsiConsole.MarkupLine("[dim]           dim[/]            → tenue / opaco");
        AnsiConsole.MarkupLine("[italic]        italic[/]         → cursiva");
        AnsiConsole.MarkupLine("[underline]     underline[/]      → subrayado");
        AnsiConsole.MarkupLine("[strikethrough] strikethrough[/]  → tachado");
        AnsiConsole.MarkupLine("[invert]        invert[/]         → invierte fg y bg");
        AnsiConsole.MarkupLine("[bold italic underline red] bold + italic + underline + red[/]");
        AnsiConsole.MarkupLine("[link=https://spectreconsole.net]link (ctrl+click en terminales que soporten)[/]");
        AnsiConsole.MarkupLine("[[corchetes literales]]  →  [[escapados con doble bracket]]");
    }

    private static void ShowNamedColors()
    {
        var colors = new[]
        {
            "black", "white", "red", "green", "blue", "yellow",
            "cyan", "magenta", "grey", "orange1", "deepskyblue1",
            "mediumpurple", "springgreen1", "hotpink", "gold1",
            "indianred", "chartreuse1", "aquamarine1", "cornflowerblue",
            "salmon1", "violet", "turquoise2", "pink1", "steelblue1",
        };

        foreach (var c in colors)
            AnsiConsole.MarkupLine($"  [{c}]■■■[/]  [dim]{c}[/]");
    }

    private static void ShowColorFormats()
    {
        Sub("Hex");
        AnsiConsole.MarkupLine("[#ff6600]■ #ff6600[/]   [#00ccff]■ #00ccff[/]   [#44ff88]■ #44ff88[/]");

        Sub("RGB");
        AnsiConsole.MarkupLine("[rgb(255,100,0)]■ rgb(255,100,0)[/]   [rgb(0,180,255)]■ rgb(0,180,255)[/]");

        Sub("Fondos");
        AnsiConsole.MarkupLine("[on blue]   texto sobre fondo azul   [/]");
        AnsiConsole.MarkupLine("[black on yellow]   negro sobre amarillo   [/]");
        AnsiConsole.MarkupLine("[white on red]   blanco sobre rojo   [/]");
        AnsiConsole.MarkupLine("[on #222222]   texto sobre fondo hex oscuro   [/]");

        Sub("Combinados");
        AnsiConsole.MarkupLine("[bold green on black]  bold + green + on black  [/]");
        AnsiConsole.MarkupLine("[italic white on blue] italic + white + on blue [/]");
        AnsiConsole.MarkupLine("[underline yellow]     underline + yellow       [/]");
    }

    // ────────────────────────────────────────────────────────────────────────
    // TABLES
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowAllTableBorders()
    {
        var borders = new (string Name, TableBorder Border)[]
        {
            ("None",              TableBorder.None),
            ("Ascii",             TableBorder.Ascii),
            ("Ascii2",            TableBorder.Ascii2),
            ("AsciiDoubleHead",   TableBorder.AsciiDoubleHead),
            ("Square",            TableBorder.Square),
            ("Minimal",           TableBorder.Minimal),
            ("MinimalHeavyHead",  TableBorder.MinimalHeavyHead),
            ("MinimalDoubleHead", TableBorder.MinimalDoubleHead),
            ("Simple",            TableBorder.Simple),
            ("SimpleHeavy",       TableBorder.SimpleHeavy),
            ("Horizontal",        TableBorder.Horizontal),
            ("Rounded",           TableBorder.Rounded),
            ("Heavy",             TableBorder.Heavy),
            ("HeavyEdge",         TableBorder.HeavyEdge),
            ("HeavyHead",         TableBorder.HeavyHead),
            ("Double",            TableBorder.Double),
            ("DoubleEdge",        TableBorder.DoubleEdge),
            ("Markdown",          TableBorder.Markdown),
        };

        foreach (var (name, border) in borders)
        {
            AnsiConsole.MarkupLine($"\n[dim]TableBorder.[/][bold cyan]{name}[/]");
            AnsiConsole.Write(SampleTable(border));
        }
    }

    private static void ShowTableFeatures()
    {
        Sub("Título y Caption");
        AnsiConsole.Write(new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold blue]Inventario[/]")
            .Caption("[dim]Actualizado hoy[/]")
            .AddColumn("Item")
            .AddColumn(new TableColumn("[green]Stock[/]").RightAligned())
            .AddRow("Producto A", "100")
            .AddRow("Producto B", "25"));

        Sub("Alineaciones de columna: Left · Center · Right");
        AnsiConsole.Write(new Table()
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("[grey]Left[/]").LeftAligned())
            .AddColumn(new TableColumn("[grey]Center[/]").Centered())
            .AddColumn(new TableColumn("[grey]Right[/]").RightAligned())
            .AddRow("aaa", "bbb", "ccc")
            .AddRow("izquierda", "centro", "derecha"));

        Sub("HideHeaders — ideal para key/value");
        AnsiConsole.Write(new Table()
            .Border(TableBorder.Simple)
            .HideHeaders()
            .AddColumn("k")
            .AddColumn("v")
            .AddRow("[bold]Nombre[/]", "Erick")
            .AddRow("[bold]Rol[/]", "Backend Dev")
            .AddRow("[bold]Stack[/]", "C# / .NET 8")
            .AddRow("[bold]Tests[/]", "[green]19 passing[/]"));

        Sub("BorderColor + Expand (ocupa todo el ancho)");
        AnsiConsole.Write(new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Blue)
            .Expand()
            .AddColumn("Columna A")
            .AddColumn("Columna B")
            .AddRow("dato 1", "dato 2")
            .AddRow("dato 3", "dato 4"));

        Sub("Renderables en celdas (Panel dentro de celda)");
        AnsiConsole.Write(new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Widget")
            .AddColumn("Markup")
            .AddColumn("Text")
            .AddRow(
                new Panel("[green]OK[/]") { Border = BoxBorder.Rounded },
                new Markup("[bold red]Error[/]"),
                new Text("texto plano")));
    }

    // ────────────────────────────────────────────────────────────────────────
    // PANEL
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowAllPanelBorders()
    {
        var borders = new (string Name, BoxBorder Border)[]
        {
            ("Ascii",       BoxBorder.Ascii),
            ("Double",      BoxBorder.Double),
            ("Heavy",       BoxBorder.Heavy),
            ("Rounded",     BoxBorder.Rounded),
            ("Square",      BoxBorder.Square),
            ("None",        BoxBorder.None),
        };

        foreach (var (name, border) in borders)
        {
            AnsiConsole.MarkupLine($"\n[dim]BoxBorder.[/][bold cyan]{name}[/]");
            AnsiConsole.Write(new Panel($"Contenido — borde [bold]{name}[/]")
            {
                Header = new PanelHeader($"[bold] {name} [/]", Justify.Center),
                Border = border,
            });
        }
    }

    private static void ShowPanelFeatures()
    {
        Sub("Padding(4, 1, 4, 1)");
        AnsiConsole.Write(new Panel("Texto con padding horizontal grande")
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(4, 1, 4, 1),
            Header = new PanelHeader("[blue] Padding(4,1,4,1) [/]"),
        });

        Sub("BorderStyle — color de borde");
        AnsiConsole.Write(new Panel("[green]Borde coloreado en verde[/]")
        {
            Border = BoxBorder.Heavy,
            BorderStyle = Style.Parse("green"),
            Header = new PanelHeader("[green bold] BorderStyle green [/]"),
        });

        Sub("Expand = true");
        AnsiConsole.Write(new Panel("[dim]Este panel ocupa todo el ancho disponible[/]")
        {
            Border = BoxBorder.Rounded,
            Expand = true,
            Header = new PanelHeader("[bold yellow] Expand = true [/]", Justify.Center),
        });

        Sub("Panel anidado");
        AnsiConsole.Write(new Panel(
            new Panel("Panel interior")
            {
                Border = BoxBorder.Rounded,
                Header = new PanelHeader("[blue] Interior [/]", Justify.Center),
            })
        {
            Border = BoxBorder.Heavy,
            Header = new PanelHeader("[bold] Exterior [/]", Justify.Center),
        });

        Sub("Header: Left · Center · Right");
        foreach (var (label, justify) in new[] {
            ("Left",   Justify.Left),
            ("Center", Justify.Center),
            ("Right",  Justify.Right) })
        {
            AnsiConsole.Write(new Panel($"Justificación [bold]{label}[/]")
            {
                Border = BoxBorder.Square,
                Header = new PanelHeader($"[bold] {label} [/]", justify),
            });
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    // TREE
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowTreeGuides()
    {
        var guides = new (string Name, TreeGuide Guide)[]
        {
            ("Line",       TreeGuide.Line),
            ("BoldLine",   TreeGuide.BoldLine),
            ("DoubleLine", TreeGuide.DoubleLine),
            ("Ascii",      TreeGuide.Ascii),
        };

        foreach (var (name, guide) in guides)
        {
            AnsiConsole.MarkupLine($"\n[dim]TreeGuide.[/][bold cyan]{name}[/]");
            var root = new Tree("[bold]src/[/]") { Guide = guide };
            var controllers = root.AddNode("[blue]Controllers/[/]");
            controllers.AddNode("ProductController.cs");
            controllers.AddNode("UserController.cs");
            var models = root.AddNode("[green]Models/[/]");
            models.AddNode("Product.cs");
            models.AddNode("Category.cs");
            root.AddNode("Program.cs");
            root.AddNode("appsettings.json");
            AnsiConsole.Write(root);
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    // RULE
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowRules()
    {
        Sub("Sin título");
        AnsiConsole.Write(new Rule());

        Sub("Título centrado (default)");
        AnsiConsole.Write(new Rule("[bold]Sección[/]"));

        Sub("Título izquierda");
        AnsiConsole.Write(new Rule("[bold]Título[/]") { Justification = Justify.Left });

        Sub("Título derecha");
        AnsiConsole.Write(new Rule("[bold]Título[/]") { Justification = Justify.Right });

        Sub("Con estilo de color");
        AnsiConsole.Write(new Rule("[blue]Sección azul[/]") { Style = Style.Parse("blue") });

        Sub("Estilo bold red");
        AnsiConsole.Write(new Rule("[red]Sección de errores[/]") { Style = Style.Parse("bold red") });

        Sub("Estilo grey dim");
        AnsiConsole.Write(new Rule("[grey]Sección secundaria[/]") { Style = Style.Parse("grey dim") });
    }

    // ────────────────────────────────────────────────────────────────────────
    // FIGLET
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowFiglet()
    {
        Sub("LeftAligned · Color.Blue");
        AnsiConsole.Write(new FigletText("SPECTRE")
        {
            Justification = Justify.Left
        }.Color(Color.Blue));

        Sub("Centered · Color.Green");
        AnsiConsole.Write(new FigletText("CLI").Color(Color.Green).Centered());

        Sub("RightAligned · Color.Red");
        AnsiConsole.Write(new FigletText("APP")
        {
            Justification = Justify.Right
        }.Color(Color.Red));
    }

    // ────────────────────────────────────────────────────────────────────────
    // LAYOUT
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowGrid()
    {
        var grid = new Grid();
        grid.AddColumn(new GridColumn().NoWrap().PadRight(2));
        grid.AddColumn();
        grid.AddRow("[bold]Nombre:[/]", "Erick");
        grid.AddRow("[bold]Rol:[/]", "Backend Dev");
        grid.AddRow("[bold]Stack:[/]", "C# / .NET 8");
        grid.AddRow("[bold]Proyecto:[/]", "ProductSeeker");
        grid.AddRow("[bold]Tests:[/]", "[green]19 passing ✓[/]");
        grid.AddRow("[bold]DB:[/]", "SQL Server + EF Core + NTS");
        AnsiConsole.Write(grid);
    }

    private static void ShowColumns()
    {
        AnsiConsole.Write(new Columns(
            new Panel("[blue]Widget A[/]").Border(BoxBorder.Rounded),
            new Panel("[green]Widget B[/]").Border(BoxBorder.Rounded),
            new Panel("[red]Widget C[/]").Border(BoxBorder.Rounded),
            new Panel("[yellow]Widget D[/]").Border(BoxBorder.Rounded),
            new Panel("[magenta]Widget E[/]").Border(BoxBorder.Rounded)
        ));
    }

    private static void ShowRows()
    {
        AnsiConsole.Write(new Rows(
            new Markup("[bold]─── Producto A ───[/]"),
            new Markup("  Precio: [green]$10.00[/]   Stock: [yellow]120[/]"),
            new Rule(),
            new Markup("[bold]─── Producto B ───[/]"),
            new Markup("  Precio: [green]$5.00[/]    Stock: [red]3[/]")
        ));
    }

    private static void ShowPadderAndAlign()
    {
        Sub("Padder — Padding(4, 1, 4, 1)");
        AnsiConsole.Write(new Padder(
            new Panel("[green]Con padding[/]").Border(BoxBorder.Rounded),
            new Padding(4, 1, 4, 1)
        ));

        Sub("Align — HorizontalAlignment.Center");
        AnsiConsole.Write(new Align(
            new Panel("[cyan]Centrado[/]").Border(BoxBorder.Rounded),
            HorizontalAlignment.Center
        ));

        Sub("Align — HorizontalAlignment.Right");
        AnsiConsole.Write(new Align(
            new Panel("[yellow]Derecha[/]").Border(BoxBorder.Rounded),
            HorizontalAlignment.Right
        ));
    }

    // ────────────────────────────────────────────────────────────────────────
    // CHARTS
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowBarChart()
    {
        AnsiConsole.Write(new BarChart()
            .Width(60)
            .Label("[bold blue]Ventas mensuales 2025[/]")
            .CenterLabel()
            .AddItem("Enero", 42, Color.Blue)
            .AddItem("Febrero", 87, Color.Green)
            .AddItem("Marzo", 63, Color.Red)
            .AddItem("Abril", 95, Color.Yellow)
            .AddItem("Mayo", 71, Color.Cyan1)
            .AddItem("Junio", 50, Color.Magenta1)
            .AddItem("Julio", 38, Color.Orange1)
            .AddItem("Agosto", 82, Color.Chartreuse1));
    }

    private static void ShowBreakdownChart()
    {
        AnsiConsole.Write(new BreakdownChart()
            .Width(60)
            .AddItem("C#", 45.2, Color.Blue)
            .AddItem("Python", 30.1, Color.Green)
            .AddItem("Rust", 15.7, Color.Red)
            .AddItem("Go", 5.0, Color.Cyan1)
            .AddItem("Otros", 4.0, Color.Grey)
            .UseValueFormatter(v => $"{v:F1}%"));
    }

    // ────────────────────────────────────────────────────────────────────────
    // CALENDAR
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowCalendar()
    {
        var today = DateTime.Today;
        var cal = new Calendar(today.Year, today.Month)
            .AddCalendarEvent(today.Year, today.Month, 1)
            .AddCalendarEvent(today.Year, today.Month, today.Day)
            .AddCalendarEvent(today.Year, today.Month, Math.Min(today.Day + 5, 28))
            .HeaderStyle(Style.Parse("bold blue"))
            .HighlightStyle(Style.Parse("bold yellow on blue"))
            .Border(TableBorder.Rounded);
        AnsiConsole.Write(cal);
    }

    // ────────────────────────────────────────────────────────────────────────
    // CANVAS
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowCanvas()
    {
        Sub("Tablero de ajedrez 8×8");
        var chess = new Canvas(8, 8);
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                chess.SetPixel(x, y, (x + y) % 2 == 0 ? Color.White : Color.Grey15);
        AnsiConsole.Write(chess);

        Sub("Gradiente horizontal 32×4");
        var gradient = new Canvas(32, 4);
        Color[] ramp =
        [
            Color.Blue, Color.Blue1, Color.DeepSkyBlue1, Color.Cyan1,
            Color.SpringGreen1, Color.Chartreuse1, Color.Yellow, Color.Orange1, Color.Red,
        ];
        for (int x = 0; x < 32; x++)
        {
            var color = ramp[x * ramp.Length / 32];
            for (int y = 0; y < 4; y++)
                gradient.SetPixel(x, y, color);
        }
        AnsiConsole.Write(gradient);

        Sub("Patrón diagonal 16×16");
        var diag = new Canvas(16, 16);
        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                diag.SetPixel(x, y, (x + y) % 4 == 0 ? Color.Cyan1 : Color.Grey15);
        AnsiConsole.Write(diag);
    }

    // ────────────────────────────────────────────────────────────────────────
    // TEXTPATH
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowTextPath()
    {
        Sub("Unix path");
        var unix = new TextPath("/home/erick/projects/ProductSeeker/src/Controllers/ProductController.cs")
        {
            RootStyle = Style.Parse("bold blue"),
            SeparatorStyle = Style.Parse("grey"),
            StemStyle = Style.Parse("dim"),
            LeafStyle = Style.Parse("bold green"),
        }.LeftJustified();

        //ALTERNATIVA
        // var unix = (...)
        //.RootStole


        AnsiConsole.Write(unix);

        Sub("Windows path");
        var win = new TextPath(@"C:\Users\Erick\source\repos\ProductSeeker\appsettings.Development.json")
        {
            RootStyle = Style.Parse("bold yellow"),
            SeparatorStyle = Style.Parse("grey"),
            StemStyle = Style.Parse("dim"),
            LeafStyle = Style.Parse("bold white"),
        }.RightJustified();
        AnsiConsole.Write(win);
    }

    // ────────────────────────────────────────────────────────────────────────
    // PROMPTS — representación estática
    // (los prompts son interactivos; esto muestra cómo se verían en pantalla)
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowPromptsStatic()
    {
        Sub("TextPrompt — entrada básica (Ask<T>)");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]¿Nombre?[/] [grey][[Anon]][/]: [green]Erick[/]");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]¿Edad?[/]:              [green]21[/]");
        
        //var name = AnsiConsole.Ask<string>("What's your [green]name[/]?");
        //AnsiConsole.MarkupLine($"Hello, [blue]{name}[/]!");

        Sub("TextPrompt — validación fallida y reintento");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]Nombre:[/] [green]AB[/]");
        AnsiConsole.MarkupLine("[red]» Mínimo 3 caracteres[/]");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]Nombre:[/] [green]Erick[/]");



        Sub("TextPrompt — .Secret() (contraseña)");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]Password:[/] [red]••••••••[/]");

        Sub("Confirm()");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]¿Continuar?[/] [grey][[y/n]][/]: [green]y[/]");

        //if (AnsiConsole.Confirm("Continue with installation?"))
        //{
        //    AnsiConsole.MarkupLine("[green]Installing...[/]");
        //}


        Sub("SelectionPrompt — idle (cursor en primera opción)");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]¿Fruta favorita?[/]");
        AnsiConsole.MarkupLine("[blue]>[/] [bold on default]Manzana[/]");
        AnsiConsole.MarkupLine("  [dim]Pera[/]");
        AnsiConsole.MarkupLine("  [dim]Uva[/]");
        AnsiConsole.MarkupLine("  [dim]Kiwi[/]");
        AnsiConsole.MarkupLine("[grey]  (↑↓ para más opciones)[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim](después de seleccionar Uva, el menú colapsa a:)[/]");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]¿Fruta favorita?[/] [green]Uva[/]");
        
        //var choice = AnsiConsole.Prompt(
        //    new SelectionPrompt<string>()
        //        .Title("Select an [green]environment[/]:")
        //        .AddChoices("Development", "Staging", "Production"));
        //
        //AnsiConsole.MarkupLine($"Deploying to [blue]{choice}[/]");

        Sub("SelectionPrompt — con grupos (AddChoiceGroup)");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]Framework:[/]");
        AnsiConsole.MarkupLine("  [grey dim]Backend[/]");
        AnsiConsole.MarkupLine("[blue]>[/] [bold].NET[/]");
        AnsiConsole.MarkupLine("  [dim]Node[/]");
        AnsiConsole.MarkupLine("  [dim]Django[/]");
        AnsiConsole.MarkupLine("  [grey dim]Frontend[/]");
        AnsiConsole.MarkupLine("  [dim]React[/]");
        AnsiConsole.MarkupLine("  [dim]Vue[/]");
        AnsiConsole.MarkupLine("  [dim]Svelte[/]");

        Sub("MultiSelectionPrompt — seleccionando");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]Lenguajes:[/]");
        AnsiConsole.MarkupLine("[grey]  (↑↓ mover, SPACE marcar, ENTER confirmar)[/]");
        AnsiConsole.MarkupLine("  [green][[x]][/] C#");
        AnsiConsole.MarkupLine("  [dim][[ ]][/]  Python");
        AnsiConsole.MarkupLine("[blue]>[/] [green][[x]][/] Go");
        AnsiConsole.MarkupLine("  [grey dim]Web[/]");
        AnsiConsole.MarkupLine("  [dim][[ ]][/]  JS");
        AnsiConsole.MarkupLine("  [green][[x]][/] TS");
        AnsiConsole.MarkupLine("  [dim][[ ]][/]  CSS");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim](después de confirmar, colapsa a:)[/]");
        AnsiConsole.MarkupLine("[blue]?[/] [bold]Lenguajes:[/] [green]C#, Go, TS[/]");

        //var features = AnsiConsole.Prompt(
        //    new MultiSelectionPrompt<string>()
        //        .Title("Select [green]features[/] to enable:")
        //        .AddChoices("Logging", "Caching", "Authentication", "Analytics"));
        //
        //AnsiConsole.MarkupLine($"Enabled: [blue]{string.Join(", ", features)}[/]");


    }

    // ────────────────────────────────────────────────────────────────────────
    // EXCEPCIONES
    // ────────────────────────────────────────────────────────────────────────

    private static void ShowException()
    {
        Sub("WriteException con ExceptionFormats");
        try
        {
            throw new InvalidOperationException(
                "Estado inválido: el producto no existe en el repositorio.",
                new KeyNotFoundException("ProductId '9f3a' no encontrado en DbSet<Product>."));
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex,
                ExceptionFormats.ShortenPaths |
                ExceptionFormats.ShortenTypes |
                ExceptionFormats.ShowLinks);
        }

        Sub("WriteException sin opciones (paths y tipos completos)");
        try
        {
            _ = int.Parse("no-es-un-numero");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }
}
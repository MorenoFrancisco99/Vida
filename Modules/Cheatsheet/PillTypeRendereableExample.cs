using Spectre.Console;
using Spectre.Console.Rendering;


/*
Let's start by defining a PillType enum and creating a class that implements IRenderable. 
This interface requires two methods: Measure() to report size constraints and Render() to produce output.

The PillType enum defines four semantic variants with predefined color schemes. 
The Pill class maps these types to styles internally, making it easy to create consistent status indicators.
*/
public enum PillType
{
    Success,
    Warning,
    Error,
    Info,
}

public sealed class Pill : IRenderable
{
    private readonly string _text;
    private readonly Style _style;

    /// <summary>
    /// Creates a new pill with the specified text and type.
    /// </summary>
    /// <param name="text">The text to display inside the pill.</param>
    /// <param name="type">The pill type which determines its color scheme.</param>
    public Pill(string text, PillType type)
    {
        _text = text;
        _style = GetStyleForType(type);
    }

    private static Style GetStyleForType(PillType type) => type switch
    {
        PillType.Success => new Style(Color.White, Color.Green),
        PillType.Warning => new Style(Color.Black, Color.Yellow),
        PillType.Error => new Style(Color.White, Color.Red),
        PillType.Info => new Style(Color.White, Color.Blue),
        _ => new Style(Color.White, Color.Grey),
    };


    /*
    The Measure() method tells Spectre.Console how wide our pill needs to be. 
    Containers like Table and Panel call this before rendering to calculate layouts.

    Our pill width is the text length plus 4 characters: two for padding spaces and two for the rounded cap characters. 
    We return the same value for both minimum and maximum since our pill has a fixed width.
    */

    /// <summary>
    /// Measures the pill's width in console cells.
    /// </summary>
    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        // Width = text + 2 padding spaces + 2 cap characters
        var width = _text.Length + 4;
        return new Measurement(width, width);
    }


    /*
    The Render() method produces the actual output as Segment objects. Each segment contains text and an optional style.
    We yield three segments: the left cap, the padded text, and the right cap. 
    The yield return pattern lets Spectre.Console process segments efficiently without creating intermediate collections.

    Notice the Unicode detection: options.Capabilities.Unicode tells us whether the terminal supports Unicode characters. We use U+E0B4 and U+E0B5 for nice rounded caps, falling back to spaces on limited terminals.
    */
    
    /// <summary>
    /// Renders the pill as a sequence of styled segments.
    /// </summary>
    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        // Use rounded half-circles if Unicode is supported, otherwise spaces
        const string LeftCap = "\uE0B6";
        const string RightCap = "\uE0B4";

        var inverseStyle = new Style(_style.Background);

        if (options.Capabilities.Unicode)
        {
            yield return new Segment(LeftCap, inverseStyle);
            yield return new Segment($" {_text} ", _style);
            yield return new Segment(RightCap, inverseStyle);
        }
        else
        {
            yield return new Segment($"  {_text}  ", _style);
        }

    }
}
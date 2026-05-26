using Spectre.Console;

public static class SpectreConsoleUtilities
{
    public static void WaitForUserInput()
    {
        while (!Console.KeyAvailable)
        {
        }
        Console.ReadKey(intercept: true); // 'true' prevents the keys from being displayed
    }
}
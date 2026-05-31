namespace Pomodoro;
using Core;
public class Pomodoro : IModule
{
    public string Name => "Pomodoro";

    public string Description => "Manages pomodoro settings";

    public Repository DB { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void GetDatabase(Repository db)
    {
        DB = db;
    }

    public Task RunAsync()
    {
        throw new NotImplementedException();
    }
}

using static Cheatsheet.Spectre.SpectreConsole;
using Core;
namespace Cheatsheet;


public class Cheatsheet : IModule
{
    public string Name => "Cheatsheet";
    
    public string Description => "Registra cheatsheet que se necesitan tener a mano;";

    public Repository DB { get; set; }

    public void GetDatabase(Repository db)
    {
        DB = db;
    }

    public async Task RunAsync()
    {
        ShowSpectre();
    }




}


namespace Core;
public interface IModule
{
    string Name { get; }
    string Description { get; }
    Repository? DB {get; set;}

    public void GetDatabase(Repository db);

    Task RunAsync();
}

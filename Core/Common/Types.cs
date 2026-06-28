namespace Core.Types;
public record TaskType
{
    public int? Id {get; set;}
    public string? Name {get; set ;}
};


public record PomodoroData
{
    public int Id {get; set;}
    public string Date {get; set;}
    public string TypeId {get; set;}
    public string Time {get; set;}

    public DateTime RecordedDate => DateTime.Parse(Date);
}
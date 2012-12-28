namespace OShell.Core.Contracts
{
    public interface ICommand
    {
        string Name { get; }
        string Args { get; set; }
        string Help { get; }
    }
}
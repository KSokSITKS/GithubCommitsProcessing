namespace ConsoleApp.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        Task ExecuteAsync();
	}
}

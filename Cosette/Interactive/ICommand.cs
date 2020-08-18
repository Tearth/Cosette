namespace Cosette.Interactive
{
    public interface ICommand
    {
        string Description { get; }

        void Run();
    }
}

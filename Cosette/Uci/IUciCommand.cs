namespace Cosette.Uci
{
    public interface IUciCommand
    {
        void Run(params string[] parameters);
    }
}

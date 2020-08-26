namespace Exemplos.CustomVisionApi.Commands
{
    using McMaster.Extensions.CommandLineUtils;

    public interface ICommand
    {
        string CommandName { get; }

        void Configure(CommandLineApplication command);

        int Execute();
    }
}

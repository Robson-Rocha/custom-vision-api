namespace Exemplos.CustomVisionApi.Commands.Image
{
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;

    internal class ImageCommand : ICommand
    {
        private CommandLineApplication _command;

        public string CommandName => "image";

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Manages Custom Vision projects images.";
            command.HelpOption("-?|-h|--help");

            command.AddCommand(new ImageDeleteSubCommand())
            ;

            _command = command;
        }

        public int Execute()
        {
            _command.ShowHelp();
            return Util.Success();
        }
    }
}
namespace Exemplos.CustomVisionApi.Commands.Tag.Image
{
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;

    public class TagImageSubCommand : ICommand
    {
        private CommandLineApplication _command;

        public string CommandName => "image";

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Manages Custom Vision project tag images.";
            command.HelpOption("-?|-h|--help");

            command.AddCommand(new TagImageUploadSubCommand())
                   .AddCommand(new TagImageListSubCommand())
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
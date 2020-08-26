namespace Exemplos.CustomVisionApi.Commands.Tag
{
    using Exemplos.CustomVisionApi.Commands.Tag.Image;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

    internal class TagCommand : ICommand
    {
        private CommandLineApplication _command;

        public string CommandName => "tag";

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Manages Custom Vision projects tags.";
            command.HelpOption("-?|-h|--help");

            command.AddCommand(new TagListSubCommand())
                   .AddCommand(new TagCreateSubCommand())
                   .AddCommand(new TagDeleteSubCommand())
                   .AddCommand(new TagImageSubCommand())
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
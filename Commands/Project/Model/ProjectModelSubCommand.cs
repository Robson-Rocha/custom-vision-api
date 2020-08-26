namespace Exemplos.CustomVisionApi.Commands.Project.Model
{
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

    internal class ProjectModelSubCommand : ICommand
    {
        private CommandLineApplication _command;

        public string CommandName => "model";

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Manages project published models.";
            command.HelpOption("-?|-h|--help");

            command.AddCommand(new ProjectModelListSubCommand())
                   .AddCommand(new ProjectModelDeleteSubCommand());

            _command = command;
        }

        public int Execute()
        {
            _command.ShowHelp();
            return Util.Success();
        }
    }
}
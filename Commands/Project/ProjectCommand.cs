namespace Exemplos.CustomVisionApi.Commands.Project
{
    using Exemplos.CustomVisionApi.Commands.Project.Model;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;

    internal class ProjectCommand : ICommand
    {
        private CommandLineApplication _command;

        public string CommandName => "project";

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Manages Custom Vision projects.";
            command.HelpOption("-?|-h|--help");

            command.AddCommand(new ProjectListSubCommand())
                   .AddCommand(new ProjectCreateSubCommand())
                   .AddCommand(new ProjectDeleteSubCommand())
                   .AddCommand(new ProjectModelSubCommand())
                   .AddCommand(new ProjectTrainSubCommand())
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
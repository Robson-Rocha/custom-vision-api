namespace Exemplos.CustomVisionApi
{
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

    internal class ProjectCommand : ICommand
    {
        private readonly string _predictionResourceId;

        private CommandLineApplication _command;

        public string CommandName => "project";
        private readonly CustomVisionTrainingClient _trainingApi;

        public ProjectCommand(CustomVisionTrainingClient trainingApi, string predictionResourceId)
        {
            _trainingApi = trainingApi;
            _predictionResourceId = predictionResourceId;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Manages Custom Vision projects.";
            command.HelpOption("-?|-h|--help");

            command.AddCommand(new ProjectListSubCommand(_trainingApi))
                   .AddCommand(new ProjectCreateSubCommand(_trainingApi))
                   .AddCommand(new ProjectDeleteSubCommand(_trainingApi))
                   .AddCommand(new ProjectTrainSubCommand(_trainingApi, _predictionResourceId));

            _command = command;
        }

        public int Execute()
        {
            _command.ShowHelp();
            return 0;
        }
    }
}
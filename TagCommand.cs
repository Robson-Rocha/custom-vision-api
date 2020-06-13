namespace Exemplos.CustomVisionApi
{
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

    internal class TagCommand : ICommand
    {
        private CommandLineApplication _command;

        public string CommandName => "tag";
        private readonly CustomVisionTrainingClient _trainingApi;

        public TagCommand(CustomVisionTrainingClient trainingApi)
        {
            _trainingApi = trainingApi;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Manages Custom Vision projects tags.";
            command.HelpOption("-?|-h|--help");

            command.AddCommand(new TagListSubCommand(_trainingApi))
                   .AddCommand(new TagCreateSubCommand(_trainingApi))
                   .AddCommand(new TagDeleteSubCommand(_trainingApi))
                   .AddCommand(new TagUploadImageSubCommand(_trainingApi));

            _command = command;
        }

        public int Execute()
        {
            _command.ShowHelp();
            return 0;
        }
    }
}
namespace Exemplos.CustomVisionApi
{
    using System;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

    internal class TagDeleteSubCommand : ICommand
    {
        public string CommandName => "delete";

        private CommandOption _projectIdOption;
        private CommandOption _tagIdOption;
        private readonly CustomVisionTrainingClient _trainingApi;

        public TagDeleteSubCommand(CustomVisionTrainingClient trainingApi)
        {
            _trainingApi = trainingApi;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Deletes a tag in a project.";
            command.HelpOption("-?|-h|--help");

            _projectIdOption = command.Option("--projectId|-p", "Required. The id of the project to delete the tag.", CommandOptionType.SingleValue).IsRequired();
            _tagIdOption = command.Option("--tagId|-t", "Required. The id of the tag to be deleted.", CommandOptionType.SingleValue).IsRequired();

            _projectIdOption.Validators.Add(new ProjectIdOptionValidator(_trainingApi));
            _tagIdOption.Validators.Add(new TagIdOptionValidator(_trainingApi, _projectIdOption));
        }

        public int Execute()
        {
            Guid projectId = Guid.Parse(_projectIdOption.Value());
            Guid tagId = Guid.Parse(_tagIdOption.Value());

            _trainingApi.DeleteTag(projectId, tagId);

            Console.WriteLine($"The tag with id '{tagId}' was deleted.");

            return 0;
        }
    }
}
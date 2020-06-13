namespace Exemplos.CustomVisionApi
{
    using System;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    using Newtonsoft.Json;

    internal class ProjectCreateSubCommand : ICommand
    {
        public string CommandName => "create";

        private CommandOption _projectNameOption;
        private readonly CustomVisionTrainingClient _trainingApi;

        public ProjectCreateSubCommand(CustomVisionTrainingClient trainingApi)
        {
            _trainingApi = trainingApi;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Creates a new project.";
            command.HelpOption("-?|-h|--help");

            _projectNameOption = command.Option("--projectName|-n", "Required. The name of the project to be created.", CommandOptionType.SingleValue).IsRequired();
        }

        public int Execute()
        {
            Project project = _trainingApi.CreateProject(_projectNameOption.Value());

            Console.WriteLine(JsonConvert.SerializeObject(new {project.Name, project.Id}, Formatting.Indented));

            return 0;
        }
    }
}
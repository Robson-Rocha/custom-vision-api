namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    using Newtonsoft.Json;

    internal class TagListSubCommand : ICommand
    {
        public string CommandName => "list";
        private CommandOption _projectIdOption;
        private readonly CustomVisionTrainingClient _trainingApi;

        public TagListSubCommand(CustomVisionTrainingClient trainingApi)
        {
            _trainingApi = trainingApi;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Lists created tags.";
            command.HelpOption("-?|-h|--help");

            _projectIdOption = command.Option("--projectId|-p", "Required. The id of the project to create the tag.", CommandOptionType.SingleValue).IsRequired();

            _projectIdOption.Validators.Add(new ProjectIdOptionValidator(_trainingApi));
        }

        public int Execute()
        {
            Guid projectId = Guid.Parse(_projectIdOption.Value());

            IList<Tag> tags = _trainingApi.GetTags(projectId);

            if (tags.Any())
            {
                Console.WriteLine(JsonConvert.SerializeObject(tags.Select(t => new {t.Name, t.Id}).ToArray(), Formatting.Indented));
            }
            else
            {
                return Util.Failure("There are no tags in your project.");
            }

            return 0;
        }
    }
}
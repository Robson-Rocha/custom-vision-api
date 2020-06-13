namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    using Newtonsoft.Json;

    internal class TagCreateSubCommand : ICommand
    {
        public string CommandName => "create";

        private CommandOption _projectIdOption;
        private CommandOption _tagNameOption;
        private readonly CustomVisionTrainingClient _trainingApi;

        public TagCreateSubCommand(CustomVisionTrainingClient trainingApi)
        {
            _trainingApi = trainingApi;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Creates a new tag in a project.";
            command.HelpOption("-?|-h|--help");

            _projectIdOption = command.Option("--projectId|-p", "Required. The id of the project to create the tag.", CommandOptionType.SingleValue).IsRequired();
            _tagNameOption = command.Option("--tagName|-n", "Required. The name of the tag to be created.", CommandOptionType.SingleValue).IsRequired();

            _projectIdOption.Validators.Add(new ProjectIdOptionValidator(_trainingApi));
        }

        public int Execute()
        {
            Guid projectId = Guid.Parse(_projectIdOption.Value());

            string tagName = _tagNameOption.Value();
            IList<Tag> tagList = _trainingApi.GetTags(projectId);
            if (tagList.Any(t => t.Name == tagName))
            {
                return Util.Failure($"A tag named '{tagName}' already exists.");
            }

            Tag tag = _trainingApi.CreateTag(projectId, tagName);

            Console.WriteLine(JsonConvert.SerializeObject(new {tag.Name, tag.Id}, Formatting.Indented));

            return 0;
        }
    }
}
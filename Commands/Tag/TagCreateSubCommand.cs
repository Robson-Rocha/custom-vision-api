namespace Exemplos.CustomVisionApi.Commands.Tag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    

    internal class TagCreateSubCommand : BaseCommandWithProjectId
    {
        public override string CommandName => "create";

        private CommandOption _tagNameOption;

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Creates a new tag in a project.";

            _tagNameOption = command.Option("--tagName|-n", "Required. The name of the tag to be created.", CommandOptionType.SingleValue).IsRequired();
        }

        public override int Execute()
        {
            base.Execute();
            Guid projectId = GetProjectIdValue();
            var _trainingApi = Util.GetTrainingApi();

            string tagName = _tagNameOption.Value();
            IList<Tag> tagList = _trainingApi.GetTags(projectId);
            if (tagList.Any(t => t.Name == tagName))
            {
                return Util.Failure($"A tag named '{tagName}' already exists.");
            }

            Tag tag = _trainingApi.CreateTag(projectId, tagName);

            Util.WriteObject(new {tag.Name, tag.Id});

            return Util.Success();
        }
    }
}
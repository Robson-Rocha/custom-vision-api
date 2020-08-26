namespace Exemplos.CustomVisionApi.Commands.Tag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    

    internal class TagListSubCommand : BaseCommandWithProjectId
    {
        public override string CommandName => "list";

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Lists created tags.";
        }

        public override int Execute()
        {
            base.Execute();
            Guid projectId = GetProjectIdValue();

            IList<Tag> tags = Util.GetTrainingApi().GetTags(projectId);

            if (tags.Any())
            {
                Util.WriteObject(tags.Select(t => new {t.Name, t.Id}).ToArray());
            }
            else
            {
                return Util.Failure("There are no tags in your project.");
            }

            return Util.Success();
        }
    }
}
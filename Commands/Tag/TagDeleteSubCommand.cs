namespace Exemplos.CustomVisionApi.Commands.Tag
{
    using System;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

    public class TagDeleteSubCommand : BaseCommandWithTagId
    {
        public override string CommandName => "delete";

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Deletes a tag in a project.";
        }

        public override int Execute()
        {
            base.Execute();

            Guid projectId = GetProjectIdValue();
            Guid tagId = GetTagIdValue();

            Util.GetTrainingApi().DeleteTag(projectId, tagId);

            Console.WriteLine($"The tag with id '{tagId}' was deleted.");

            return Util.Success();
        }
    }
}
namespace Exemplos.CustomVisionApi.Commands.Project.Model
{
    using System;
    using System.Linq;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    

    public class ProjectModelListSubCommand : BaseCommandWithProjectId
    {
        public override string CommandName => "list";

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "List the project published models.";
        }

        public override int Execute()
        {
            base.Execute();
            Guid projectId = GetProjectIdValue();

            var publishedIterations = Util.GetTrainingApi().GetIterations(projectId);

            Util.WriteObject(publishedIterations.Select(i => new {i.Name, i.PublishName, i.Id}).ToArray());

            return Util.Success();
        }
    }
}

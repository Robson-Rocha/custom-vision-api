namespace Exemplos.CustomVisionApi.Commands.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

    internal class ProjectDeleteSubCommand : BaseCommandWithProjectId
    {
        public override string CommandName => "delete";

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Deletes a project.";
        }

        public override int Execute()
        {
            base.Execute();
            Guid projectId = GetProjectIdValue();
            var _trainingApi = Util.GetTrainingApi();

            IList<Iteration> iterations = _trainingApi.GetIterations(projectId);
            foreach (Iteration iteration in iterations.Where(i => i.PublishName != null))
            {
                Console.WriteLine($"Unpublishing iteration {iteration.PublishName}...");
                _trainingApi.UnpublishIteration(projectId, iteration.Id);
            }

            _trainingApi.DeleteProject(projectId);

            Console.WriteLine($"The project with id '{projectId}' was deleted");

            return Util.Success();
        }
    }
}
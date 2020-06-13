namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

    internal class ProjectDeleteSubCommand : ICommand
    {
        public string CommandName => "delete";

        private CommandOption _projectIdOption;
        private readonly CustomVisionTrainingClient _trainingApi;

        public ProjectDeleteSubCommand(CustomVisionTrainingClient trainingApi)
        {
            _trainingApi = trainingApi;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Deletes a project.";
            command.HelpOption("-?|-h|--help");

            _projectIdOption = command.Option("--projectId|-p", "Required. The id of the project to be removed.", CommandOptionType.SingleValue)
                                      .IsRequired();

            _projectIdOption.Validators.Add(new ProjectIdOptionValidator(_trainingApi));
        }

        public int Execute()
        {
            Guid projectId = Guid.Parse(_projectIdOption.Value());

            IList<Iteration> iterations = _trainingApi.GetIterations(projectId);
            foreach (Iteration iteration in iterations.Where(i => i.PublishName != null))
            {
                Console.WriteLine($"Unpublishing iteration {iteration.PublishName}...");
                _trainingApi.UnpublishIteration(projectId, iteration.Id);
            }

            _trainingApi.DeleteProject(projectId);

            Console.WriteLine($"The project with id '{projectId}' was deleted");

            return 0;
        }
    }
}
namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    using Newtonsoft.Json;

    internal class ProjectListSubCommand : ICommand
    {
        public string CommandName => "list";
        private readonly CustomVisionTrainingClient _trainingApi;

        public ProjectListSubCommand(CustomVisionTrainingClient trainingApi)
        {
            _trainingApi = trainingApi;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Lists created projects.";
            command.HelpOption("-?|-h|--help");
        }

        public int Execute()
        {
            IList<Project> projects = _trainingApi.GetProjects();

            if (projects.Any())
            {
                Console.WriteLine(JsonConvert.SerializeObject(projects.Select(p => new {p.Name, p.Id}).ToArray(), Formatting.Indented));
            }
            else
            {
                return Util.Failure("There are no projects in your Custom Vision.");
            }

            return 0;
        }
    }
}
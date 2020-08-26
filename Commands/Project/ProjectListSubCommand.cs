namespace Exemplos.CustomVisionApi.Commands.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    

    internal class ProjectListSubCommand : BaseCommand
    {
        public override string CommandName => "list";

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Lists created projects.";
        }

        public override int Execute()
        {
            base.Execute();
            IList<Project> projects = Util.GetTrainingApi().GetProjects();

            if (projects.Any())
            {
                Util.WriteObject(projects.Select(p => new {p.Name, p.Id}).ToArray());
            }
            else
            {
                return Util.Failure("There are no projects in your Custom Vision.");
            }

            return Util.Success();
        }
    }
}
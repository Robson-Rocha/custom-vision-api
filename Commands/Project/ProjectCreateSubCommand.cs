namespace Exemplos.CustomVisionApi.Commands.Project
{
    using System;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    

    internal class ProjectCreateSubCommand : BaseCommand
    {
        public override string CommandName => "create";

        private CommandOption _projectNameOption;

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Creates a new project.";

            _projectNameOption = command.Option("--projectName|-n", "Required. The name of the project to be created.", CommandOptionType.SingleValue).IsRequired();
        }

        public override int Execute()
        {
            base.Execute();
            Project project = Util.GetTrainingApi().CreateProject(_projectNameOption.Value());

            Util.WriteObject(new {project.Name, project.Id});

            return Util.Success();
        }
    }
}
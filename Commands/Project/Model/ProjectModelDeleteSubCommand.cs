namespace Exemplos.CustomVisionApi.Commands.Project.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    

    internal class ProjectModelDeleteSubCommand : BaseCommandWithProjectId
    {
        public override string CommandName => "delete";

        private CommandOption _modelNameOption;
        private CommandOption _iterationIdOption;

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Deletes a project published model or unpublished iteration.";

            _modelNameOption = command.Option("--modelName|-m", "Optional. The name of the published model to be deleted.", CommandOptionType.SingleValue);
            _iterationIdOption = command.Option("--iterationId|-i", "Optional. The id of the unpublished iteration to be deleted.", CommandOptionType.SingleValue);
        }

        public override int Execute()
        {
            base.Execute();
            Guid projectId = GetProjectIdValue();
            string modelName = _modelNameOption.Value();
            Guid? iterationId = _iterationIdOption.Value() != null ? Guid.Parse(_iterationIdOption.Value()) : default(Guid?);

            if (string.IsNullOrWhiteSpace(modelName) && !iterationId.HasValue)
            {
                return Util.Failure("Neither the --modelName nor --iterationId were specified.");
            }
            if (!string.IsNullOrWhiteSpace(modelName) && iterationId.HasValue)
            {
                return Util.Failure("You cannot specify only --modelName or --iterationId, not both");
            }

            try
            {
                var _trainingApi = Util.GetTrainingApi();
                IList<Iteration> iterations = _trainingApi.GetIterations(projectId);

                Iteration iteration = null;
                
                if(!string.IsNullOrWhiteSpace(modelName))
                {
                    iteration = iterations.FirstOrDefault(i => i.PublishName == modelName);
                } 
                else if(iterationId != null) 
                {
                    iteration = iterations.FirstOrDefault(i => i.Id == iterationId.Value);
                }

                if (iteration == null)
                {
                    return Util.Failure("Not found.");
                }

                if(!string.IsNullOrWhiteSpace(iteration.PublishName))
                {
                    Console.Write($"Unpublishing model {iteration.PublishName}... ");
                    _trainingApi.UnpublishIteration(projectId, iteration.Id);
                    Console.WriteLine($"done!");
                }
                Console.Write($"Deleting iteration {iteration.Id}... ");
                _trainingApi.DeleteIteration(projectId, iteration.Id);
                Console.WriteLine($"done!");
            }
            catch (CustomVisionErrorException customVisionError)
            {
                return Util.FailureObject(customVisionError.Body);
            }


            return Util.Success();
        }
    }
}

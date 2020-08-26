namespace Exemplos.CustomVisionApi.Commands.Project
{
    using System;
    using System.Linq;
    using System.Threading;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
    

    internal class ProjectTrainSubCommand : BaseCommandWithProjectId
    {
        public override string CommandName => "train";

        private CommandOption _modelNameOption;

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Train the classifier using previously uploaded images.";

            _modelNameOption = command.Option("--modelName|-m", "Required. The name of the published model to be trained for use in predictions.", CommandOptionType.SingleValue).IsRequired();
        }

        public override int Execute()
        {
            base.Execute();
            Guid projectId = GetProjectIdValue();
            string modelName = _modelNameOption.Value();
            var _trainingApi = Util.GetTrainingApi();
            string _predictionResourceId = Util.Configuration["customVision:prediction:resourceId"];
            if (string.IsNullOrWhiteSpace(_predictionResourceId))
            {
                return Util.Failure("No Custom Vision Prediction Resource Id set in the config file");
            }

            Console.Write("Training");
            Iteration iteration;
            try
            {
                iteration = _trainingApi.TrainProject(projectId);
            }
            catch(CustomVisionErrorException customVisionError) when (customVisionError.Body.Code == CustomVisionErrorCodes.BadRequestTrainingNotNeeded)
            {
                Console.Write(" ...training not needed;");
                iteration = _trainingApi.GetIterations(projectId).FirstOrDefault(i => i.PublishName == modelName);
            }

            if (iteration == null)
            {
                return Util.Failure($"\r\nNo published iterations with name {modelName} were found. Check if there are unpublished iterations.");
            }

            while (iteration.Status == "Training")
            {
                Thread.Sleep(1000);
                Console.Write(".");
                iteration = _trainingApi.GetIteration(projectId, iteration.Id);
            }
            Console.WriteLine(" done!");

            Console.Write($"Publishing iteration '{iteration.Id}' as '{modelName}'...");
            bool published = false;
            while (!published)
            {
                try
                {
                    _trainingApi.PublishIteration(projectId, iteration.Id, modelName, _predictionResourceId);
                    published = true;
                }
                catch(CustomVisionErrorException customVisionError_PublishIteration) when (customVisionError_PublishIteration.Body.Code == CustomVisionErrorCodes.BadRequestInvalidPublishName ||
                                                                                           customVisionError_PublishIteration.Body.Code == CustomVisionErrorCodes.BadRequestIterationIsPublished)
                {
                    try
                    {
                        _trainingApi.UnpublishIteration(projectId, iteration.Id);
                    }
                    catch(CustomVisionErrorException customVisionError_UnpublishIteration)
                    {
                        return Util.FailureObject(customVisionError_UnpublishIteration.Body);
                    }
                }
                catch
                {
                    throw;
                }
            }

            Console.WriteLine(" done!");

            return Util.Success();
        }
    }
}

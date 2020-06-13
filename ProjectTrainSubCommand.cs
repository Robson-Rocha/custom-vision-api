namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Linq;
    using System.Threading;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

    internal class ProjectTrainSubCommand : ICommand
    {
        private readonly string _predictionResourceId;

        public string CommandName => "train";

        private CommandOption _projectIdOption;
        private CommandOption _modelNameOption;
        private readonly CustomVisionTrainingClient _trainingApi;

        public ProjectTrainSubCommand(CustomVisionTrainingClient trainingApi, string predictionResourceId)
        {
            this._trainingApi = trainingApi;
            _predictionResourceId = predictionResourceId;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Train the classifier using previously uploaded images.";
            command.HelpOption("-?|-h|--help");

            _projectIdOption = command.Option("--projectId|-p", "Required. The id of the project to be trained.", CommandOptionType.SingleValue).IsRequired();
            _modelNameOption = command.Option("--modelName|-m", "Required. The name of the published model to be trained for use in predictions.", CommandOptionType.SingleValue).IsRequired();

            _projectIdOption.Validators.Add(new ProjectIdOptionValidator(_trainingApi));
        }

        public int Execute()
        {
            Guid projectId = Guid.Parse(_projectIdOption.Value());
            string modelName = _modelNameOption.Value();

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
                catch(CustomVisionErrorException customVisionError) when (customVisionError.Body.Code == CustomVisionErrorCodes.BadRequestInvalidPublishName ||
                                                                          customVisionError.Body.Code == CustomVisionErrorCodes.BadRequestIterationIsPublished)
                {
                    _trainingApi.UnpublishIteration(projectId, iteration.Id);
                }
                catch
                {
                    throw;
                }
            }

            Console.WriteLine(" done!");

            return 0;
        }
    }
}

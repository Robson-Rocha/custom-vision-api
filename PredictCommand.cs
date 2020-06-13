namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

    internal class PredictCommand : ICommand
    {
        public string CommandName => "predict";

        private CommandOption _projectIdOption;
        private CommandOption _modelNameOption;
        private CommandOption _pathOption;
        private readonly CustomVisionTrainingClient _trainingApi;
        private readonly CustomVisionPredictionClient _predictionApi;

        public PredictCommand(CustomVisionTrainingClient trainingApi, CustomVisionPredictionClient predictionApi)
        {
            _trainingApi = trainingApi;
            _predictionApi = predictionApi;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Tries to predict the classification (tag) of the given image.";
            command.HelpOption("-?|-h|--help");

            _projectIdOption = command.Option("--projectId|-p", "Required. The id of the project to be used for predictions.", CommandOptionType.SingleValue).IsRequired();
            _modelNameOption = command.Option("--modelName|-m", "Required. The name of the published trained model used for predictions.", CommandOptionType.SingleValue).IsRequired();
            _pathOption = command.Option("--imagePath|-i", "Required. The path of one or more images whose classification (tag) must be predicted.", CommandOptionType.MultipleValue).IsRequired();

            _projectIdOption.Validators.Add(new ProjectIdOptionValidator(_trainingApi));
        }

        public int Execute()
        {
            string modelName = _modelNameOption.Value();
            Guid projectId = Guid.Parse(_projectIdOption.Value());
            List<string> imagePaths = _pathOption.Values;

            foreach (string imagePath in imagePaths)
                if (!File.Exists(imagePath))
                    return Util.Failure($"The path '{imagePath}' does not exist.");

            foreach (string imagePath in imagePaths)
            {
                Console.WriteLine($"\r\n{imagePath}:");
                ImagePrediction result;
                using (var imageStream = new MemoryStream(File.ReadAllBytes(imagePath)))
                    result = _predictionApi.ClassifyImage(projectId, modelName, imageStream);

                foreach (PredictionModel predictionModel in result.Predictions)
                {
                    Console.WriteLine($"\t{predictionModel.TagName}: {predictionModel.Probability:P2}");
                }
            }

            return 0;
        }
    }
}
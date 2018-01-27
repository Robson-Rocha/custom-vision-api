namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Cognitive.CustomVision.Prediction;
    using Microsoft.Cognitive.CustomVision.Prediction.Models;
    using Microsoft.Cognitive.CustomVision.Training;
    using Microsoft.Cognitive.CustomVision.Training.Models;
    using Microsoft.Extensions.CommandLineUtils;

    internal class PredictCommand : ICommand
    {
        private readonly Guid _projectId;
        private readonly string _trainingKey;
        private readonly string _predictionKey;

        private CommandArgument _pathArgument;

        public string CommandName => "predict";

        public PredictCommand(Guid projectId, string trainingKey, string predictionKey)
        {
            _projectId = projectId;
            _trainingKey = trainingKey;
            _predictionKey = predictionKey;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Tries to predict the classification (tag) of the given image.";
            command.HelpOption("-?|-h|--help");

            _pathArgument = command.Argument("path", "Required. The path of one or more images whose classification (tag) must be predicted.", multipleValues: true);
        }

        public int Execute()
        {
            List<string> imagePaths = _pathArgument.Values;

            foreach (string imagePath in imagePaths)
                if (!File.Exists(imagePath))
                    return Util.Fail($"The path '{imagePath}' does not exist.");

            TrainingApi trainingApi = new TrainingApi() { ApiKey = _trainingKey };
            IList<Iteration> iterations = trainingApi.GetIterations(_projectId);

            if (iterations.Count == 0)
                return Util.Fail("You must train the classifier before trying to make any prediction.\r\nUse the [upload] command to upload at least 5 images to a given classification (tag), and then use the command [train] to train the classifier.");

            Guid iterationId = iterations.OrderByDescending(i => i.IsDefault).ThenByDescending(i => i.TrainedAt).First().Id;

            PredictionEndpoint predictionEndpoint = new PredictionEndpoint() { ApiKey = _predictionKey };

            foreach (string imagePath in imagePaths)
            {
                Console.WriteLine($"\r\n{imagePath}:");
                ImagePredictionResultModel result;
                using (var imageStream = new MemoryStream(File.ReadAllBytes(imagePath)))
                    result = predictionEndpoint.PredictImage(_projectId, imageStream, iterationId);

                foreach (ImageTagPredictionModel predictedTag in result.Predictions)
                    Console.WriteLine($"\t{predictedTag.Tag}: {predictedTag.Probability:P2}");
            }

            return 0;
        }
    }
}
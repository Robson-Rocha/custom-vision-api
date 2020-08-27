namespace Exemplos.CustomVisionApi.Commands.Predict
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

    public class PredictCommand : BaseCommandWithProjectId
    {
        public override string CommandName => "predict";

        private CommandOption _modelNameOption;
        private CommandOption _pathOption;

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Tries to predict the classification (tag) of the given image.";

            _modelNameOption = command.Option("--modelName|-m", "Required. The name of the published trained model used for predictions.", CommandOptionType.SingleValue).IsRequired();
            _pathOption = command.Option("--imagePath|-i", "Required. The path of one or more images whose classification (tag) must be predicted.", CommandOptionType.MultipleValue).IsRequired();
        }

        public override int Execute()
        {
            base.Execute();
            string modelName = _modelNameOption.Value();
            Guid projectId = GetProjectIdValue();
            List<string> imagePaths = _pathOption.Values;

            foreach (string imagePath in imagePaths)
                if (!File.Exists(imagePath))
                    return Util.Failure($"The path '{imagePath}' does not exist.");

            foreach (string imagePath in imagePaths)
            {
                Console.WriteLine($"\r\n{imagePath}:");
                ImagePrediction result;
                using (var imageStream = new MemoryStream(File.ReadAllBytes(imagePath)))
                    result = Util.GetPredictionApi().ClassifyImage(projectId, modelName, imageStream);

                foreach (PredictionModel predictionModel in result.Predictions)
                {
                    Console.WriteLine($"\t{predictionModel.TagName}: {predictionModel.Probability:P2}");
                }
            }

            return Util.Success();
        }
    }
}
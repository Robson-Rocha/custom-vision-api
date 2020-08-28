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
        private CommandOption _urlOption;

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Tries to predict the classification (tag) of the given image.";

            _modelNameOption = command.Option("--modelName|-m", "Required. The name of the published trained model used for predictions.", CommandOptionType.SingleValue).IsRequired();
            _pathOption = command.Option("--imagePath|-i", "At least one Required (or one imageUrl). The path of one image which classification (tag) must be predicted. Can be specified multiple times.", CommandOptionType.MultipleValue);
            _urlOption = command.Option("--imageUrl|-u", "Ate least one Required (or one imagePath). The url of one image which classification (tag) must be predicted. Can be specified multiple times.", CommandOptionType.MultipleValue);
        }

        public class PredictionResult
        {
            public PredictionResult()
            {
                PredictedTags = new List<Tag>();
            }

            public class Tag
            {
                public string TagName { get; set; }
                public double Probability { get; set; }
            }

            public string ImagePath { get; set; }
            public List<Tag> PredictedTags { get; set; }
        }

        public override int Execute()
        {
            base.Execute();
            string modelName = _modelNameOption.Value();
            Guid projectId = GetProjectIdValue();

            List<string> imagePaths = _pathOption.Values;
            List<string> imageUrls = _urlOption.Values;

            if (imagePaths.Count == 0 && imageUrls.Count == 0)
            {
                return Util.Failure($"At least one imagePath or imageUrl must be specified.");
            }

            foreach (string imagePath in imagePaths)
            {
                if (!File.Exists(imagePath))
                {
                    return Util.Failure($"The path '{imagePath}' does not exist.");
                }
            }

            foreach (string imageUrl in imageUrls)
            {
                if (!Util.TestUrl(imageUrl, out var statusCode))
                {
                    return Util.Failure($"The url '{imageUrl}' cannot be reached ({statusCode}).");
                }
            }

            var predictionApi = Util.GetPredictionApi();
            var results = new List<PredictionResult>();

            void ReadPrediction(PredictionResult result, ImagePrediction imagePrediction)
            {
                foreach (PredictionModel predictionModel in imagePrediction.Predictions)
                {
                    result.PredictedTags.Add(new PredictionResult.Tag 
                    { 
                        TagName = predictionModel.TagName, 
                        Probability = Math.Round(predictionModel.Probability * 100, 2)
                    });
                }
            }

            foreach (string imagePath in imagePaths)
            {
                var result = new PredictionResult();
                result.ImagePath = imagePath;

                ImagePrediction imagePrediction;
                using (var imageStream = new MemoryStream(File.ReadAllBytes(imagePath)))
                    imagePrediction = predictionApi.ClassifyImage(projectId, modelName, imageStream);

                ReadPrediction(result, imagePrediction);

                results.Add(result);
            }

            foreach (string imageUrl in imageUrls)
            {
                var result = new PredictionResult {
                    ImagePath = imageUrl
                };

                ImagePrediction imagePrediction;
                imagePrediction = predictionApi.ClassifyImageUrl(projectId, modelName, new ImageUrl(imageUrl));

                ReadPrediction(result, imagePrediction);

                results.Add(result);
            }

            Util.WriteObject(results);

            return Util.Success();
        }
    }
}
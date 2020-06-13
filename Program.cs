namespace Exemplos.CustomVisionApi
{
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "custom-vision";
            app.FullName = "Azure Cognitive Services Custom Vision API + Computer Vision API Demostration";
            app.VersionOption("-v|--version", "v2.0");
            app.HelpOption("-?|-h|--help");

            string customVision_TrainingEndpoint = Util.Configuration["customVision:training:endpoint"];
            string customVision_TrainingKey = Util.Configuration["customVision:training:key"];
            string customVision_PredictionKey = Util.Configuration["customVision:prediction:key"];
            string customVision_PredictionEndpoint = Util.Configuration["customVision:prediction:endpoint"];
            string customVision_PredictionResourceId = Util.Configuration["customVision:prediction:resourceId"];

            string computerVision_Key = Util.Configuration["computerVision:key"];
            string computerVision_Endpoint = Util.Configuration["computerVision:endpoint"];

            CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient()
            {
                ApiKey = customVision_TrainingKey,
                Endpoint = customVision_TrainingEndpoint
            };

            CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient()
            {
                ApiKey = customVision_PredictionKey,
                Endpoint = customVision_PredictionEndpoint
            };

            app.AddCommand(new ProjectCommand(trainingApi, customVision_PredictionResourceId))
               .AddCommand(new TagCommand(trainingApi))
               .AddCommand(new PredictCommand(trainingApi, predictionApi))
               .AddCommand(new OCRCommand(computerVision_Key, computerVision_Endpoint));

            try
            {
                if (args.Length == 0)
                    app.ShowHelp();
                else
                    app.Execute(args);
            }
            catch (CommandParsingException cpe)
            {
                Console.WriteLine(cpe.Message);
                app.ShowHelp();
            }
        }
    }
}

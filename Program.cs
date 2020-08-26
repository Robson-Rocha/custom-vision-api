namespace Exemplos.CustomVisionApi
{
    using Exemplos.CustomVisionApi.Commands.CreateConfig;
    using Exemplos.CustomVisionApi.Commands.Image;
    using Exemplos.CustomVisionApi.Commands.OCR;
    using Exemplos.CustomVisionApi.Commands.Predict;
    using Exemplos.CustomVisionApi.Commands.Project;
    using Exemplos.CustomVisionApi.Commands.Tag;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using System;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "custom-vision";
            app.FullName = "Azure Cognitive Services Custom Vision API Helper CLI";
            app.VersionOption("-v|--version", "v2.0");
            app.HelpOption("-?|-h|--help");

            app.AddCommand(new CreateConfigCommand())
               .AddCommand(new ProjectCommand())
               .AddCommand(new TagCommand())
               .AddCommand(new ImageCommand())
               .AddCommand(new PredictCommand())
            //    .AddCommand(new OCRCommand(computerVision_Key, computerVision_Endpoint))
            ;

            try
            {
                if (args.Length == 0)
                {
                    app.ShowHelp();
                }
                else
                {
                    app.Execute(args);
                }
            }
            catch (CommandParsingException cpe)
            {
                Console.WriteLine(cpe.Message);
                app.ShowHelp();
            }
        }
    }
}

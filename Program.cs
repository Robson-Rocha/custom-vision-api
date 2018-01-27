namespace Exemplos.CustomVisionApi
{
    using Microsoft.Extensions.CommandLineUtils;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "custom-vision";
            app.FullName = "Azure Cognitive Services Custom Vision API Demostration";
            app.VersionOption("-v|--version", "v1.0");
            app.HelpOption("-?|-h|--help");

            Guid projectId = new Guid(Util.Configuration["projectId"]);
            string trainingKey = Util.Configuration["trainingKey"];
            string predictionKey = Util.Configuration["predictionKey"];

            app.AddCommand(new UploadCommand(projectId, trainingKey))
               .AddCommand(new TrainCommand(projectId, trainingKey))
               .AddCommand(new PredictCommand(projectId, trainingKey, predictionKey));

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

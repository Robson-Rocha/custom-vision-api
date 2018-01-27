namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Threading;
    using Microsoft.Cognitive.CustomVision.Training;
    using Microsoft.Cognitive.CustomVision.Training.Models;
    using Microsoft.Extensions.CommandLineUtils;

    internal class TrainCommand : ICommand
    {
        private readonly Guid _projectId;
        private readonly string _trainingKey;

        public string CommandName => "train";

        public TrainCommand(Guid projectId, string trainingKey)
        {
            _projectId = projectId;
            _trainingKey = trainingKey;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Train the classifier using previously uploaded images.";
            command.HelpOption("-?|-h|--help");
        }

        public int Execute()
        {
            TrainingApi trainingApi = new TrainingApi() { ApiKey = _trainingKey };

            Console.Write("\tTraining");
            Iteration iteration = trainingApi.TrainProject(_projectId);

            while (iteration.Status == "Training")
            {
                Thread.Sleep(1000);
                Console.Write(".");
                iteration = trainingApi.GetIteration(_projectId, iteration.Id);
            }

            iteration.IsDefault = true;
            trainingApi.UpdateIteration(_projectId, iteration.Id, iteration);
            Console.WriteLine("done!");

            return 0;
        }
    }
}

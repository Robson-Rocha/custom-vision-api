namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

    internal class TagUploadImageSubCommand : ICommand
    {
        private CommandOption _projectIdOption;
        private CommandOption _tagIdOption;
        private CommandOption _imagesPathOption;

        public string CommandName => "upload-image";
        private readonly CustomVisionTrainingClient _trainingApi;

        public TagUploadImageSubCommand(CustomVisionTrainingClient trainingApi)
        {
            _trainingApi = trainingApi;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Upload images to train the classifier for a specific document type (tag).";
            command.HelpOption("-?|-h|--help");

            _projectIdOption = command.Option("--projectId|-p", "Required. The id of the project to be trained.", CommandOptionType.SingleValue).IsRequired();
            _tagIdOption = command.Option("--tagId|-t", "Required. The id of the tag to be trained.", CommandOptionType.SingleValue).IsRequired();
            _imagesPathOption = command.Option("--imagesPath|-i", "Required. The path where the training images are located.", CommandOptionType.SingleValue).IsRequired();

            _projectIdOption.Validators.Add(new ProjectIdOptionValidator(_trainingApi));
            _tagIdOption.Validators.Add(new TagIdOptionValidator(_trainingApi, _projectIdOption));
        }

        public int Execute()
        {
            Guid projectId = Guid.Parse(_projectIdOption.Value());
            Guid tagId = Guid.Parse(_tagIdOption.Value());
            string imagesPath = _imagesPathOption.Value();

            if (!Directory.Exists(imagesPath))
                return Util.Failure($"The path '{imagesPath}' does not exist");

            string[] trainingImages = Directory.GetFiles(imagesPath);

            List<Guid> tagIds = new List<Guid> { tagId };

            foreach (var imagePath in trainingImages)
            {
                Console.Write($"Uploading '{imagePath}'... ");
                using (var imageStream = new MemoryStream(File.ReadAllBytes(imagePath)))
                    _trainingApi.CreateImagesFromData(projectId, imageStream, tagIds);
                Console.WriteLine($"done");
            }

            Console.WriteLine($"\r\nDone uploading {trainingImages.Length} images.");

            return 0;
        }
    }
}

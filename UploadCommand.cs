namespace Exemplos.CustomVisionApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Cognitive.CustomVision.Training;
    using Microsoft.Cognitive.CustomVision.Training.Models;
    using Microsoft.Extensions.CommandLineUtils;

    internal class UploadCommand : ICommand
    {
        private readonly Guid _projectId;
        private readonly string _trainingKey;

        private CommandArgument _tagArgument;
        private CommandArgument _pathArgument;

        public string CommandName => "upload";

        public UploadCommand(Guid projectId, string trainingKey)
        {
            _projectId = projectId;
            _trainingKey = trainingKey;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Upload images to train the classifier for a specific document type (tag).";
            command.HelpOption("-?|-h|--help");

            _tagArgument = command.Argument("tag", "Required. The name of the tag to be trained.");
            _pathArgument = command.Argument("path", "Required. The path where the training images are located.");
        }

        public int Execute()
        {
            string tagName = _tagArgument.Value;
            string imagesPath = _pathArgument.Value;

            if (!Directory.Exists(imagesPath))
                return Util.Fail($"The path '{imagesPath}' does not exist");

            string[] trainingImages = Directory.GetFiles(imagesPath);

            TrainingApi trainingApi = new TrainingApi() { ApiKey = _trainingKey };

            TagList tagList = trainingApi.GetTags(_projectId);
            Tag tag = tagList.Tags.FirstOrDefault(t => t.Name == tagName);

            if (tag == null)
                return Util.Fail($"No tag named {tagName} was found.");

            List<string> tagIds = new List<string> { tag.Id.ToString() };

            foreach (var imagePath in trainingImages)
            {
                Console.Write($"Uploading '{imagePath}'... ");
                using (var imageStream = new MemoryStream(File.ReadAllBytes(imagePath)))
                    trainingApi.CreateImagesFromData(_projectId, imageStream, tagIds);
                Console.WriteLine($"done");
            }

            Console.WriteLine($"\r\nDone uploading {trainingImages.Length} images.");

            return 0;
        }
    }
}

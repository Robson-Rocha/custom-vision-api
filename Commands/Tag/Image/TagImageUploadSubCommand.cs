namespace Exemplos.CustomVisionApi.Commands.Tag.Image
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

    internal class TagImageUploadSubCommand : BaseCommandWithTagId
    {
        private CommandOption _imagesPathOption;

        public override string CommandName => "upload-images";

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Upload images to train the classifier for a specific document type (tag).";

            _imagesPathOption = command.Option("--imagesPath|-i", "Required. The path where the images to be uploaded are located.", CommandOptionType.SingleValue).IsRequired();
        }

        public override int Execute()
        {
            base.Execute();
            Guid projectId = GetProjectIdValue();
            Guid tagId = GetTagIdValue();
            string imagesPath = _imagesPathOption.Value();

            if (!Directory.Exists(imagesPath))
                return Util.Failure($"The path '{imagesPath}' does not exist");

            string[] trainingImages = Directory.GetFiles(imagesPath);

            List<Guid> tagIds = new List<Guid> { tagId };

            foreach (var imagePath in trainingImages)
            {
                Console.Write($"Uploading '{imagePath}'... ");
                using (var imageStream = new MemoryStream(File.ReadAllBytes(imagePath)))
                    Util.GetTrainingApi().CreateImagesFromData(projectId, imageStream, tagIds);
                Console.WriteLine($"done");
            }

            Console.WriteLine($"\r\nDone uploading {trainingImages.Length} images.");

            return Util.Success();
        }
    }
}

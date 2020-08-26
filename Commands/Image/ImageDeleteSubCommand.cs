using System;
using System.Collections.Generic;
using System.Linq;
using Exemplos.CustomVisionApi.Extensions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

namespace Exemplos.CustomVisionApi.Commands.Image
{
    internal class ImageDeleteSubCommand : BaseCommandWithProjectId
    {
        private CommandOption _imageIdOption;

        public override string CommandName => "delete";

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Deletes a Custom Vision project image.";

            _imageIdOption = command.Option("--imageId|-i", "Required. The id of the image to be deleted.", CommandOptionType.SingleValue).IsRequired();
        }

        public override int Execute()
        {
            base.Execute();
            var projectId = GetProjectIdValue();
            Guid imageId;
            if (!Guid.TryParse(_imageIdOption.Value(), out imageId))
            {
                return Util.Failure($"The image id {_imageIdOption.Value()} is not a valid Guid.");
            }

            var trainingApi = Util.GetTrainingApi();
            var imageIds = new List<Guid> { imageId };

            if(!trainingApi.GetImagesByIds(projectId, imageIds).Any())
            {
                return Util.Failure($"No image with id '{imageId}' was found");
            }

            trainingApi.DeleteImages(projectId, imageIds);

            return Util.Success($"Image id {imageId} deleted");
        }
    }
}
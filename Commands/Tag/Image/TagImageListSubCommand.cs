namespace Exemplos.CustomVisionApi.Commands.Tag.Image
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Exemplos.CustomVisionApi.Commands;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

    public class TagImageListSubCommand : BaseCommandWithTagId
    {
        private CommandOption _modelNameOption;
        private CommandOption _skipOption;
        private CommandOption _takeOption;
        private CommandOption _countOption;

        public override string CommandName => "list";

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            command.Description = "Lists or counts Custom Vision project tagged images.";

            _modelNameOption = command.Option("--modelName|-m", "Required. The name of the published model whose tags images shall be listed or counted.", CommandOptionType.SingleValue).IsRequired();

            _skipOption = command.Option("--skip", "Optional. Number of images to skip. Default is 0.", CommandOptionType.SingleValue);
            _takeOption = command.Option("--take", "Optional. Number of images to take. Default is 50.", CommandOptionType.SingleValue);

            _countOption = command.Option("--count", "Optional. If supplied, returns the number of images of the given tag at the model.", CommandOptionType.NoValue);
        }

        public override int Execute()
        {
            base.Execute();
            var trainingApi = Util.GetTrainingApi();
            var projectId = GetProjectIdValue();
            var modelName = _modelNameOption.Value();
            var tagId = GetTagIdValue();

            var iteration = trainingApi.GetIterations(projectId)
                                       .FirstOrDefault(i => i.PublishName == modelName);

            var tagIds = new List<Guid> { tagId };

            if (_countOption.HasValue())
            {
                Util.WriteObject(new {count = trainingApi.GetTaggedImageCount(projectId, iteration.Id, tagIds)});
            }
            else
            {
                int skip = 0;
                int take = 50;
                if (_skipOption.HasValue()) 
                {
                    int.TryParse(_skipOption.Value(), out skip);
                } 
                if (_takeOption.HasValue()) 
                {
                    int.TryParse(_takeOption.Value(), out take);
                } 
                var images = trainingApi.GetTaggedImagesWithHttpMessagesAsync(projectId, iteration.Id, tagIds, skip: skip, take: take).Result.Body;
                Util.WriteObject(images);
            }

            return Util.Success();
        }
    }
}
namespace Exemplos.CustomVisionApi
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Exemplos.CustomVisionApi.Extensions;
    using Exemplos.CustomVisionApi.OptionValidators;
    using McMaster.Extensions.CommandLineUtils;
    using McMaster.Extensions.CommandLineUtils.Validation;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

    public class TagIdOptionValidator : IOptionValidator, IDeferredValidation
    {
        private readonly Func<CustomVisionTrainingClient> _trainingApiInitializer;
        private CustomVisionTrainingClient TrainingApi {
            get
            {
                return _trainingApiInitializer();
            }
        }

        private readonly CommandOption _projectIdOption;

        public TagIdOptionValidator(Func<CustomVisionTrainingClient> trainingApiInitializer, CommandOption projectIdOption)
        {
            this._trainingApiInitializer = trainingApiInitializer;
            _projectIdOption = projectIdOption;
        }

        public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
        {
            if (!Guid.TryParse(option.Value(), out Guid tagId))
            {
                return new ValidationResult($"The tag id '{option.Value()}' is not a valid GUID.");
            }

            return ValidationResult.Success;
        }

        public void ValidateDeferred(CommandOption option)
        {
            var projectId = Guid.Parse(_projectIdOption.Value());
            var tagId = Guid.Parse(option.Value());

            if (!TrainingApi.TryGetTag(projectId, tagId, out Tag tag))
            {
                Util.Fail($"Could not find any tag with id '{tagId}'.");
            }
        }
    }
}
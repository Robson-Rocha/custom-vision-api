namespace Exemplos.CustomVisionApi
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using McMaster.Extensions.CommandLineUtils;
    using McMaster.Extensions.CommandLineUtils.Validation;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

    public class TagIdOptionValidator : IOptionValidator
    {
        private readonly CustomVisionTrainingClient _trainingApi;
        private readonly CommandOption _projectIdOption;

        public TagIdOptionValidator(CustomVisionTrainingClient trainingApi, CommandOption projectIdOption)
        {
            _trainingApi = trainingApi;
            _projectIdOption = projectIdOption;
        }

        public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
        {
            if (!Guid.TryParse(option.Value(), out Guid tagId))
            {
                return new ValidationResult($"The project id '{option.Value()}' is not a valid GUID.");
            }

            var projectId = Guid.Parse(_projectIdOption.Value());

            if (!_trainingApi.TryGetTag(projectId, tagId, out Tag tag))
            {
                return new ValidationResult($"Could not find any project with id '{tagId}'.");
            }

            return ValidationResult.Success;
        }
    }
}
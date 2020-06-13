namespace Exemplos.CustomVisionApi
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using McMaster.Extensions.CommandLineUtils;
    using McMaster.Extensions.CommandLineUtils.Validation;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

    public class ProjectIdOptionValidator : IOptionValidator
    {
        private readonly CustomVisionTrainingClient _trainingApi;

        public ProjectIdOptionValidator(CustomVisionTrainingClient trainingApi)
        {
            _trainingApi = trainingApi;
        }

        public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
        {
            if (!Guid.TryParse(option.Value(), out Guid projectId))
            {
                return new ValidationResult($"The project id '{option.Value()}' is not a valid GUID.");
            }
            if (!_trainingApi.TryGetProject(projectId, out Project project))
            {
                return new ValidationResult($"Could not find any project with id '{projectId}'.");
            }

            return ValidationResult.Success;
        }
    }
}
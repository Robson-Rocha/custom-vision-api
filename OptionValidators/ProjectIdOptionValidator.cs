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

    public class ProjectIdOptionValidator : IOptionValidator, IDeferredValidation
    {
        private readonly Func<CustomVisionTrainingClient> _trainingApiInitializer;
        private CustomVisionTrainingClient TrainingApi {
            get
            {
                return _trainingApiInitializer();
            }
        }

        public ProjectIdOptionValidator(Func<CustomVisionTrainingClient> trainingApiInitializer)
        {
            this._trainingApiInitializer = trainingApiInitializer;
        }

        public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
        {
            if (option.HasValue() && !Guid.TryParse(option.Value(), out Guid projectId))
            {
                return new ValidationResult($"The project id '{option.Value()}' is not a valid GUID.");
            }

            return ValidationResult.Success;
        }

        public void ValidateDeferred(CommandOption option)
        {
            string configProjectId = Util.Configuration["projectId"];
            if (!option.HasValue() && string.IsNullOrWhiteSpace(configProjectId))
            {
                Util.Fail("No project id specified by --projectId command line option or projectId key in config.");
            }

            var projectId = Guid.Parse(option.Value() ?? configProjectId);
            if (!TrainingApi.TryGetProject(projectId, out Project project))
            {
                Util.Fail($"Could not find any project with id '{projectId}'.");
            }
        }
    }
}
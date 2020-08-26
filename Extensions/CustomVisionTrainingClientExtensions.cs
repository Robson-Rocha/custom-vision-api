namespace Exemplos.CustomVisionApi
{
    using System;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

    public static class CustomVisionTrainingClientExtensions
    {
        public static bool TryGetProject(this CustomVisionTrainingClient trainingApi, Guid projectId, out Project project)
        {
            project = null;
            try
            {
                project = trainingApi.GetProject(projectId);
                return true;
            }
            catch (CustomVisionErrorException)
            {
                return false;
            }
        }

        public static bool TryGetTag(this CustomVisionTrainingClient trainingApi, Guid projectId, Guid tagId, out Tag tag)
        {
            tag = null;
            try
            {
                tag = trainingApi.GetTag(projectId, tagId);
                return true;
            }
            catch (CustomVisionErrorException)
            {
                return false;
            }
        }
    }
}
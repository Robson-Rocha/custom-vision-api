namespace Exemplos.CustomVisionApi.Extensions
{
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
    using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;
    using System.Text.Json;

    public static class Util
    {
        private static string _configFilePath;

        //Obtém as configurações do arquivo appSettings.json
        private static readonly Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(_configFilePath)
                .Build()
        );

        public static void SetConfigFilePath(CommandOption configFileOption)
        {
            _configFilePath = configFileOption.Value();
            if (!File.Exists(_configFilePath))
            {
                Util.Fail($"The config file '{_configFilePath}' was not found");
            }
        }

        public static IConfiguration Configuration => _configuration.Value;

        private static CustomVisionTrainingClient _trainingApi;
        private static CustomVisionPredictionClient _predictionApi;

        public static CustomVisionTrainingClient GetTrainingApi()
        {
            string apiKey = Util.Configuration["customVision:training:key"];
            string endpoint = Util.Configuration["customVision:training:endpoint"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Util.Fail("No Custom Vision Training Key set in the config file.");
            }

            if (string.IsNullOrWhiteSpace(endpoint))
            {
                Util.Fail("No Custom Vision Training Endpoint set in the config file.");
            }

            return _trainingApi ?? (_trainingApi = new CustomVisionTrainingClient()
            {
                ApiKey = apiKey,
                Endpoint = endpoint
            });
        }

        public static CustomVisionPredictionClient GetPredictionApi()
        {
            string apiKey = Util.Configuration["customVision:prediction:key"];
            string endpoint = Util.Configuration["customVision:prediction:endpoint"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Util.Fail("No Custom Vision Prediction Key set in the config file.");
            }

            if (string.IsNullOrWhiteSpace(endpoint))
            {
                Util.Fail("No Custom Vision Prediction Endpoint set in the config file.");
            }

            return _predictionApi ?? (_predictionApi = new CustomVisionPredictionClient()
            {
                ApiKey = apiKey,
                Endpoint = endpoint
            });
        }

        private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions{WriteIndented = true};
        internal static void WriteObject<T>(T input)
        {
            Console.WriteLine(JsonSerializer.Serialize(input, _serializerOptions));
        }

        public static int FailureObject<T>(T input, int failureCode = -1)
        {
            return Failure(JsonSerializer.Serialize(input, _serializerOptions), failureCode);
        }

        public static int Failure(string message, int failureCode = -1)
        {
            var currentForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = currentForegroundColor;
            Console.Beep(300, 100);
            Console.Beep(300, 100);
            return failureCode;
        }

        public static void Fail(string message, int failureCode = -1)
        {
            Environment.Exit(Failure(message, failureCode));
        }

        internal static void Succeed()
        {
            Environment.Exit(Success());
        }
        
        public static int Success(bool beep = true)
        {
            if (beep) Console.Beep(800, 50);
            return 0;
        }

        public static int Success(string message)
        {
            var currentForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = currentForegroundColor;
            return Util.Success();
        }
    }
}

using System;
using System.IO;
using Exemplos.CustomVisionApi.Extensions;
using McMaster.Extensions.CommandLineUtils;

namespace Exemplos.CustomVisionApi.Commands.CreateConfig
{
    public class CreateConfigCommand : ICommand
    {
        private CommandOption _fileNameOption;

        public string CommandName => "create-config";

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Creates a new config file named 'config.json' at current path";
            _fileNameOption = command.Option("-n|--fileName", "Optional. Name of the config file. If not specified, defaults to 'config.json'.", CommandOptionType.SingleValue);
            command.HelpOption("-?|-h|--help");
        }

        public int Execute()
        {
            int i = 0;

            string fileName = _fileNameOption.Value() ?? "config.json";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileNameExtension = Path.GetExtension(fileName);

            string configFilePath = null;
            do {
                configFilePath = Path.Combine(Environment.CurrentDirectory, $"{fileNameWithoutExtension}{(i == 0 ? "" : $"({i})")}{fileNameExtension}");
                i++; 
            } while (File.Exists(configFilePath));
            File.WriteAllText(configFilePath, @"
{
  ""customVision"": {
    ""training"" : {
      ""key"": """",
      ""endpoint"": """"
    },
    ""prediction"": {
      ""key"": """",
      ""resourceId"": """",
      ""endpoint"": """"
    }
  },
  ""computerVision"": {
    ""key"": """",
    ""endpoint"": """"
  },
  ""projectId"": """"
}
");
            return Util.Success($"Config file created at '{configFilePath}'");
        }
    }
}
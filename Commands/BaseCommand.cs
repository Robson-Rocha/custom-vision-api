using System;
using Exemplos.CustomVisionApi.Extensions;
using McMaster.Extensions.CommandLineUtils;

namespace Exemplos.CustomVisionApi.Commands
{
    public abstract class BaseCommand : ICommand
    {
        protected CommandOption _configFileOption;

        public abstract string CommandName { get; }

        protected CommandLineApplication _command;

        public virtual void Configure(CommandLineApplication command)
        {
            _command = command;
            command.HelpOption("-?|-h|--help");
            _configFileOption = command.AddConfigFileOption();
        }

        public virtual int Execute()
        {
            Util.SetConfigFilePath(_configFileOption);
            _command.ValidateDeferred();
            return Util.Success(false);
        }
    }

    public abstract class BaseCommandWithProjectId : BaseCommand
    {
        protected CommandOption _projectIdOption;

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            _projectIdOption = command.Option("--projectId|-p", "Required if not in config file. The id of the project.", CommandOptionType.SingleValue);
            _projectIdOption.Validators.Add(new ProjectIdOptionValidator(Util.GetTrainingApi));
        }

        public virtual Guid GetProjectIdValue()
        {
            string configProjectId = Util.Configuration["projectId"];
            return Guid.Parse(_projectIdOption.Value() ?? configProjectId);
        }
    }

    public abstract class BaseCommandWithTagId : BaseCommandWithProjectId
    {
        private CommandOption _tagIdOption;

        public override void Configure(CommandLineApplication command)
        {
            base.Configure(command);
            _tagIdOption = command.Option("--tagId|-t", "Required. The id of the tag whose images shall be listed or counted.", CommandOptionType.SingleValue).IsRequired();
            _tagIdOption.Validators.Add(new TagIdOptionValidator(Util.GetTrainingApi, _projectIdOption));
        }

        public virtual Guid GetTagIdValue()
        {
            return Guid.Parse(_tagIdOption.Value());
        }
    }
}
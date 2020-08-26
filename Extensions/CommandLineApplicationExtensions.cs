namespace Exemplos.CustomVisionApi.Extensions
{
    using System.Linq;
    using Exemplos.CustomVisionApi.Commands;
    using Exemplos.CustomVisionApi.OptionValidators;
    using McMaster.Extensions.CommandLineUtils;

    public static class CommandLineApplicationExtensions
    {
        public static CommandOption AddConfigFileOption(this CommandLineApplication commandLineApplication)
        {
            return commandLineApplication.Option("--configFile|-c", "Required. Path of the config file to be used.", CommandOptionType.SingleValue).IsRequired();
        }

        public static CommandLineApplication AddCommand(this CommandLineApplication commandLineApplication, ICommand command)
        {
            commandLineApplication.Command(command.CommandName, command.Configure)
                                  .OnExecute(() => command.Execute()); //The call is ambiguous between Func<int> and Task<Func<int>>
            return commandLineApplication;
        }

        public static void ValidateDeferred(this CommandLineApplication commandLineApplication)
        {
            foreach (var option in commandLineApplication.Options)
            {
                foreach (var validator in option.Validators.OfType<IDeferredValidation>())
                {
                    validator.ValidateDeferred(option);
                }
            }
        }

    }
}

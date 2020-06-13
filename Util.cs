namespace Exemplos.CustomVisionApi
{
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;

    public interface ICommand
    {
        string CommandName { get; }

        void Configure(CommandLineApplication command);

        int Execute();
    }

    public static class CommandLineApplicationExtensions
    {
        public static CommandLineApplication AddCommand<TCommand>(this CommandLineApplication commandLineApplication)
            where TCommand : ICommand, new()
        {
            ICommand command = new TCommand();
            commandLineApplication.Command(command.CommandName, command.Configure)
                                  .OnExecute(() => command.Execute()); //The call is ambiguous between Func<int> and Task<Func<int>>
            return commandLineApplication;
        }

        public static CommandLineApplication AddCommand(this CommandLineApplication commandLineApplication, ICommand command)
        {
            commandLineApplication.Command(command.CommandName, command.Configure)
                                  .OnExecute(() => command.Execute()); //The call is ambiguous between Func<int> and Task<Func<int>>
            return commandLineApplication;
        }
    }

    public static class Util
    {
        //Obtém as configurações do arquivo appSettings.json
        private static readonly Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .Build()
        );
        public static IConfiguration Configuration => _configuration.Value;

        public static int Failure(string message, int failureCode = -1)
        {
            Console.WriteLine(message);
            return failureCode;
        }
        
        public static int Success(string message)
        {
            Console.WriteLine(message);
            return 0;
        }
    }
}

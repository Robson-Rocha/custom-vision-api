namespace Exemplos.CustomVisionApi.OptionValidators
{
    using McMaster.Extensions.CommandLineUtils;

    public interface IDeferredValidation
    {
         void ValidateDeferred(CommandOption option);
    }
}
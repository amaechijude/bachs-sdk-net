using Bachs.Exceptions;
using Microsoft.Extensions.Options;

namespace Bachs.Common;

internal sealed class BachsClientOptionsValidator : IValidateOptions<BachsClientOptions>
{
    public ValidateOptionsResult Validate(string? name, BachsClientOptions options)
    {
        try
        {
            options.Validate();
            return ValidateOptionsResult.Success;
        }
        catch (BachsInvalidOperationException ex)
        {
            return ValidateOptionsResult.Fail(ex.Message);
        }
    }
}

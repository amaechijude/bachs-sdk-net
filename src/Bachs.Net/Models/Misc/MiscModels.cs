using System.Text.Json.Serialization;
using Bachs.Net.Models.Common;

namespace Bachs.Net.Models.Misc;

public sealed record PaymentMethodInfo
{
    [JsonPropertyName("method")]
    public string Method { get; init; } = string.Empty;

    [JsonPropertyName("currencies")]
    public IReadOnlyList<Currency> Currencies { get; init; } = Array.Empty<Currency>();
}

public sealed record BankInfo
{
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

public sealed record ResolveBankAccountRequest
{
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; init; } = string.Empty;

    [JsonPropertyName("bank_code")]
    public string BankCode { get; init; } = string.Empty;
}

public sealed record ResolveBankAccountResponse
{
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; init; } = string.Empty;

    [JsonPropertyName("account_name")]
    public string AccountName { get; init; } = string.Empty;

    [JsonPropertyName("bank_code")]
    public string BankCode { get; init; } = string.Empty;
}

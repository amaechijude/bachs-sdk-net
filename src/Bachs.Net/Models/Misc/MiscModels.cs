using System.Text.Json.Serialization;

namespace Bachs.Net.Models.Misc;

public record PaymentMethodInfo
{
    [JsonPropertyName("method")]
    public string Method { get; init; } = string.Empty;

    [JsonPropertyName("currencies")]
    public IReadOnlyList<string> Currencies { get; init; } = Array.Empty<string>();
}

public record BankInfo
{
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

public class ResolveBankAccountRequest
{
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; } = string.Empty;

    [JsonPropertyName("bank_code")]
    public string BankCode { get; set; } = string.Empty;
}

public record ResolveBankAccountResponse
{
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; init; } = string.Empty;

    [JsonPropertyName("account_name")]
    public string AccountName { get; init; } = string.Empty;

    [JsonPropertyName("bank_code")]
    public string BankCode { get; init; } = string.Empty;
}

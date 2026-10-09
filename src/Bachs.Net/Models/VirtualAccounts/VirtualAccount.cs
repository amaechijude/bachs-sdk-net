using System.Text.Json.Serialization;

namespace Bachs.Net.Models.VirtualAccounts;

/// <summary>
/// Represents a fixed bank account number issued for continuous deposit collections.
/// </summary>
public sealed record VirtualAccount
{
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; init; } = string.Empty;

    [JsonPropertyName("account_name")]
    public string AccountName { get; init; } = string.Empty;

    [JsonPropertyName("bank_name")]
    public string BankName { get; init; } = string.Empty;

    [JsonPropertyName("bank_code")]
    public string? BankCode { get; init; }

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = "NGN";

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class CreateVirtualAccountRequest
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "NGN";
}

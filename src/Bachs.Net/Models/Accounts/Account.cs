using System.Text.Json.Serialization;
using Bachs.Net.Common;

namespace Bachs.Net.Models.Accounts;

/// <summary>
/// Represents a connected account (sub-account) on Bachs platform.
/// </summary>
public sealed record Account
{
    private readonly string? _id;
    private readonly string? _accountId;

    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _accountId ?? string.Empty;
        init => _id = value;
    }

    [JsonPropertyName("account_id")]
    public string AccountId
    {
        get => !string.IsNullOrEmpty(_accountId) ? _accountId : _id ?? string.Empty;
        init => _accountId = value;
    }

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("business_name")]
    public string? BusinessName { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record CreateAccountRequest
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "standard";

    [JsonPropertyName("country")]
    public string Country { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("business_name")]
    public string? BusinessName { get; init; }

    [JsonPropertyName("capabilities")]
    public IList<string>? Capabilities { get; init; }
}

public sealed record UpdateAccountRequest
{
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("business_name")]
    public string? BusinessName { get; init; }
}

public sealed record CreateAccountLinkRequest
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "onboarding";

    [JsonPropertyName("return_url")]
    public string ReturnUrl { get; init; } = string.Empty;

    [JsonPropertyName("refresh_url")]
    public string? RefreshUrl { get; init; }
}

public sealed record AccountLink
{
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; init; }
}

public sealed record Person
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("relationship")]
    public PersonRelationship? Relationship { get; init; }
}

public sealed record PersonRelationship
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("owner")]
    public bool Owner { get; init; }

    [JsonPropertyName("executive")]
    public bool Executive { get; init; }
}

public sealed record CreatePersonRequest
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; init; }
}

public sealed class ListAccountsOptions : ListRequestOptions { }

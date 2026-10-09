using System.Text.Json.Serialization;
using Bachs.Net.Common;

namespace Bachs.Net.Models.Accounts;

/// <summary>
/// Represents a connected account (sub-account) on Bachs platform.
/// </summary>
public record Account
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonIgnore]
    public string AccountId => Id;

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

public class CreateAccountRequest
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "standard";

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("business_name")]
    public string? BusinessName { get; set; }

    [JsonPropertyName("capabilities")]
    public IList<string>? Capabilities { get; set; }
}

public class UpdateAccountRequest
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("business_name")]
    public string? BusinessName { get; set; }
}

public class CreateAccountLinkRequest
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "onboarding";

    [JsonPropertyName("return_url")]
    public string ReturnUrl { get; set; } = string.Empty;

    [JsonPropertyName("refresh_url")]
    public string? RefreshUrl { get; set; }
}

public record AccountLink
{
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; init; }
}

public record Person
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

public record PersonRelationship
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("owner")]
    public bool Owner { get; init; }

    [JsonPropertyName("executive")]
    public bool Executive { get; init; }
}

public class CreatePersonRequest
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

public class ListAccountsOptions : ListRequestOptions
{
}

using System.Text.Json.Serialization;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;

namespace Bachs.Net.Models.Customers;

/// <summary>
/// Represents a customer in Bachs.
/// </summary>
public sealed record Customer
{
    /// <summary>
    /// Unique identifier for the customer (starts with 'cust_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Alias for <see cref="Id"/>.
    /// </summary>
    [JsonIgnore]
    public string CustomerId => Id;

    /// <summary>
    /// Customer's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Customer's full name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Customer's phone number in E.164 format.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Customer's billing address.
    /// </summary>
    [JsonPropertyName("billing_address")]
    public Address? BillingAddress { get; init; }

    /// <summary>
    /// Custom key-value pairs stored with this customer.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// When the customer record was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the customer was last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}

/// <summary>
/// Parameters for creating a new customer.
/// </summary>
public sealed record CreateCustomerRequest
{
    /// <summary>
    /// Customer's email address (required).
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Customer's full name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Customer's phone number.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Customer's billing address.
    /// </summary>
    [JsonPropertyName("billing_address")]
    public Address? BillingAddress { get; init; }

    /// <summary>
    /// Custom key-value pairs.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Parameters for updating an existing customer.
/// </summary>
public sealed record UpdateCustomerRequest
{
    /// <summary>
    /// Updated email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Updated name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Updated phone number.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Updated billing address. Note: Sending an address completely replaces the stored address.
    /// </summary>
    [JsonPropertyName("billing_address")]
    public Address? BillingAddress { get; init; }

    /// <summary>
    /// Custom key-value pairs.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Options for listing customers.
/// </summary>
public sealed class ListCustomersOptions : ListRequestOptions
{
    /// <summary>
    /// Search term to filter customers by email or name.
    /// </summary>
    public string? Search { get; set; }

    public override IDictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();
        if (!string.IsNullOrWhiteSpace(Search))
        {
            parameters["search"] = Search;
        }
        return parameters;
    }
}

/// <summary>
/// Response when creating an authenticated customer portal session.
/// </summary>
public sealed record CustomerPortalSession
{
    /// <summary>
    /// The temporary authenticated URL to redirect the customer to.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// When this portal session link expires.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }
}

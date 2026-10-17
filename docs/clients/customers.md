# Customers & Billing Portal Client

The Customers client manages customer records, contact information, billing addresses, and self-service Customer Portal sessions in Bachs.

- **Interface**: [`ICustomersClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Clients/ICustomersClient.cs)
- **Implementation**: [`CustomersClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Clients/CustomersClient.cs)
- **Property**: `client.Customers`
- **DI Service**: `services.AddBachsClient(...)` registers `ICustomersClient` as a transient service.

> **Handling responses:** Every client method returns `BachsResponse<T>` and returns (rather than throws) on API errors (4xx/5xx). Always check `response.IsSuccessStatusCode` first: on **failure**, inspect `response.Error`/`response.StatusCode` and handle it (log it, retry, or surface it to the caller); on **success**, read the parsed payload from `response.Data`. Transport, timeout, circuit-breaker, and serialization failures still throw typed `Bachs.Net.Exceptions` — see [Response Handling](../README.md#response-handling).

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateAsync`](#1-create-a-customer) | `POST` | `/v1/customers` | Creates a new customer profile. |
| [`GetAsync`](#2-retrieve-a-customer) | `GET` | `/v1/customers/{customerId}` | Retrieves customer details by ID. |
| [`ListAsync`](#3-list-customers) | `GET` | `/v1/customers` | Lists customers with filtering by email and pagination. |
| [`UpdateAsync`](#4-update-a-customer) | `PATCH` | `/v1/customers/{customerId}` | Updates customer email, phone, name, or metadata. |
| [`CreatePortalSessionAsync`](#5-create-customer-portal-session) | `POST` | `/v1/customers/{customerId}/portal-sessions` | Generates a secure hosted Customer Portal URL for self-service subscription management. |

---

## Usage Examples

### 1. Create a Customer

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Models.Common;
using Bachs.Net.Models.Customers;

var response = await client.Customers.CreateAsync(new CreateCustomerRequest
{
    Email = "alex.rivera@example.com",
    Name = "Alex Rivera",
    PhoneNumber = "+15551234567",
    BillingAddress = new Address
    {
        Line1 = "100 Market St",
        City = "San Francisco",
        State = "CA",
        PostalCode = "94105",
        Country = "US"
    },
    Metadata = new Dictionary<string, string>
    {
        ["user_id"] = "usr_55481"
    }
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Customer creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var customer = response.Data;

Console.WriteLine($"Customer ID: {customer.Id}");
```

---

### 2. Retrieve a Customer

```csharp
var response = await client.Customers.GetAsync("cus_123456789");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Customer lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var customer = response.Data;

Console.WriteLine($"Name: {customer.Name}");
Console.WriteLine($"Email: {customer.Email}");
Console.WriteLine($"Phone: {customer.PhoneNumber}");
Console.WriteLine($"Registered: {customer.CreatedAt:yyyy-MM-dd}");
```

---

### 3. List Customers

```csharp
var response = await client.Customers.ListAsync(new ListCustomersOptions
{
    Email = "alex.rivera@example.com",
    Limit = 10
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Customer list request failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var results = response.Data;

foreach (var c in results.Items)
{
    Console.WriteLine($"Customer: {c.Id} - {c.Name} <{c.Email}>");
}
```

---

### 4. Update a Customer

```csharp
var response = await client.Customers.UpdateAsync("cus_123456789", new UpdateCustomerRequest
{
    Name = "Alex Rivera-Smith",
    PhoneNumber = "+15559876543",
    BillingAddress = new Address
    {
        Line1 = "250 Montgomery St",
        City = "San Francisco",
        State = "CA",
        PostalCode = "94104",
        Country = "US"
    }
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Customer update failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var updated = response.Data;
```

---

### 5. Create Customer Portal Session

The Customer Portal allows users to update payment methods, view invoices, and manage subscriptions without you building a custom billing UI:

```csharp
var response = await client.Customers.CreatePortalSessionAsync("cus_123456789");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Customer portal session creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var portal = response.Data;

Console.WriteLine($"Direct user to Portal URL: {portal.Url}");
Console.WriteLine($"Session expires at: {portal.ExpiresAt}");
```

---

## Models

### [`Customer`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Models/Customers/Customer.cs)
- `Id` (`string`): Starts with `cus_`. Also exposed as `CustomerId` (the `customer_id` webhook alias).
- `Email` (`string`): Customer email address.
- `Name` (`string?`): Full name or business entity.
- `PhoneNumber` (`string?`): E.164 phone number.
- `BillingAddress` (`Address?`): Street address, city, country, postal code.
- `Metadata` (`IReadOnlyDictionary<string, string>?`): Stored key-value pairs.
- `CreatedAt` (`DateTimeOffset`): Timestamp of creation.
- `UpdatedAt` (`DateTimeOffset?`): Timestamp of last update.

### [`CustomerPortalSession`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Models/Customers/Customer.cs)
- `Url` (`string`): Hosted URL to redirect the customer.
- `ExpiresAt` (`DateTimeOffset?`): Expiration timestamp of the portal session link.

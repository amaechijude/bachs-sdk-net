# Customers & Billing Portal Client

The Customers client manages customer records, contact information, billing addresses, and self-service Customer Portal sessions in Bachs.

- **Interface**: [`ICustomersClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/ICustomersClient.cs)
- **Implementation**: [`CustomersClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/CustomersClient.cs)
- **Property**: `client.Customers`
- **DI Service**: `services.AddBachsClient(...)` registers `ICustomersClient` as a transient service.

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

var customer = await client.Customers.CreateAsync(new CreateCustomerRequest
{
    Email = "alex.rivera@example.com",
    Name = "Alex Rivera",
    Phone = "+15551234567",
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

Console.WriteLine($"Customer ID: {customer.Id}");
```

---

### 2. Retrieve a Customer

```csharp
var customer = await client.Customers.GetAsync("cus_123456789");

Console.WriteLine($"Name: {customer.Name}");
Console.WriteLine($"Email: {customer.Email}");
Console.WriteLine($"Phone: {customer.Phone}");
Console.WriteLine($"Registered: {customer.CreatedAt:yyyy-MM-dd}");
```

---

### 3. List Customers

```csharp
var results = await client.Customers.ListAsync(new ListCustomersOptions
{
    Email = "alex.rivera@example.com",
    Limit = 10
});

foreach (var c in results.Data)
{
    Console.WriteLine($"Customer: {c.Id} - {c.Name} <{c.Email}>");
}
```

---

### 4. Update a Customer

```csharp
var updated = await client.Customers.UpdateAsync("cus_123456789", new UpdateCustomerRequest
{
    Name = "Alex Rivera-Smith",
    Phone = "+15559876543",
    BillingAddress = new Address
    {
        Line1 = "250 Montgomery St",
        City = "San Francisco",
        State = "CA",
        PostalCode = "94104",
        Country = "US"
    }
});
```

---

### 5. Create Customer Portal Session

The Customer Portal allows users to update payment methods, view invoices, and manage subscriptions without you building a custom billing UI:

```csharp
var portal = await client.Customers.CreatePortalSessionAsync("cus_123456789");

Console.WriteLine($"Direct user to Portal URL: {portal.Url}");
Console.WriteLine($"Session expires at: {portal.ExpiresAt}");
```

---

## Models

### [`Customer`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Customers/Customer.cs)
- `Id` (`string`): Starts with `cus_`.
- `Email` (`string`): Customer email address.
- `Name` (`string?`): Full name or business entity.
- `Phone` (`string?`): E.164 phone number.
- `BillingAddress` (`Address?`): Street address, city, country, postal code.
- `CreatedAt` (`DateTimeOffset`): Timestamp of creation.

### [`CustomerPortalSession`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Customers/Customer.cs)
- `Id` (`string`): Portal session ID.
- `Url` (`string`): Hosted URL to redirect the customer.
- `ExpiresAt` (`DateTimeOffset?`): Expiration timestamp of the portal session link.

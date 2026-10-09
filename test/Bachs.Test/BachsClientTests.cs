using System.Net;
using System.Text;
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Products;

namespace Bachs.Test;

public sealed class BachsClientTests
{
    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastRequestBody { get; private set; }
        public HttpResponseMessage ResponseToReturn { get; set; } = new(HttpStatusCode.OK);

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (request.Content != null)
            {
                LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            return ResponseToReturn;
        }
    }

    [Fact]
    public async Task Client_Applies_DefaultHeaders()
    {
        var handler = new MockHttpMessageHandler
        {
            ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"items":[],"pagination":{"limit":20,"offset":0,"returned":0,"has_more":false}}""", Encoding.UTF8, "application/json")
            }
        };

        var httpClient = new HttpClient(handler);
        var client = new BachsClient(httpClient, new BachsClientOptions
        {
            ApiKey = "sk_sandbox_testkey123"
        });

        await client.Products.ListAsync();

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("Bearer", handler.LastRequest.Headers.Authorization?.Scheme);
        Assert.Equal("sk_sandbox_testkey123", handler.LastRequest.Headers.Authorization?.Parameter);
        Assert.Contains(handler.LastRequest.Headers.Accept, a => a.MediaType == "application/json");
        Assert.Contains(handler.LastRequest.Headers.UserAgent, u => u.Product?.Name == "Bachs.Net");
    }

    [Fact]
    public async Task Client_Sends_IdempotencyKey_And_AccountId()
    {
        var handler = new MockHttpMessageHandler
        {
            ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                    "checkout_id": "chk_123",
                    "checkout_url": "https://checkout.bachs.io/c/abc",
                    "status": "open",
                    "created_at": "2026-04-27T12:00:00Z"
                }
                """, Encoding.UTF8, "application/json")
            }
        };

        var httpClient = new HttpClient(handler);
        var client = new BachsClient(httpClient, new BachsClientOptions
        {
            ApiKey = "sk_sandbox_testkey123"
        });

        var requestOptions = new RequestOptions()
            .WithIdempotencyKey("idem-unique-123")
            .WithAccountId("acct_connected456");

        var result = await client.CheckoutSessions.CreateAsync(
            new CreateCheckoutSessionRequest
            {
                Pricing = new CheckoutPricingRequest { Currency = "USD", Amount = 50.00m },
                SuccessUrl = "https://example.com/success"
            },
            requestOptions);

        Assert.NotNull(result);
        Assert.Equal("chk_123", result.CheckoutId);
        Assert.NotNull(handler.LastRequest);
        Assert.True(handler.LastRequest.Headers.Contains("Idempotency-Key"));
        Assert.Equal("idem-unique-123", handler.LastRequest.Headers.GetValues("Idempotency-Key").First());
        Assert.True(handler.LastRequest.Headers.Contains("X-Account-Id"));
        Assert.Equal("acct_connected456", handler.LastRequest.Headers.GetValues("X-Account-Id").First());
    }

    [Fact]
    public async Task Client_Throws_BachsValidationException_On_422()
    {
        var errorJson = """
        {
            "detail": "Missing required field(s): name, price",
            "error_code": "VALIDATION_ERROR",
            "doc_url": "https://docs.bachs.io/api-reference/error-reference#general",
            "errors": [
                { "field": "name", "message": "Field required", "type": "missing" },
                { "field": "price", "message": "Field required", "type": "missing" }
            ]
        }
        """;

        var handler = new MockHttpMessageHandler
        {
            ResponseToReturn = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
            {
                Content = new StringContent(errorJson, Encoding.UTF8, "application/json")
            }
        };

        var httpClient = new HttpClient(handler);
        var client = new BachsClient(httpClient, new BachsClientOptions
        {
            ApiKey = "sk_sandbox_testkey123",
            MaxRetries = 0
        });

        var ex = await Assert.ThrowsAsync<BachsValidationException>(() =>
            client.Products.CreateAsync(new CreateProductRequest()));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, ex.StatusCode);
        Assert.Equal("VALIDATION_ERROR", ex.ErrorCode);
        Assert.Equal(2, ex.ValidationErrors.Count);
        Assert.Equal("name", ex.ValidationErrors[0].Field);
        Assert.Equal("Field required", ex.ValidationErrors[0].Message);
    }

    [Fact]
    public async Task Client_Throws_BachsRateLimitException_On_429()
    {
        var errorJson = """
        {
            "detail": "Rate limit exceeded",
            "error_code": "RATE_LIMIT_EXCEEDED"
        }
        """;

        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent(errorJson, Encoding.UTF8, "application/json")
        };
        response.Headers.Add("Retry-After", "10");
        response.Headers.Add("X-RateLimit-Limit", "100");
        response.Headers.Add("X-RateLimit-Remaining", "0");

        var handler = new MockHttpMessageHandler
        {
            ResponseToReturn = response
        };

        var httpClient = new HttpClient(handler);
        var client = new BachsClient(httpClient, new BachsClientOptions
        {
            ApiKey = "sk_sandbox_testkey123",
            MaxRetries = 0 // Disable retry to assert exception
        });

        var ex = await Assert.ThrowsAsync<BachsRateLimitException>(() =>
            client.Products.ListAsync());

        Assert.Equal(HttpStatusCode.TooManyRequests, ex.StatusCode);
        Assert.Equal("RATE_LIMIT_EXCEEDED", ex.ErrorCode);
        Assert.Equal(TimeSpan.FromSeconds(10), ex.RetryAfter);
        Assert.Equal(100, ex.RateLimit?.Limit);
        Assert.Equal(0, ex.RateLimit?.Remaining);
    }
}

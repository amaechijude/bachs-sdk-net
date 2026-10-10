using Bachs.Net.Clients;
using Bachs.Net.Extensions;
using Bachs.Net.WebHooks;
using Microsoft.Extensions.DependencyInjection;

namespace Bachs.Test;

public sealed class BachsServiceExtensionsTests
{
    [Fact]
    public void AddBachsClient_Registers_Client_And_SubClients()
    {
        var services = new ServiceCollection();
        services.AddBachsClient("sk_sandbox_testkey123");

        var serviceProvider = services.BuildServiceProvider();

        var client = serviceProvider.GetService<IBachsClient>();
        Assert.NotNull(client);
        Assert.Equal("sk_sandbox_testkey123", client.Options.ApiKey);

        // Verify sub-clients are resolvable
        Assert.NotNull(serviceProvider.GetService<ICheckoutSessionsClient>());
        Assert.NotNull(serviceProvider.GetService<IProductsClient>());
        Assert.NotNull(serviceProvider.GetService<ICustomersClient>());
        Assert.NotNull(serviceProvider.GetService<ISubscriptionsClient>());
        Assert.NotNull(serviceProvider.GetService<IPaymentsClient>());
        Assert.NotNull(serviceProvider.GetService<IRefundsClient>());
        Assert.NotNull(serviceProvider.GetService<IPayoutsClient>());
        Assert.NotNull(serviceProvider.GetService<IPayoutDestinationsClient>());
        Assert.NotNull(serviceProvider.GetService<IBalancesClient>());
        Assert.NotNull(serviceProvider.GetService<IConversionsClient>());
        Assert.NotNull(serviceProvider.GetService<ITransfersClient>());
        Assert.NotNull(serviceProvider.GetService<IDisputesClient>());
        Assert.NotNull(serviceProvider.GetService<IAccountsClient>());
        Assert.NotNull(serviceProvider.GetService<IVirtualAccountsClient>());
        Assert.NotNull(serviceProvider.GetService<IWebhooksClient>());
        Assert.NotNull(serviceProvider.GetService<IWebhookEventValidator>());
        Assert.NotNull(serviceProvider.GetService<IWebhookValidator>());
        Assert.NotNull(serviceProvider.GetService<IMiscClient>());
    }

    [Fact]
    public void AddBachsClient_Returns_IHttpClientBuilder_And_Allows_Resilience_Configuration()
    {
        var services = new ServiceCollection();
        IHttpClientBuilder builder = services.AddBachsClient(options =>
        {
            options.ApiKey = "sk_sandbox_testkey123";
            options.MaxRetries = 0; // Handled by resilience pipeline
        });

        Assert.NotNull(builder);
        Assert.NotNull(builder.Services);

        // Configure standard resilience pipeline from Microsoft.Extensions.Http.Resilience
        builder.AddStandardResilienceHandler();

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<IBachsClient>();
        Assert.NotNull(client);
        Assert.Equal("sk_sandbox_testkey123", client.Options.ApiKey);
        Assert.Equal(0, client.Options.MaxRetries);
    }

    [Fact]
    public void AddBachsClient_With_ConfigureHttpClient_Delegate_Configures_Resilience()
    {
        var services = new ServiceCollection();
        services.AddBachsClient(
            options =>
            {
                options.ApiKey = "sk_sandbox_testkey123";
                options.MaxRetries = 0;
            },
            builder =>
            {
                builder.AddStandardResilienceHandler();
            }
        );

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<IBachsClient>();
        Assert.NotNull(client);
        Assert.NotNull(serviceProvider.GetService<ICheckoutSessionsClient>());
    }

    [Fact]
    public void AddBachsClient_With_ApiKey_And_ConfigureHttpClient_Configures_Resilience()
    {
        var services = new ServiceCollection();
        services.AddBachsClient(
            "sk_sandbox_testkey123",
            builder => builder.AddStandardResilienceHandler()
        );

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<IBachsClient>();
        Assert.NotNull(client);
        Assert.NotNull(serviceProvider.GetService<IProductsClient>());
    }
}

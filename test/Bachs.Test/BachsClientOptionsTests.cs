using Bachs.Net.Common;

namespace Bachs.Test;

public class BachsClientOptionsTests
{
    [Fact]
    public void Validate_ThrowsInvalidOperationException_WhenApiKeyIsEmpty()
    {
        var options = new BachsClientOptions { ApiKey = "" };

        Assert.Throws<InvalidOperationException>(() => options.Validate());
    }

    [Fact]
    public void Validate_Succeeds_WhenApiKeyIsProvided()
    {
        var options = new BachsClientOptions { ApiKey = "sk_sandbox_test123" };

        options.Validate(); // Should not throw
    }

    [Fact]
    public void GetEffectiveBaseUrl_InfersSandbox_WhenKeyStartsWithSkSandbox()
    {
        var options = new BachsClientOptions { ApiKey = "sk_sandbox_abc123" };

        var url = options.GetEffectiveBaseUrl();

        Assert.Equal("https://sandbox-api.bachs.io/", url.ToString());
    }

    [Fact]
    public void GetEffectiveBaseUrl_InfersProduction_WhenKeyStartsWithSkLive()
    {
        var options = new BachsClientOptions { ApiKey = "sk_live_abc123" };

        var url = options.GetEffectiveBaseUrl();

        Assert.Equal("https://api.bachs.io/", url.ToString());
    }

    [Fact]
    public void GetEffectiveBaseUrl_UsesExplicitBaseUrl_WhenSpecified()
    {
        var options = new BachsClientOptions
        {
            ApiKey = "sk_sandbox_abc123",
            BaseUrl = "https://my-custom-proxy.internal"
        };

        var url = options.GetEffectiveBaseUrl();

        Assert.Equal("https://my-custom-proxy.internal/", url.ToString());
    }

    [Fact]
    public void GetEffectiveBaseUrl_UsesEnvironment_WhenExplicitlyConfigured()
    {
        var options = new BachsClientOptions
        {
            ApiKey = "sk_sandbox_test",
            Environment = BachsEnvironment.Production
        };

        var url = options.GetEffectiveBaseUrl();

        Assert.Equal("https://api.bachs.io/", url.ToString());
    }
}

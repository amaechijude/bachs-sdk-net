using Bachs.Net.Common;
using Bachs.Net.Models.Conversions;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing currency conversions.
/// </summary>
public sealed class ConversionsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IConversionsClient
{
    public Task<BachsResponse<ConversionQuote>> CreateQuoteAsync(
        CreateConversionQuoteRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateConversionQuoteRequest, ConversionQuote>(
            "v1/conversions/quotes",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<Conversion>> ExecuteAsync(
        ExecuteConversionRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<ExecuteConversionRequest, Conversion>(
            "v1/conversions",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<Conversion>> GetAsync(
        string conversionId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(conversionId);
        return BachsClient.GetAsync<Conversion>(
            $"v1/conversions/{Uri.EscapeDataString(conversionId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<BachsList<Conversion>>> ListAsync(
        ListConversionsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/conversions", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Conversion>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }
}

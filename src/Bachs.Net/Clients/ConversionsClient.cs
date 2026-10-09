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
    public Task<ConversionQuote> CreateQuoteAsync(
        CreateConversionQuoteRequest request,
        RequestOptions? requestOptions = null,
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

    public Task<Conversion> ExecuteAsync(
        ExecuteConversionRequest request,
        RequestOptions? requestOptions = null,
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

    public Task<Conversion> GetAsync(
        string conversionId,
        RequestOptions? requestOptions = null,
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

    public Task<BachsList<Conversion>> ListAsync(
        ListConversionsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = QueryBuilder.BuildUrl("v1/conversions", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Conversion>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }
}

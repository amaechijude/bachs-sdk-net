using Bachs.Net.Common;
using Bachs.Net.Models.Conversions;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for executing currency conversions between balance buckets.
/// </summary>
public interface IConversionsClient
{
    /// <summary>
    /// Locks an exchange rate quote for a conversion attempt.
    /// </summary>
    Task<ConversionQuote> CreateQuoteAsync(
        CreateConversionQuoteRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a currency conversion using a previously created quote ID.
    /// </summary>
    Task<Conversion> ExecuteAsync(
        ExecuteConversionRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a conversion record by its ID.
    /// </summary>
    Task<Conversion> GetAsync(
        string conversionId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists past conversion records with pagination and filtering.
    /// </summary>
    Task<BachsList<Conversion>> ListAsync(
        ListConversionsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}

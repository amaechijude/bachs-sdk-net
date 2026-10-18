using Bachs.Clients;
using Bachs.Common;
using Bachs.Models.Products;

namespace Bachs.Clients;

/// <summary>
/// Client implementation for managing products and catalog pricing.
/// </summary>
public sealed class ProductsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IProductsClient
{
    public Task<BachsResponse<Product>> CreateAsync(
        CreateProductRequest request,
        BachsRequestOptions requestOptions,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateProductRequest, Product>(
            "v1/products",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<Product>> GetAsync(
        string productId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);
        return BachsClient.GetAsync<Product>(
            $"v1/products/{Uri.EscapeDataString(productId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<BachsList<Product>>> ListAsync(
        ListProductsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/products", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Product>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<BachsResponse<Product>> UpdateAsync(
        string productId,
        UpdateProductRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PatchAsync<UpdateProductRequest, Product>(
            $"v1/products/{Uri.EscapeDataString(productId)}",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<Product>> ArchiveAsync(
        string productId,
        BachsRequestOptions requestOptions,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);
        return BachsClient.PostAsync<Product>(
            $"v1/products/{Uri.EscapeDataString(productId)}/archive",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<Product>> UnarchiveAsync(
        string productId,
        BachsRequestOptions requestOptions,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);
        return BachsClient.PostAsync<Product>(
            $"v1/products/{Uri.EscapeDataString(productId)}/unarchive",
            requestOptions,
            cancellationToken
        );
    }
}

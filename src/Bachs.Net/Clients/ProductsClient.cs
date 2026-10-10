using Bachs.Net.Common;
using Bachs.Net.Models.Products;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing products and catalog pricing.
/// </summary>
public sealed class ProductsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IProductsClient
{
    public Task<Product> CreateAsync(
        CreateProductRequest request,
        RequestOptions? requestOptions = null,
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

    public Task<Product> GetAsync(
        string productId,
        RequestOptions? requestOptions = null,
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

    public Task<BachsList<Product>> ListAsync(
        ListProductsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = QueryBuilder.BuildUrl("v1/products", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Product>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<Product> UpdateAsync(
        string productId,
        UpdateProductRequest request,
        RequestOptions? requestOptions = null,
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

    public Task<Product> ArchiveAsync(
        string productId,
        RequestOptions? requestOptions = null,
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

    public Task<Product> UnarchiveAsync(
        string productId,
        RequestOptions? requestOptions = null,
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

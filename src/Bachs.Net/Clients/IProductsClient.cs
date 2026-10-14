using Bachs.Net.Common;
using Bachs.Net.Models.Products;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for managing products and catalog pricing.
/// </summary>
public interface IProductsClient
{
    /// <summary>
    /// Creates a new product in your catalog.
    /// </summary>
    Task<Product> CreateAsync(
        CreateProductRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a single product by its ID.
    /// </summary>
    Task<Product> GetAsync(
        string productId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists products in your catalog with pagination.
    /// </summary>
    Task<BachsList<Product>> ListAsync(
        ListProductsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    Task<Product> UpdateAsync(
        string productId,
        UpdateProductRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Archives a product so it can no longer be used for new purchases.
    /// </summary>
    Task<Product> ArchiveAsync(
        string productId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Unarchives an archived product to active status.
    /// </summary>
    Task<Product> UnarchiveAsync(
        string productId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}

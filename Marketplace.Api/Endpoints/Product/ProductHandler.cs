using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Core;
using Marketplace.Core.Interfaces;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Product;
using Marketplace.Core.Validation;
using Marketplace.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.Product;

[WolverineHandler]
public class ProductHandler
{
    [Transactional]
    public async Task<ProductResponse> Handle(ProductRequest command, IProductRepository productRepository)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(productRepository, nameof(productRepository));

        Data.Entities.Product? product = null;

        if (command.ProductId.HasValue && command.ProductId > 0)
            product = await productRepository.GetByIdAsync(command.ProductId.Value);

        if (command.AllProducts)
        {
            var products = await productRepository.GetAllAsync();

            // Filter products based on request parameters
            var filteredProducts = products.Where(p =>
                (!command.CardId.HasValue || p.CardId == command.CardId) &&
                (string.IsNullOrEmpty(command.Category) || p.Category == command.Category) &&
                (string.IsNullOrEmpty(command.ProductType) || p.ProductType == command.ProductType) &&
                (!command.IsEnabled.HasValue || p.IsEnabled == command.IsEnabled) &&
                (!command.IsDeleted.HasValue || p.IsDeleted == command.IsDeleted)
            ).ToList();

            return new ProductResponse { Products = filteredProducts };
        }

        if (command.CardId.HasValue && product == null)
        {
            var cardProducts = await productRepository.GetProductsByCardIdAsync(command.CardId.Value);
            product = cardProducts.FirstOrDefault();
        }

        product ??= await productRepository.GetFirstOrDefaultAsync(p => true);
        return new ProductResponse
        {
            Product = product
        };
    }

    [Transactional]
    public async Task<ProductResponse> Handle(ProductCreate command, IProductRepository productRepository,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(productRepository, nameof(productRepository));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new ProductResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

        var currentUser = currentUserService.GetCurrentUserName();
        var product = new Data.Entities.Product
        {
            Title = command.Title,
            Description = command.Description,
            ProductType = command.ProductType,
            Category = command.Category,
            IsEnabled = command.IsEnabled,
            IsDeleted = command.IsDeleted,
            CardId = command.CardId,
            ProductDetailId = command.ProductDetailId,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = currentUser,
            ModifiedDate = DateTime.UtcNow
        };

        await productRepository.AddAsync(product);
        await productRepository.SaveChangesAsync();

        return new ProductResponse { Product = product };
    }

    [Transactional]
    public async Task<ProductResponse> Handle(ProductUpdate command, IProductRepository productRepository,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(productRepository, nameof(productRepository));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new ProductResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

        var product = await productRepository.GetByIdAsync(command.Id);
        if (product == null) return new ProductResponse { Product = null };

        product.Title = command.Title;
        product.Description = command.Description;
        product.ProductType = command.ProductType;
        product.Category = command.Category;
        product.IsEnabled = command.IsEnabled;
        product.IsDeleted = command.IsDeleted;
        product.CardId = command.CardId;
        product.ProductDetailId = command.ProductDetailId;
        product.ModifiedBy = currentUserService.GetCurrentUserName();
        product.ModifiedDate = DateTime.UtcNow;

        await productRepository.UpdateAsync(product);
        await productRepository.SaveChangesAsync();

        return new ProductResponse { Product = product };
    }

    [Transactional]
    public async Task Handle(ProductDelete command, IProductRepository productRepository,
        IProductDetailRepository productDetailRepository)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(productRepository, nameof(productRepository));
        ArgumentNullException.ThrowIfNull(productDetailRepository, nameof(productDetailRepository));

        var product = await productRepository.GetProductWithDetailsAsync(command.Id);

        if (product != null)
        {
            // Delete related ProductDetail and its children first
            if (product.ProductDetail != null)
                // Documents and Media will be deleted automatically due to cascade delete
                await productDetailRepository.DeleteAsync(product.ProductDetail.Id);

            // Now delete the product
            await productRepository.DeleteAsync(product.Id);
            await productRepository.SaveChangesAsync();
        }
    }
}
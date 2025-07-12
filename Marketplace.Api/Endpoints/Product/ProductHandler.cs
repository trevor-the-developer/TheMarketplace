using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Marketplace.Core.Services;
using Marketplace.Core.Validation;
using Marketplace.Core;
using Newtonsoft.Json;

namespace Marketplace.Api.Endpoints.Product;

[WolverineHandler]
public class ProductHandler
{
    [Transactional]
    public async Task<ProductResponse> Handle(ProductRequest command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        Data.Entities.Product? product = null;
        
        if (command.ProductId > 0)
        {
            product = await dbContext.Products.FindAsync(command.ProductId);
        }

        if (command.AllProducts)
        {
            var productsQuery = dbContext.Products.AsQueryable();
            
            if (command.CardId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CardId == command.CardId);
            }
            
            if (!string.IsNullOrEmpty(command.Category))
            {
                productsQuery = productsQuery.Where(p => p.Category == command.Category);
            }
            
            if (!string.IsNullOrEmpty(command.ProductType))
            {
                productsQuery = productsQuery.Where(p => p.ProductType == command.ProductType);
            }
            
            if (command.IsEnabled.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.IsEnabled == command.IsEnabled);
            }
            
            if (command.IsDeleted.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.IsDeleted == command.IsDeleted);
            }
            
            var products = await productsQuery.ToListAsync();
            return new ProductResponse() { Products = products };
        }

        if (command.CardId.HasValue && product == null)
        {
            product = await dbContext.Products.FirstOrDefaultAsync(p => p.CardId == command.CardId);
        }

        product ??= await dbContext.Products.FirstOrDefaultAsync();
        return new ProductResponse()
        {
            Product = product
        };
    }

    [Transactional]
    public async Task<ProductResponse> Handle(ProductCreate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
        {
            return new ProductResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                    StatusCode: StatusCodes.Status400BadRequest,
                    ErrorMessage: "Validation failed",
                    StackTrace: JsonConvert.SerializeObject(validationErrors)
                )
            };
        }

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

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return new ProductResponse { Product = product };
    }

    [Transactional]
    public async Task<ProductResponse> Handle(ProductUpdate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
        {
            return new ProductResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                    StatusCode: StatusCodes.Status400BadRequest,
                    ErrorMessage: "Validation failed",
                    StackTrace: JsonConvert.SerializeObject(validationErrors)
                )
            };
        }

        var product = await dbContext.Products.FindAsync(command.Id);
        if (product == null)
        {
            return new ProductResponse { Product = null };
        }

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

        await dbContext.SaveChangesAsync();

        return new ProductResponse { Product = product };
    }

    [Transactional]
    public async Task Handle(ProductDelete command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        var product = await dbContext.Products.FindAsync(command.Id);
        if (product != null)
        {
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }
    }
}

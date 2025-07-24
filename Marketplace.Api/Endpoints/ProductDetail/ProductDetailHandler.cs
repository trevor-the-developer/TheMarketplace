using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Core;
using Marketplace.Core.Interfaces;
using Marketplace.Core.Models;
using Marketplace.Core.Models.ProductDetail;
using Marketplace.Core.Validation;
using Marketplace.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.ProductDetail;

[WolverineHandler]
public class ProductDetailHandler
{
    [Transactional]
    public async Task<ProductDetailResponse> Handle(ProductDetailRequest command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        Data.Entities.ProductDetail? productDetail = null;

        if (command.ProductDetailId > 0)
            productDetail = await dbContext.ProductDetails
                .Include(pd => pd.Media)
                .Include(pd => pd.Documents)
                .FirstOrDefaultAsync(pd => pd.Id == command.ProductDetailId);

        if (command.AllProductDetails)
        {
            var productDetailsQuery = dbContext.ProductDetails
                .Include(pd => pd.Media)
                .Include(pd => pd.Documents)
                .AsQueryable();

            if (command.ProductId.HasValue)
                productDetailsQuery = productDetailsQuery.Where(pd => pd.ProductId == command.ProductId);

            var productDetails = await productDetailsQuery.ToListAsync();
            return new ProductDetailResponse { ProductDetails = productDetails };
        }

        if (command.ProductId.HasValue && productDetail == null)
            productDetail = await dbContext.ProductDetails
                .Include(pd => pd.Media)
                .Include(pd => pd.Documents)
                .FirstOrDefaultAsync(pd => pd.ProductId == command.ProductId);

        productDetail ??= await dbContext.ProductDetails
            .Include(pd => pd.Media)
            .Include(pd => pd.Documents)
            .FirstOrDefaultAsync();

        return new ProductDetailResponse
        {
            ProductDetail = productDetail
        };
    }

    [Transactional]
    public async Task<ProductDetailResponse> Handle(ProductDetailCreate command, MarketplaceDbContext dbContext,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new ProductDetailResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

        var currentUser = currentUserService.GetCurrentUserName();
        var productDetail = new Data.Entities.ProductDetail
        {
            Title = command.Title,
            Description = command.Description,
            ProductId = command.ProductId,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = currentUser,
            ModifiedDate = DateTime.UtcNow
        };

        dbContext.ProductDetails.Add(productDetail);
        await dbContext.SaveChangesAsync();

        return new ProductDetailResponse { ProductDetail = productDetail };
    }

    [Transactional]
    public async Task<ProductDetailResponse> Handle(ProductDetailUpdate command, MarketplaceDbContext dbContext,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new ProductDetailResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

        var productDetail = await dbContext.ProductDetails.FindAsync(command.Id);
        if (productDetail == null) return new ProductDetailResponse { ProductDetail = null };

        productDetail.Title = command.Title;
        productDetail.Description = command.Description;
        productDetail.ProductId = command.ProductId;
        productDetail.ModifiedBy = currentUserService.GetCurrentUserName();
        productDetail.ModifiedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return new ProductDetailResponse { ProductDetail = productDetail };
    }

    [Transactional]
    public async Task Handle(ProductDetailDelete command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        var productDetail = await dbContext.ProductDetails.FindAsync(command.Id);
        if (productDetail != null)
        {
            dbContext.ProductDetails.Remove(productDetail);
            await dbContext.SaveChangesAsync();
        }
    }
}
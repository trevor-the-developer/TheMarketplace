using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Marketplace.Core.Services;

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
        {
            productDetail = await dbContext.ProductDetails
                .Include(pd => pd.Media)
                .Include(pd => pd.Documents)
                .FirstOrDefaultAsync(pd => pd.Id == command.ProductDetailId);
        }

        if (command.AllProductDetails)
        {
            var productDetailsQuery = dbContext.ProductDetails
                .Include(pd => pd.Media)
                .Include(pd => pd.Documents)
                .AsQueryable();
            
            if (command.ProductId.HasValue)
            {
                productDetailsQuery = productDetailsQuery.Where(pd => pd.ProductId == command.ProductId);
            }
            
            var productDetails = await productDetailsQuery.ToListAsync();
            return new ProductDetailResponse() { ProductDetails = productDetails };
        }

        if (command.ProductId.HasValue && productDetail == null)
        {
            productDetail = await dbContext.ProductDetails
                .Include(pd => pd.Media)
                .Include(pd => pd.Documents)
                .FirstOrDefaultAsync(pd => pd.ProductId == command.ProductId);
        }

        productDetail ??= await dbContext.ProductDetails
            .Include(pd => pd.Media)
            .Include(pd => pd.Documents)
            .FirstOrDefaultAsync();
        
        return new ProductDetailResponse()
        {
            ProductDetail = productDetail
        };
    }

    [Transactional]
    public async Task<ProductDetailResponse> Handle(ProductDetailCreate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

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
    public async Task<ProductDetailResponse> Handle(ProductDetailUpdate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        var productDetail = await dbContext.ProductDetails.FindAsync(command.Id);
        if (productDetail == null)
        {
            return new ProductDetailResponse { ProductDetail = null };
        }

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

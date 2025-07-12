using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Marketplace.Core.Services;
using Marketplace.Core.Validation;
using Marketplace.Core;
using Newtonsoft.Json;

namespace Marketplace.Api.Endpoints.Media;

[WolverineHandler]
public class MediaHandler
{
    [Transactional]
    public async Task<MediaResponse> Handle(MediaRequest command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        Data.Entities.Media? media = null;
        
        if (command.MediaId > 0)
        {
            media = await dbContext.Files
                .FirstOrDefaultAsync(m => m.Id == command.MediaId);
        }

        if (command.AllMedia)
        {
            var mediaQuery = dbContext.Files.AsQueryable();
            
            if (command.ProductDetailId.HasValue)
            {
                mediaQuery = mediaQuery.Where(m => m.ProductDetailId == command.ProductDetailId);
            }

            if (!string.IsNullOrEmpty(command.MediaType))
            {
                mediaQuery = mediaQuery.Where(m => m.MediaType == command.MediaType);
            }
            
            var mediaList = await mediaQuery.ToListAsync();
            return new MediaResponse() { MediaList = mediaList };
        }

        if (command.ProductDetailId.HasValue && media == null)
        {
            media = await dbContext.Files
                .FirstOrDefaultAsync(m => m.ProductDetailId == command.ProductDetailId);
        }

        media ??= await dbContext.Files.FirstOrDefaultAsync();
        return new MediaResponse()
        {
            Media = media
        };
    }

    [Transactional]
    public async Task<MediaResponse> Handle(MediaCreate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
        {
            return new MediaResponse
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
        var media = new Data.Entities.Media
        {
            Title = command.Title,
            Description = command.Description,
            FilePath = command.FilePath,
            DirectoryPath = command.DirectoryPath,
            MediaType = command.MediaType,
            ProductDetailId = command.ProductDetailId,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = currentUser,
            ModifiedDate = DateTime.UtcNow
        };

        dbContext.Files.Add(media);
        await dbContext.SaveChangesAsync();

        return new MediaResponse { Media = media };
    }

    [Transactional]
    public async Task<MediaResponse> Handle(MediaUpdate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
        {
            return new MediaResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                    StatusCode: StatusCodes.Status400BadRequest,
                    ErrorMessage: "Validation failed",
                    StackTrace: JsonConvert.SerializeObject(validationErrors)
                )
            };
        }

        var media = await dbContext.Files.FindAsync(command.Id);
        if (media == null)
        {
            return new MediaResponse { Media = null };
        }

        media.Title = command.Title;
        media.Description = command.Description;
        media.FilePath = command.FilePath;
        media.DirectoryPath = command.DirectoryPath;
        media.MediaType = command.MediaType;
        media.ProductDetailId = command.ProductDetailId;
        media.ModifiedBy = currentUserService.GetCurrentUserName();
        media.ModifiedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return new MediaResponse { Media = media };
    }

    [Transactional]
    public async Task Handle(MediaDelete command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        var media = await dbContext.Files.FindAsync(command.Id);
        if (media != null)
        {
            dbContext.Files.Remove(media);
            await dbContext.SaveChangesAsync();
        }
    }
}

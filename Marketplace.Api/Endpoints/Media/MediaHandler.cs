using Marketplace.Core;
using Marketplace.Core.Interfaces;
using Marketplace.Core.Validation;
using Marketplace.Data.Interfaces;
using Newtonsoft.Json;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.Media;

[WolverineHandler]
public class MediaHandler
{
    [Transactional]
    public async Task<MediaResponse> Handle(MediaRequest command, IMediaRepository mediaRepository)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(mediaRepository, nameof(mediaRepository));

        Data.Entities.Media? media = null;

        if (command.MediaId > 0) media = await mediaRepository.GetByIdAsync(command.MediaId);

        if (command.AllMedia)
        {
            var mediaList = await mediaRepository.GetAllAsync();
            return new MediaResponse { MediaList = mediaList.ToList() };
        }

        media ??= await mediaRepository.GetFirstOrDefaultAsync(m => true);
        return new MediaResponse
        {
            Media = media
        };
    }

    [Transactional]
    public async Task<MediaResponse> Handle(MediaCreate command, IMediaRepository mediaRepository,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(mediaRepository, nameof(mediaRepository));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new MediaResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

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

        await mediaRepository.AddAsync(media);
        await mediaRepository.SaveChangesAsync();

        return new MediaResponse { Media = media };
    }

    [Transactional]
    public async Task<MediaResponse> Handle(MediaUpdate command, IMediaRepository mediaRepository,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(mediaRepository, nameof(mediaRepository));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new MediaResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

        var media = await mediaRepository.GetByIdAsync(command.Id);
        if (media == null) return new MediaResponse { Media = null };

        media.Title = command.Title;
        media.Description = command.Description;
        media.FilePath = command.FilePath;
        media.DirectoryPath = command.DirectoryPath;
        media.MediaType = command.MediaType;
        media.ProductDetailId = command.ProductDetailId;
        media.ModifiedBy = currentUserService.GetCurrentUserName();
        media.ModifiedDate = DateTime.UtcNow;

        await mediaRepository.UpdateAsync(media);
        await mediaRepository.SaveChangesAsync();

        return new MediaResponse { Media = media };
    }

    [Transactional]
    public async Task Handle(MediaDelete command, IMediaRepository mediaRepository)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(mediaRepository, nameof(mediaRepository));

        var media = await mediaRepository.GetByIdAsync(command.Id);
        if (media != null)
        {
            await mediaRepository.DeleteAsync(media.Id);
            await mediaRepository.SaveChangesAsync();
        }
    }
}
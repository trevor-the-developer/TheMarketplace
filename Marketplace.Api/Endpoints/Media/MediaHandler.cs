using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Core;
using Marketplace.Core.Interfaces;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Media;
using Marketplace.Core.Services;
using Marketplace.Core.Validation;
using Marketplace.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
    public async Task<MediaResponse> Handle(MediaCreateWithFile command, IMediaRepository mediaRepository,
        ICurrentUserService currentUserService, IValidationService validationService,
        IS3MediaService s3MediaService, ILogger<MediaHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(mediaRepository, nameof(mediaRepository));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));
        ArgumentNullException.ThrowIfNull(s3MediaService, nameof(s3MediaService));

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

        try
        {
            string? objectKey = null;
            string? directoryPath = null;

            // Upload file to S3 if provided
            if (command.FileStream != null && !string.IsNullOrEmpty(command.FileName))
            {
                // Create directory structure based on ProductDetailId
                directoryPath = $"products/{command.ProductDetailId}/media";
                
                // Generate unique filename to avoid conflicts
                var fileExtension = Path.GetExtension(command.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                
                objectKey = await s3MediaService.UploadFileAsync(
                    command.FileStream, 
                    uniqueFileName, 
                    command.ContentType ?? "application/octet-stream", 
                    directoryPath);

                logger.LogInformation("File uploaded to S3: {ObjectKey}", objectKey);
            }

            var currentUser = currentUserService.GetCurrentUserName();
            var media = new Data.Entities.Media
            {
                Title = command.Title,
                Description = command.Description,
                FilePath = objectKey, // S3 object key
                DirectoryPath = directoryPath, // S3 directory/prefix
                MediaType = command.MediaType,
                ProductDetailId = command.ProductDetailId,
                CreatedBy = currentUser,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = currentUser,
                ModifiedDate = DateTime.UtcNow
            };

            await mediaRepository.AddAsync(media);
            await mediaRepository.SaveChangesAsync();

            logger.LogInformation("Media record created with ID: {MediaId}", media.Id);

            return new MediaResponse { Media = media };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating media with file upload");
            
            return new MediaResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status500InternalServerError.ToString(),
                    StatusCodes.Status500InternalServerError,
                    "Error creating media",
                    ex.Message
                )
            };
        }
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
    public async Task Handle(MediaDelete command, IMediaRepository mediaRepository, IS3MediaService s3MediaService, ILogger<MediaHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(mediaRepository, nameof(mediaRepository));
        ArgumentNullException.ThrowIfNull(s3MediaService, nameof(s3MediaService));

        var media = await mediaRepository.GetByIdAsync(command.Id);
        if (media != null)
        {
            // Delete from S3 first if file exists
            if (!string.IsNullOrEmpty(media.FilePath))
            {
                try
                {
                    var deleted = await s3MediaService.DeleteFileAsync(media.FilePath);
                    if (!deleted)
                    {
                        logger.LogWarning("Failed to delete file from S3: {FilePath}", media.FilePath);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error deleting file from S3: {FilePath}", media.FilePath);
                }
            }

            await mediaRepository.DeleteAsync(media.Id);
            await mediaRepository.SaveChangesAsync();
        }
    }

    public async Task<Stream> Handle(MediaDownloadRequest command, IMediaRepository mediaRepository, IS3MediaService s3MediaService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(mediaRepository, nameof(mediaRepository));
        ArgumentNullException.ThrowIfNull(s3MediaService, nameof(s3MediaService));

        var media = await mediaRepository.GetByIdAsync(command.MediaId);
        if (media == null || string.IsNullOrEmpty(media.FilePath))
        {
            throw new FileNotFoundException($"Media file not found for ID: {command.MediaId}");
        }

        return await s3MediaService.DownloadFileAsync(media.FilePath);
    }

    // NEW: Handler for presigned URLs
    public async Task<MediaUrlResponse> Handle(MediaUrlRequest command, IMediaRepository mediaRepository, IS3MediaService s3MediaService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(mediaRepository, nameof(mediaRepository));
        ArgumentNullException.ThrowIfNull(s3MediaService, nameof(s3MediaService));

        var media = await mediaRepository.GetByIdAsync(command.MediaId);
        if (media == null || string.IsNullOrEmpty(media.FilePath))
        {
            throw new FileNotFoundException($"Media file not found for ID: {command.MediaId}");
        }

        var expiration = TimeSpan.FromHours(command.ExpirationHours ?? 1);
        var url = await s3MediaService.GetPresignedUrlAsync(media.FilePath, expiration);

        return new MediaUrlResponse
        {
            Url = url,
            ExpiresIn = expiration.TotalSeconds
        };
    }
}
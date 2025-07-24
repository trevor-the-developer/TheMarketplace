using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Marketplace.Core.Interfaces;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Media;
using Marketplace.Core.Validation;
using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wolverine.Attributes;

namespace Marketplace.Core.Services;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IS3MediaService _s3MediaService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidationService _validationService;
    private readonly ILogger<MediaService> _logger;

    public MediaService(
        IMediaRepository mediaRepository,
        IS3MediaService s3MediaService,
        ICurrentUserService currentUserService,
        IValidationService validationService,
        ILogger<MediaService> logger)
    {
        _mediaRepository = mediaRepository;
        _s3MediaService = s3MediaService;
        _currentUserService = currentUserService;
        _validationService = validationService;
        _logger = logger;
    }

    [Transactional]
    public async Task<MediaResponse> CreateMediaAsync(MediaCreateWithFile command)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        // Validate input
        var validationErrors = await _validationService.ValidateAndGetErrorsAsync(command);
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
                // Create directory structure based on ProductDetailId or other logic
                directoryPath = $"products/{command.ProductDetailId}/media";
                
                // Generate unique filename to avoid conflicts
                var fileExtension = Path.GetExtension(command.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                
                objectKey = await _s3MediaService.UploadFileAsync(
                    command.FileStream, 
                    uniqueFileName, 
                    command.ContentType ?? "application/octet-stream", 
                    directoryPath);

                _logger.LogInformation("File uploaded to S3: {ObjectKey}", objectKey);
            }

            var currentUser = _currentUserService.GetCurrentUserName();
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

            await _mediaRepository.AddAsync(media);
            await _mediaRepository.SaveChangesAsync();

            _logger.LogInformation("Media record created with ID: {MediaId}", media.Id);

            return new MediaResponse { Media = media };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating media");
            
            // Cleanup: Try to delete uploaded file if database operation failed
            if (!string.IsNullOrEmpty(command.FileName) && command.FileStream != null)
            {
                try
                {
                    var directoryPath = $"products/{command.ProductDetailId}/media";
                    var fileExtension = Path.GetExtension(command.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    var objectKey = $"{directoryPath}/{uniqueFileName}";
                    
                    await _s3MediaService.DeleteFileAsync(objectKey);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning(cleanupEx, "Failed to cleanup uploaded file during error handling");
                }
            }

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

    public async Task<MediaResponse> GetMediaAsync(int id)
    {
        try
        {
            var media = await _mediaRepository.GetByIdAsync(id);
            if (media == null)
            {
                return new MediaResponse
                {
                    ApiError = new ApiError(
                        StatusCodes.Status404NotFound.ToString(),
                        StatusCodes.Status404NotFound,
                        "Media not found",
                        $"Media with ID {id} was not found"
                    )
                };
            }

            return new MediaResponse { Media = media };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving media with ID: {MediaId}", id);
            return new MediaResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status500InternalServerError.ToString(),
                    StatusCodes.Status500InternalServerError,
                    "Error retrieving media",
                    ex.Message
                )
            };
        }
    }

    public async Task<Stream> GetMediaFileAsync(int id)
    {
        var media = await _mediaRepository.GetByIdAsync(id);
        if (media == null || string.IsNullOrEmpty(media.FilePath))
        {
            throw new FileNotFoundException($"Media file not found for ID: {id}");
        }

        return await _s3MediaService.DownloadFileAsync(media.FilePath);
    }

    public async Task<string> GetMediaPresignedUrlAsync(int id, TimeSpan? expiration = null)
    {
        var media = await _mediaRepository.GetByIdAsync(id);
        if (media == null || string.IsNullOrEmpty(media.FilePath))
        {
            throw new FileNotFoundException($"Media file not found for ID: {id}");
        }

        var exp = expiration ?? TimeSpan.FromHours(1); // Default 1 hour expiration
        return await _s3MediaService.GetPresignedUrlAsync(media.FilePath, exp);
    }

    [Transactional]
    public async Task<bool> DeleteMediaAsync(int id)
    {
        try
        {
            var media = await _mediaRepository.GetByIdAsync(id);
            if (media == null)
            {
                return false;
            }

            // Delete from S3 first
            if (!string.IsNullOrEmpty(media.FilePath))
            {
                var deleted = await _s3MediaService.DeleteFileAsync(media.FilePath);
                if (!deleted)
                {
                    _logger.LogWarning("Failed to delete file from S3: {FilePath}", media.FilePath);
                }
            }

            // Delete from database
            await _mediaRepository.DeleteAsync(media);
            await _mediaRepository.SaveChangesAsync();

            _logger.LogInformation("Media deleted successfully: {MediaId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting media: {MediaId}", id);
            return false;
        }
    }

    public async Task<IEnumerable<Media>> GetMediaByProductDetailIdAsync(int productDetailId)
    {
        return await _mediaRepository.GetMediaByProductDetailIdAsync(productDetailId);
    }
}

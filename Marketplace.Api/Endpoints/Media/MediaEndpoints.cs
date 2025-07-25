using System;
using System.IO;
using Marketplace.Core;
using Marketplace.Core.Constants;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Media;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Marketplace.Api.Endpoints.Media;

public static class MediaEndpoints
{
    public static void MapMediaEndpoints(this IEndpointRouteBuilder routes)
    {
        // POST /api/media - Create new media (metadata only)
        routes.MapPost(ApiConstants.ApiMediaCreate, async (MediaCreate command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<MediaResponse>(command);
                return response.ApiError != null 
                    ? Results.BadRequest(response)
                    : Results.Created($"{ApiConstants.ApiMedia}/{response.Media?.Id}", response);
            })
            .RequireAuthorization()
            .WithTags("Media")
            .WithName("Create Media")
            .Produces<MediaResponse>(StatusCodes.Status201Created)
            .Produces<ApiError>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // POST /api/media/upload - Create media with file upload
        routes.MapPost(ApiConstants.ApiMediaUpload, async ([FromForm] MediaCreateRequest request, IMessageBus bus) =>
            {
                var command = new MediaCreateWithFile
                {
                    Title = request.Title ?? string.Empty,
                    Description = request.Description,
                    MediaType = request.MediaType,
                    ProductDetailId = request.ProductDetailId,
                    FileStream = request.File?.OpenReadStream(),
                    FileName = request.File?.FileName,
                    ContentType = request.File?.ContentType
                };

                var response = await bus.InvokeAsync<MediaResponse>(command);
                
                return response.ApiError != null
                    ? Results.BadRequest(response)
                    : Results.Created($"{ApiConstants.ApiMedia}/{response.Media!.Id}", response);
            })
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithTags("Media")
            .WithName("Upload Media File")
            .Accepts<MediaCreateRequest>("multipart/form-data")
            .Produces<Data.Entities.Media>(StatusCodes.Status201Created)
            .Produces<ApiError>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/media/{id} - Update existing media
        routes.MapPut(ApiConstants.ApiMediaUpdateById, async (int id, MediaUpdate command, IMessageBus bus) =>
            {
                if (id != command.Id) 
                {
                    var badRequestResponse = new MediaResponse
                    {
                        ApiError = new ApiError(
                            StatusCodes.Status400BadRequest.ToString(),
                            StatusCodes.Status400BadRequest,
                            "Bad Request",
                            "ID mismatch between route and request body"
                        )
                    };
                    return Results.BadRequest(badRequestResponse);
                }

                var response = await bus.InvokeAsync<MediaResponse>(command);
                return response.ApiError != null 
                    ? Results.BadRequest(response)
                    : response.Media == null 
                        ? Results.NotFound(new MediaResponse
                        {
                            ApiError = new ApiError(
                                StatusCodes.Status404NotFound.ToString(),
                                StatusCodes.Status404NotFound,
                                "Not Found",
                                $"Media with ID {id} not found"
                            )
                        })
                        : Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Media")
            .WithName("Update Media")
            .Produces<MediaResponse>(StatusCodes.Status200OK)
            .Produces<ApiError>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/media/{id} - Delete media (includes S3 file deletion)
        routes.MapDelete(ApiConstants.ApiMediaDeleteById, async (int id, IMessageBus bus) =>
            {
                await bus.InvokeAsync(new MediaDelete { Id = id });
                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("Media")
            .WithName("Delete Media")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/media - Get all media
        routes.MapGet(ApiConstants.ApiAllMedia, async (IMessageBus bus) =>
            {
                var command = new MediaRequest { AllMedia = true };
                var response = await bus.InvokeAsync<MediaResponse>(command);

                return response?.ApiError != null 
                    ? Results.BadRequest(response)
                    : response?.MediaList != null 
                        ? Results.Ok(response) 
                        : Results.NotFound(new MediaResponse
                        {
                            ApiError = new ApiError(
                                StatusCodes.Status404NotFound.ToString(),
                                StatusCodes.Status404NotFound,
                                "Not Found",
                                "No media found"
                            )
                        });
            })
            .RequireAuthorization()
            .WithTags("Media")
            .WithName("Get All Media")
            .Produces<MediaResponse>(StatusCodes.Status200OK)
            .Produces<ApiError>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/media/{id} - Get media by ID
        routes.MapGet(ApiConstants.ApiGetMediaById, async (int id, IMessageBus bus) =>
            {
                var command = new MediaRequest { MediaId = id };
                var response = await bus.InvokeAsync<MediaResponse>(command);

                return response?.ApiError != null 
                    ? Results.BadRequest(response)
                    : response?.Media != null 
                        ? Results.Ok(response) 
                        : Results.NotFound(new MediaResponse
                        {
                            ApiError = new ApiError(
                                StatusCodes.Status404NotFound.ToString(),
                                StatusCodes.Status404NotFound,
                                "Not Found",
                                $"Media with ID {id} not found"
                            )
                        });
            })
            .RequireAuthorization()
            .WithTags("Media")
            .WithName("Get Media by Id")
            .Produces<MediaResponse>(StatusCodes.Status200OK)
            .Produces<ApiError>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/media/{id}/download - Download media file from S3
        routes.MapGet(ApiConstants.ApiMediaDownload, async (int id, IMessageBus bus) =>
            {
                try
                {
                    var fileStream = await bus.InvokeAsync<Stream>(new MediaDownloadRequest(id));
                    
                    // Get media info for proper content type and filename
                    var mediaResponse = await bus.InvokeAsync<MediaResponse>(new MediaRequest { MediaId = id });
                    var media = mediaResponse?.Media;
                    
                    var fileName = media?.Title ?? "download";
                    var contentType = media?.MediaType switch
                    {
                        "Video" => "video/mp4",
                        "Image" => "image/jpeg", 
                        "Audio" => "audio/mpeg",
                        _ => "application/octet-stream"
                    };

                    return Results.File(fileStream, contentType, fileName);
                }
                catch (FileNotFoundException)
                {
                    var notFoundResponse = new MediaResponse
                    {
                        ApiError = new ApiError(
                            StatusCodes.Status404NotFound.ToString(),
                            StatusCodes.Status404NotFound,
                            "File Not Found",
                            $"File for media ID {id} not found"
                        )
                    };
                    return Results.NotFound(notFoundResponse);
                }
                catch (Exception ex)
                {
                    var errorResponse = new MediaResponse
                    {
                        ApiError = new ApiError(
                            StatusCodes.Status500InternalServerError.ToString(),
                            StatusCodes.Status500InternalServerError,
                            "Download Error",
                            $"Error downloading file: {ex.Message}"
                        )
                    };
                    return Results.Problem(detail: errorResponse.ApiError.ErrorMessage, 
                                         statusCode: errorResponse.ApiError.StatusCode);
                }
            })
            .RequireAuthorization()
            .WithTags("Media")
            .WithName("Download Media File")
            .Produces(StatusCodes.Status200OK, typeof(FileResult), "application/octet-stream")
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/media/{id}/url - Get presigned URL for media file
        routes.MapGet(ApiConstants.ApiMediaByIdUrl, async (int id, [FromQuery] int? expirationHours, IMessageBus bus) =>
            {
                try
                {
                    var response = await bus.InvokeAsync<MediaUrlResponse>(new MediaUrlRequest(id, expirationHours));
                    return Results.Ok(response);
                }
                catch (FileNotFoundException)
                {
                    var notFoundResponse = new MediaResponse
                    {
                        ApiError = new ApiError(
                            StatusCodes.Status404NotFound.ToString(),
                            StatusCodes.Status404NotFound,
                            "File Not Found",
                            $"File for media ID {id} not found"
                        )
                    };
                    return Results.NotFound(notFoundResponse);
                }
                catch (Exception ex)
                {
                    var errorResponse = new MediaResponse
                    {
                        ApiError = new ApiError(
                            StatusCodes.Status500InternalServerError.ToString(),
                            StatusCodes.Status500InternalServerError,
                            "URL Generation Error",
                            $"Error generating URL: {ex.Message}"
                        )
                    };
                    return Results.Problem(detail: errorResponse.ApiError.ErrorMessage, 
                                         statusCode: errorResponse.ApiError.StatusCode);
                }
            })
            .RequireAuthorization()
            .WithTags("Media")
            .WithName("Get Media Presigned URL")
            .Produces<MediaUrlResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/media/product/{productDetailId} - Get media by product detail ID
        routes.MapGet(ApiConstants.ApiMediaByProductDetailId, async (int productDetailId, IMessageBus bus) =>
            {
                var command = new MediaRequest { ProductDetailId = productDetailId };
                var response = await bus.InvokeAsync<MediaResponse>(command);

                return response?.ApiError != null 
                    ? Results.BadRequest(response)
                    : response?.MediaList != null 
                        ? Results.Ok(response) 
                        : Results.NotFound(new MediaResponse
                        {
                            ApiError = new ApiError(
                                StatusCodes.Status404NotFound.ToString(),
                                StatusCodes.Status404NotFound,
                                "Not Found",
                                $"No media found for product detail ID {productDetailId}"
                            )
                        });
            })
            .RequireAuthorization()
            .WithTags("Media")
            .WithName("Get Media by Product Detail")
            .Produces<MediaResponse>(StatusCodes.Status200OK)
            .Produces<ApiError>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
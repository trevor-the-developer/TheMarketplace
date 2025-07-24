using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Marketplace.Core.Interfaces;
using Marketplace.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marketplace.Core.Services;

/// <summary>
/// Service class to interact with an AWS S3 object store
/// AWS SDK S3 developer guide: https://docs.aws.amazon.com/sdk-for-net/
/// </summary>
public class S3MediaService : IS3MediaService
{
    private readonly AmazonS3Client _s3Client;
    private readonly S3Configuration _config;
    private readonly ILogger<S3MediaService> _logger;

    public S3MediaService(IOptions<S3Configuration> config, ILogger<S3MediaService> logger)
    {
        _config = config.Value;
        _logger = logger;

        var s3Config = new AmazonS3Config
        {
            ServiceURL = _config.ServiceUrl,
            ForcePathStyle = true, // Required for GarageHQ/MinIO
            UseHttp = _config.ServiceUrl.StartsWith("http://"), // Use HTTP for local development
        };

        _s3Client = new AmazonS3Client(_config.AccessKey, _config.SecretKey, s3Config);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? directory = null)
    {
        try
        {
            var objectKey = directory != null ? $"{directory.TrimEnd('/')}/{fileName}" : fileName;
            
            var request = new PutObjectRequest
            {
                BucketName = _config.BucketName,
                Key = objectKey,
                InputStream = fileStream,
                ContentType = contentType,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.None // Adjust based on your needs
            };

            var response = await _s3Client.PutObjectAsync(request);
            
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("File uploaded successfully: {ObjectKey}", objectKey);
                return objectKey;
            }
            
            throw new Exception($"Failed to upload file. Status: {response.HttpStatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string objectKey)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _config.BucketName,
                Key = objectKey
            };

            var response = await _s3Client.GetObjectAsync(request);
            return response.ResponseStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {ObjectKey}", objectKey);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string objectKey)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _config.BucketName,
                Key = objectKey
            };

            var response = await _s3Client.DeleteObjectAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {ObjectKey}", objectKey);
            return false;
        }
    }

    public async Task<string> GetPresignedUrlAsync(string objectKey, TimeSpan expiration)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _config.BucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.Add(expiration),
                Verb = HttpVerb.GET
            };

            return await _s3Client.GetPreSignedURLAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL: {ObjectKey}", objectKey);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string objectKey)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _config.BucketName,
                Key = objectKey
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence: {ObjectKey}", objectKey);
            throw;
        }
    }

    public void Dispose()
    {
        _s3Client?.Dispose();
    }
}
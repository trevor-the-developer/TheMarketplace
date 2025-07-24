using System;
using System.IO;
using System.Threading.Tasks;

namespace Marketplace.Core.Interfaces;

public interface IS3MediaService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? directory = null);
    Task<Stream> DownloadFileAsync(string objectKey);
    Task<bool> DeleteFileAsync(string objectKey);
    Task<string> GetPresignedUrlAsync(string objectKey, TimeSpan expiration);
    Task<bool> FileExistsAsync(string objectKey);
}
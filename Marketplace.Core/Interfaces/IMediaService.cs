using Marketplace.Core.Models.Media;
using Marketplace.Data.Entities;

namespace Marketplace.Core.Interfaces;

public interface IMediaService
{
    Task<MediaResponse> CreateMediaAsync(MediaCreateWithFile command);
    Task<MediaResponse> GetMediaAsync(int id);
    Task<Stream> GetMediaFileAsync(int id);
    Task<string> GetMediaPresignedUrlAsync(int id, TimeSpan? expiration = null);
    Task<bool> DeleteMediaAsync(int id);
    Task<IEnumerable<Media>> GetMediaByProductDetailIdAsync(int productDetailId);
}
namespace Marketplace.Core.Models;

public record S3Configuration
{
    public string ServiceUrl { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string BucketName { get; init; } = string.Empty;
    public string Region { get; init; } = "garage";
}
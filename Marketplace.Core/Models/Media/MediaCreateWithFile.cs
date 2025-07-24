using System.IO;

namespace Marketplace.Core.Models.Media;

public class MediaCreateWithFile : MediaCreate
{
    public Stream? FileStream { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
}
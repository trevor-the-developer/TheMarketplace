namespace Marketplace.Core.Models.Media;

public record MediaUrlRequest(int MediaId, int? ExpirationHours = 1);
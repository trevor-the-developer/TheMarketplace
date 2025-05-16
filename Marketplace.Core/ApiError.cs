namespace Marketplace.Core
{
    public record ApiError(string? HttpStatusCode, int? StatusCode, string? ErrorMessage, string? StackTrace);
}

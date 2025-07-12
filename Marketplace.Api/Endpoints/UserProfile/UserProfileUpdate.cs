namespace Marketplace.Api.Endpoints.UserProfile;

public class UserProfileUpdate
{
    public string ApplicationUserId { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Bio { get; init; } = string.Empty;
    public string SocialMedia { get; init; } = string.Empty;
}

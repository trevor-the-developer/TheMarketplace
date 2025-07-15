namespace Marketplace.Api.Endpoints.UserProfile;

public class UserProfileCreate
{
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string SocialMedia { get; set; } = string.Empty;
    public string ApplicationUserId { get; set; } = string.Empty;
}
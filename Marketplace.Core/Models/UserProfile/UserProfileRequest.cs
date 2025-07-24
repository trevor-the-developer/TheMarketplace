namespace Marketplace.Core.Models.UserProfile;

public class UserProfileRequest
{
    public int? UserProfileId { get; set; }
    public string? ApplicationUserId { get; set; }
    public string? DisplayName { get; set; }
    public bool AllUserProfiles { get; set; } = false;
}
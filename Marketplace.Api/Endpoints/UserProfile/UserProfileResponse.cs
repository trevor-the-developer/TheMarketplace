namespace Marketplace.Api.Endpoints.UserProfile;

public class UserProfileResponse
{
    public Data.Entities.UserProfile? UserProfile { get; set; }
    public List<Data.Entities.UserProfile>? UserProfiles { get; set; }
}

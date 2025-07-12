using Marketplace.Core;

namespace Marketplace.Api.Endpoints.UserProfile;

public class UserProfileResponse
{
    public Data.Entities.UserProfile? UserProfile { get; set; }
    public List<Data.Entities.UserProfile>? UserProfiles { get; set; }
    public ApiError? ApiError { get; set; }
}

using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Marketplace.Core.Services;
using Marketplace.Core.Validation;
using Marketplace.Core;
using Newtonsoft.Json;

namespace Marketplace.Api.Endpoints.UserProfile;

[WolverineHandler]
public class UserProfileHandler
{
    [Transactional]
    public async Task<UserProfileResponse> Handle(UserProfileRequest command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        Data.Entities.UserProfile? userProfile = null;
        
        if (command.UserProfileId > 0)
        {
            userProfile = await dbContext.Profiles
                .Include(up => up.ApplicationUser)
                .FirstOrDefaultAsync(up => up.Id == command.UserProfileId);
        }

        if (command.AllUserProfiles)
        {
            var userProfilesQuery = dbContext.Profiles
                .Include(up => up.ApplicationUser)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(command.ApplicationUserId))
            {
                userProfilesQuery = userProfilesQuery.Where(up => up.ApplicationUserId == command.ApplicationUserId);
            }

            if (!string.IsNullOrEmpty(command.DisplayName))
            {
                userProfilesQuery = userProfilesQuery.Where(up => up.DisplayName.Contains(command.DisplayName));
            }
            
            var userProfiles = await userProfilesQuery.ToListAsync();
            return new UserProfileResponse() { UserProfiles = userProfiles };
        }

        if (!string.IsNullOrEmpty(command.ApplicationUserId) && userProfile == null)
        {
            userProfile = await dbContext.Profiles
                .Include(up => up.ApplicationUser)
                .FirstOrDefaultAsync(up => up.ApplicationUserId == command.ApplicationUserId);
        }

        if (!string.IsNullOrEmpty(command.DisplayName) && userProfile == null)
        {
            userProfile = await dbContext.Profiles
                .Include(up => up.ApplicationUser)
                .FirstOrDefaultAsync(up => up.DisplayName == command.DisplayName);
        }

        userProfile ??= await dbContext.Profiles
            .Include(up => up.ApplicationUser)
            .FirstOrDefaultAsync();
        
        return new UserProfileResponse()
        {
            UserProfile = userProfile
        };
    }

    [Transactional]
    public async Task<UserProfileResponse> Handle(UserProfileCreate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
        {
            return new UserProfileResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                    StatusCode: StatusCodes.Status400BadRequest,
                    ErrorMessage: "Validation failed",
                    StackTrace: JsonConvert.SerializeObject(validationErrors)
                )
            };
        }

        var currentUser = currentUserService.GetCurrentUserName();
        var userProfile = new Data.Entities.UserProfile
        {
            DisplayName = command.DisplayName,
            Bio = command.Bio,
            SocialMedia = command.SocialMedia,
            ApplicationUserId = command.ApplicationUserId,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = currentUser,
            ModifiedDate = DateTime.UtcNow
        };

        dbContext.Profiles.Add(userProfile);
        await dbContext.SaveChangesAsync();

        return new UserProfileResponse { UserProfile = userProfile };
    }

    [Transactional]
    public async Task<UserProfileResponse> Handle(UserProfileUpdate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
        {
            return new UserProfileResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                    StatusCode: StatusCodes.Status400BadRequest,
                    ErrorMessage: "Validation failed",
                    StackTrace: JsonConvert.SerializeObject(validationErrors)
                )
            };
        }

        var userProfile = await dbContext.Profiles.FirstOrDefaultAsync(up => up.ApplicationUserId == command.ApplicationUserId);
        if (userProfile == null)
        {
            return new UserProfileResponse { UserProfile = null };
        }

        userProfile.DisplayName = command.DisplayName;
        userProfile.Bio = command.Bio;
        userProfile.SocialMedia = command.SocialMedia;
        userProfile.ApplicationUserId = command.ApplicationUserId;
        userProfile.ModifiedBy = currentUserService.GetCurrentUserName();
        userProfile.ModifiedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return new UserProfileResponse { UserProfile = userProfile };
    }

    [Transactional]
    public async Task Handle(UserProfileDelete command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        var userProfile = await dbContext.Profiles.FirstOrDefaultAsync(up => up.ApplicationUserId == command.ApplicationUserId);
        if (userProfile != null)
        {
            dbContext.Profiles.Remove(userProfile);
            await dbContext.SaveChangesAsync();
        }
    }
}

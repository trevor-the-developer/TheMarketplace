namespace Marketplace.Core.Constants;

public static class AuthConstants
{
    // Auth-authz
    public const string AdminstratorRole = "Administrator";
    public const string UserRole = "User";
    public const string CreatedRoleAdministrator = "Created role 'Administrator'.";
    public const string CreatedRoleUser = "Created role 'User'.";
    public const string ErrorCreatingRoleAdministrator = "Error creating role 'Administrator'.";
    public const string ErrorCreatingRoleUser = "Error creating role 'User'";
    public const string UserDoesntExist = "User doesn't exist.";
    public const string InvalidEmailPassword = "Invalid email/password";
    public const string UserEmailNotConfirmed = "User email not confirmed.";
    public const string LoginSucceeded = "Login succeeded";
    public const string LoginFailed = "Login failed";
    public const string Unauthorised = "Unauthorised";
    public const string UserAlreadyExists = "User already exists";
    public const string RegistrationFailed = "Registration failed";
    
    // Jwt
    public const string RevokeFailed = "Revoke failed";
    public const string JwtNotPresent = "JWT not present";
    public const string SecretKeyNotConfigured = "Secret not configured";
    public const string JwtSettingsKey = "JwtSettings:Key";
    public const string JwtSettingsIssuer = "JwtSettings:Issuer";
    public const string JwtSettingsAudience = "JwtSettings:Audience";
    
    // Token
    public const string TokenRefreshRequest = "Token refresh request";
    public const string RefreshSucceeded = "Refresh succeeded";
    public const string TokenRevokeRequest = "Token revoke request";
    public const string RevokeSucceeded = "Revoke succeeded";
}
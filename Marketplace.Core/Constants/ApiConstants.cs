namespace Marketplace.Core.Constants;

public static class ApiConstants
{
    // Email Service
    public const string DefaultSmtpHost = "localhost";
    public const int DefaultSmtpPort = 1025;
    public const bool DefaultSmtpUseSsl= false;
    
    // Authentication
    public const string RegisterHandlerCalled = "Register handler called";
    public const string UserAlreadyExists = "User already exists";
    public const string Revoke = "Revoke";
    public const string Refresh = "Refresh";
    public const string Register = "Register";
    public const string ConfirmEmail = "ConfirmEmail";
    public const string Authentication = "Authentication";
    public const string Login = "Login";    
    
    // endpoints - Auth-Authz
    public const string ApiSlashLogin = "/api/auth/login";
    public const string ApiSlashRegister = "/api/auth/register";
    public const string ApiSlashConfirmEmail = "/api/auth/confirm-email";
    public const string ApiRefresh = "/api/auth/refresh";
    public const string ApiSlashLogout = "/api/auth/logout";
    
    // endpoints - cards
    public const string ApiCards = "/api/cards";
    public const string ApiCardsById = "/api/cards/{id}";
    
    // endpoints - documents
    public const string ApiDocuments = "/api/documents";
    public const string ApiDocumentsById = "/api/documents/{id}";
    
    // endpoints - listings
    public const string ApiListings = "/api/listings";
    public const string ApiListingsById = "/api/listings/{id}";
    
    // endpoints - media
    public const string ApiMedia = "/api/media";
    public const string ApiMediaById = "/api/media/{id}";

    // endpoints - products
    public const string ApiProducts = "/api/products";
    public const string ApiProductsById = "/api/products/{id}";
    
    // endpoints - product details
    public const string ApiProductDetails = "/api/product-details";
    public const string ApiProductDetailsById = "/api/product-details/{id}";
    
    // endpoints - tags
    public const string ApiTags = "/api/tags";
    public const string ApiTagsById = "/api/tags/{id}";
    
    // endpoints - user profiles
    public const string ApiUserProfiles = "/api/user-profiles";
    public const string ApiUserProfilesById = "/api/user-profiles/{id}";
}
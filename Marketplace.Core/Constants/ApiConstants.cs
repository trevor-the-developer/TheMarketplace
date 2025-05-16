namespace Marketplace.Core.Constants;

public static class ApiConstants
{
    // Api constants
    public const string RegisterHandlerCalled = "Register handler called";
    public const string UserAlreadyExists = "User already exists";
    public const string Revoke = "Revoke";
    public const string Refresh = "Refresh";
    public const string Register = "Register";
    public const string ConfirmEmail = "ConfirmEmail";
    public const string Authentication = "Authentication";
    public const string Login = "Login";    
    
    // endpoints
    public const string ApiSlashLogin = "/api/login/";
    public const string ApiSlashRegister = "/api/register/";
    public const string ApiSlashConfirmEmail = "api/confirm_email/";
    public const string ApiRefresh = "/api/refresh/";
    public const string ApiSlashLogout = "/api/logout/";
}
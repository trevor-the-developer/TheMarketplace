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
    
    // endpoints - Auth-Authz
    public const string ApiSlashLogin = "/api/login/";
    public const string ApiSlashRegister = "/api/register/";
    public const string ApiSlashRegisterStepTwo = "/api/register/step-two";
    public const string ApiSlashConfirmEmail = "api/confirm_email/";
    public const string ApiRefresh = "/api/refresh/";
    public const string ApiSlashLogout = "/api/logout/";
    
    // endpoints -card
    public const string ApiSlashCardCreate = "/api/card/create";
    public const string ApiSlashCardDelete = "/api/card/delete/{id}";
    public const string ApiSlashCardUpdate = "/api/card/update{id}";
    public const string ApiSlashGetCards = "/api/get/card";
    public const string ApiSlashGetAllCards = "/api/get/card/all/";
    public const string ApiSlashGetCardById = "/api/get/card/{cardId}";
    
    // endpoints - document
    public const string ApiSlashDocumentCreate =  "/api/document/create";
    public const string ApiSlashDocumentDelete = "/api/document/delete/{id}";
    public const string ApiSlashDocumentUpdate = "/api/document/update/{id}";
    public const string ApiSlashGetDocument = "/api/get/document/";
    public const string ApiSlashGetDocumentById = "/api/get/document/{documentId}";
    public const string ApiSlashGetAllDocuments =  "/api/get/documents/all";
    
    // endpoints - listing
    public const string ApiSlashListingCreate = "/api/listing/create";
    public const string ApiSlashListingDelete = "/api/listing/delete/{id}";
    public const string ApiSlashListingUpdate = "/api/listing/update/{id}";
    public const string ApiSlashGetListing = "/api/listing/get/listng/";
    public const string ApiSlashGetListingById = "/api/get/listing/{listingId}";
    public const string ApiSlashGetAllListings = "/api/get/listing/all";
    
    // endpoints - media
    public const string ApiSlashMediaCreate = "/api/media/create";
    public const string ApiSlashMediaUpdate = "/api/media/update/{id}";
    public const string ApiSlashMediaDelete = "/api/media/delete/{id}";
    public const string ApiSlashGetMedia = "/api/get/media/";
    public const string ApiSlashGetMediaById = "/api/get/media/{mediaId}";
    public const string ApiSlashGetAllMedia = "/api/get/media/all";

    // endpoints - product
    public const string ApiSlashProductCreate = "/api/product/create";
    public const string ApiSlashProductUpdate =  "/api/product/update/{id}";
    public const string ApiSlashProductDelete = "/api/product/delete/{id}";
    public const string ApiSlashGetProduct = "/api/get/product/";
    public const string ApiSlashGetProductById = "/api/get/product/{productId}";
    public const string ApiSlashGetAllProducts =  "/api/get/products/all";
    
    // endpoint - product detail
    public const string ApiSlashProductDetailCreate = "/api/productdetail/create";
    public const string ApiSlashProductDetailUpdate =  "/api/productdetail/update/{id}";
    public const string ApiSlashProductDetailDelete = "/api/productdetail/delete/{id}";
    public const string ApiSlashGetProductDetail = "/api/get/productdetail/";
    public const string ApiSlashGetProductDetailById = "/api/get/productdetail/{productDetailId}";
    public const string ApiSlashGetAllProductDetails =  "/api/get/productdetail/all";
    
    // endpoint - tag
    public const string ApiSlashTagCreate = "/api/tag/create";
    public const string ApiSlashTagUpdate =  "/api/tag/update/{id}";
    public const string ApiSlashTagDelete = "/api/tag/delete/{id}";
    public const string ApiSlashGetTag = "/api/get/tag/";
    public const string ApiSlashGetTagById = "/api/get/tag/{tagId}";
    public const string ApiSlashGetAllTags =  "/api/get/tag/all";
    
    // endpoint - user profile
    public const string ApiSlashUserProfileCreate = "/api/userprofile/create";
    public const string ApiSlashUserProfileUpdate = "/api/userprofile/update/{applicationUserId}";
    public const string ApiSlashUserProfileDelete = "/api/userprofile/delete/{applicationUserId}";
    public const string ApiSlashGetUserProfile = "/api/get/userprofile/";
    public const string ApiSlashGetUserProfileById = "/api/get/userprofile/{applicationUserId}";
    public const string ApiSlashGetAllUserProfiles = "/api/get/userprofiles/all";
}
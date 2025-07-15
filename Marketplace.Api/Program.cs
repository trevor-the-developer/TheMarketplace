using System.IdentityModel.Tokens.Jwt;
using Marketplace.Api.Endpoints.Authentication;
using Marketplace.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Oakton;
using Oakton.Resources;
using System.Text;
using Marketplace.Api.Endpoints.Card;
using Marketplace.Api.Endpoints.Listing;
using Marketplace.Api.Endpoints.Product;
using Marketplace.Api.Endpoints.ProductDetail;
using Marketplace.Api.Endpoints.Media;
using Marketplace.Api.Endpoints.Document;
using Marketplace.Api.Endpoints.Tag;
using Marketplace.Api.Endpoints.UserProfile;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.SqlServer;
using Marketplace.Core.Security;
using Marketplace.Core.Helpers;
using Marketplace.Data.Entities;
using Marketplace.Data.Repositories;
using Marketplace.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http;
using FluentValidation;

#region Host builder setup

var builder = WebApplication.CreateBuilder(args);

// must be called before builder.Build() to ensure that the Oakton commands are available
builder.Host.ApplyOaktonExtensions();

var connectionString = builder.Configuration.GetConnectionString("MarketplaceDbConnection");

#endregion

#region Wolverine

builder.Host.UseWolverine(opts =>
{
    opts.Services.AddDbContextWithWolverineIntegration<MarketplaceDbContext>(x =>
        x.UseSqlServer(connectionString), "wolverine");
    opts.PersistMessagesWithSqlServer(connectionString!, "wolverine");
    opts.UseEntityFrameworkCoreTransactions();
    opts.Policies.AutoApplyTransactions();

    // Optimises Wolverine for usage as strictly a mediator tool.
    // This completely disables all node persistence including the
    // inbox and outbox
    opts.Durability.Mode = DurabilityMode.MediatorOnly;
});

#endregion

#region Identity (Authentication and Authorisation)

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    // for public facing may want to consider 2fa see: https://rb.gy/lgx8w1
    options.SignIn.RequireConfirmedEmail = true;
})
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<MarketplaceDbContext>();

var secret = builder.Configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("Secret not configured.");

// clear JWT mapping prior to adding authentication
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = new TimeSpan(0, 0, 5),
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        NameClaimType = "name",
        RoleClaimType = "role"
    };
    // handle the events raised by the bearer token
    options.Events = new JwtBearerEvents
    {
        OnChallenge = ctx => TokenHelper.LogAttempt(ctx.Request.Headers, "OnChallenge"),
        OnTokenValidated = ctx => TokenHelper.LogAttempt(ctx.Request.Headers, "OnTokenValidated")
    };
})
.AddCookie("Cookies");

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build());

#endregion

#region Swagger

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Marketplace API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

#endregion

#region Builder services

// This is rebuilding the persistent storage database schema on startup
builder.Host.UseResourceSetupOnStartup();

builder.Services.AddResourceSetupOnStartup();
builder.Services.AddMvcCore(); // for JSON formatters

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

#endregion

#region Service registrations

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Register Repositories
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IListingRepository, ListingRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

// Register FluentValidation services
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddScoped<Marketplace.Core.Validation.IValidationService, Marketplace.Core.Validation.ValidationService>();

// Add IUrlHelper support
builder.Services.AddScoped<IUrlHelper>(factory =>
{
    var actionContextAccessor = factory.GetRequiredService<IActionContextAccessor>();
    var urlHelperFactory = factory.GetRequiredService<IUrlHelperFactory>();
    var httpContextAccessor = factory.GetRequiredService<IHttpContextAccessor>();
    
    // Create ActionContext with HttpContext if available
    var actionContext = actionContextAccessor.ActionContext;
    if (actionContext == null || actionContext.HttpContext == null)
    {
        actionContext = new ActionContext
        {
            HttpContext = httpContextAccessor.HttpContext ?? new DefaultHttpContext()
        };
    }
    
    return urlHelperFactory.GetUrlHelper(actionContext);
});
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

// Register TokenValidationParameters for dependency injection
builder.Services.AddSingleton<TokenValidationParameters>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var secret = configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("Secret not configured.");
    
    return new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false, // For refresh tokens, we allow expired tokens
        ValidateIssuerSigningKey = true,
        ValidAudience = configuration["JwtSettings:Audience"],
        ValidIssuer = configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        NameClaimType = "name",
        RoleClaimType = "role",
        ClockSkew = TimeSpan.Zero
    };
});

#endregion

#region App configuration

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

#endregion

// Ensure the database is created (skip in testing environment)
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        await scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>().Database.EnsureCreatedAsync();
    }
}

#region Endpoints

app.MapAuthenticationEndpoints();
app.MapListingEndpoints();
app.MapCardEndpoints();
app.MapProductEndpoints();
app.MapProductDetailEndpoints();
app.MapMediaEndpoints();
app.MapDocumentEndpoints();
app.MapTagEndpoints();
app.MapUserProfileEndpoints();

#endregion

//app.Run();
// This opts into using Oakton for extended command line options for this app
// Oakton is also a transitive dependency of Wolverine itself
return await app.RunOaktonCommands(args);
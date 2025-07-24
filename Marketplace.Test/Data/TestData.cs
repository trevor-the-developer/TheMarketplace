using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Marketplace.Test.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Test.Data;

public static class TestData
{
    // Authentication/authorization
    public static readonly string TestId = StringHelper.GuidToFlattenedString();
    public static readonly string? RefreshToken = StringHelper.GuidToFlattenedString();
    public static string TestUserOne = "admin@localhost";
    public static readonly string SecretKey = "622d70c6b650418c8bc7aa1e6e8c9ac6";
    public static readonly string Issuer = "MarketplaceApi";
    public static readonly string Audience = "MarketplaceApiClient";
    public static string TestEmail => "admin@localhost";

    public static ApplicationUser TestApplicationUser =>
        new()
        {
            Id = StringHelper.GuidToFlattenedString(),
            Email = TestUserOne,
            Role = Role.User,
            RefreshToken = RefreshToken,
            EmailConfirmed = false,
            UserProfile = new UserProfile
            {
                DisplayName = "Test User",
                Bio = "This is a test user",
                SocialMedia = "https://www.testuser.com",
                ApplicationUserId = TestId
            },
            RefreshTokenExpiry = DateTime.Now.AddDays(7)
        };

    public static JwtSecurityToken JwtSecurityToken
    {
        get
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Name, TestApplicationUser.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Sub, TestApplicationUser.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, TestApplicationUser.Email ?? string.Empty),
                new("userId", TestApplicationUser.Id)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                Issuer,
                Audience,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            return jwtSecurityToken;
        }
    }

    public static SecurityKey SecurityKey
    {
        get
        {
            var key = Encoding.ASCII.GetBytes(SecretKey);
            return new SymmetricSecurityKey(key);
        }
    }

    public static string AccessToken => new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);

    public static IdentityRole TestAppUserRole =>
        new()
        {
            Id = "1",
            Name = "User",
            NormalizedName = "USER"
        };
}
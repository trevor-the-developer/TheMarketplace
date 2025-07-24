using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Services;

public class EmailServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private readonly Mock<ILogger<EmailService>> _mockLogger;

    public EmailServiceTests()
    {
        _mockLogger = new Mock<ILogger<EmailService>>();

        // Create a real configuration with test values
        var configData = new Dictionary<string, string>
        {
            ["EmailSettings:SmtpHost"] = "localhost",
            ["EmailSettings:SmtpPort"] = "1025",
            ["EmailSettings:SmtpUseSSL"] = "false",
            ["EmailSettings:FromEmail"] = "admin@themarketplace.local",
            ["EmailSettings:FromName"] = "The Marketplace Admin"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        _emailService = new EmailService(_configuration, _mockLogger.Object);
    }

    [Fact]
    public void EmailService_Constructor_SetsUpConfigurationCorrectly()
    {
        // Assert - Constructor should complete without throwing
        Assert.NotNull(_emailService);
    }

    [Fact]
    public void EmailService_Constructor_UsesDefaultValues_WhenConfigurationMissing()
    {
        // Arrange - Empty configuration
        var emptyConfig = new ConfigurationBuilder().Build();

        // Act & Assert - Should not throw
        var emailService = new EmailService(emptyConfig, _mockLogger.Object);
        Assert.NotNull(emailService);
    }

    [Fact]
    public async Task SendConfirmationEmailAsync_WithValidParameters_ShouldSucceed_WhenMailHogRunning()
    {
        // Arrange
        var testEmail = "test@example.com";
        var testLink = "https://example.com/confirm?token=123";

        // Act
        // This test will attempt to connect to localhost:1025
        // If MailHog is running, it should succeed
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _emailService.SendConfirmationEmailAsync(testEmail, testLink);
        });

        // Assert
        // Should not throw an exception if MailHog is running
        Assert.Null(exception);
    }

    [Fact]
    public async Task SendConfirmationEmailAsync_WithEmptyEmail_ShouldThrow()
    {
        // Arrange
        var testLink = "https://example.com/confirm?token=123";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _emailService.SendConfirmationEmailAsync("", testLink);
        });
    }

    [Fact]
    public async Task SendConfirmationEmailAsync_WithEmptyLink_ShouldThrow()
    {
        // Arrange
        var testEmail = "test@example.com";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _emailService.SendConfirmationEmailAsync(testEmail, "");
        });
    }

    [Fact]
    public async Task SendConfirmationEmailAsync_WithNullEmail_ShouldThrow()
    {
        // Arrange
        var testLink = "https://example.com/confirm?token=123";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _emailService.SendConfirmationEmailAsync(null!, testLink);
        });
    }

    [Fact]
    public async Task SendConfirmationEmailAsync_WithNullLink_ShouldThrow()
    {
        // Arrange
        var testEmail = "test@example.com";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _emailService.SendConfirmationEmailAsync(testEmail, null!);
        });
    }
}
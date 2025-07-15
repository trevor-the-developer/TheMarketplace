using Marketplace.Api.Endpoints.Document;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Test.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Documents.UnitTests;

public class DocumentHandlerTests : IDisposable
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly MarketplaceDbContext _dbContext;
    private readonly DocumentHandler _handler;
    private readonly Mock<ILogger<DocumentHandler>> _loggerMock;
    private readonly MockValidationService _validationService;

    public DocumentHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DocumentHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _handler = new DocumentHandler();

        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new MarketplaceDbContext(options);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public void CreateDocumentHandler_Success()
    {
        // Arrange

        // Act
        var handler = new DocumentHandler();

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task CreateDocument_WithValidData_ReturnsDocument()
    {
        // Arrange
        var createCommand = new DocumentCreate
        {
            Title = "Test Document",
            Description = "Test Description",
            Text = "Test content",
            DocumentType = "PDF",
            ProductDetailId = 1
        };

        // Act
        var response = await _handler.Handle(createCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.Document);
        Assert.Equal("Test Document", response.Document.Title);
        Assert.Equal("Test Description", response.Document.Description);
        Assert.Equal("PDF", response.Document.DocumentType);
    }

    [Fact]
    public async Task UpdateDocument_WithValidData_ReturnsUpdatedDocument()
    {
        // Arrange
        var updateCommand = new DocumentUpdate
        {
            Id = 1,
            Title = "Updated Document",
            Description = "Updated Description",
            Text = "Updated content",
            DocumentType = "Word",
            ProductDetailId = 1
        };

        // Act
        var existingDocument = new Document
        {
            Id = 1,
            Title = "Original Document",
            Description = "Original Description",
            Text = "Original content",
            DocumentType = "PDF",
            ProductDetailId = 1,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Documents.Add(existingDocument);
        await _dbContext.SaveChangesAsync();

        var response = await _handler.Handle(updateCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.Document);
        Assert.Equal("Updated Document", response.Document.Title);
        Assert.Equal("Updated Description", response.Document.Description);
        Assert.Equal("Word", response.Document.DocumentType);
    }

    [Fact]
    public async Task DeleteDocument_WithValidId_DeletesDocument()
    {
        // Arrange
        var deleteCommand = new DocumentDelete { Id = 1 };

        // Act
        var existingDocument = new Document
        {
            Id = 1,
            Title = "Document to Delete",
            Description = "Description",
            Text = "Content",
            DocumentType = "PDF",
            ProductDetailId = 1,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Documents.Add(existingDocument);
        await _dbContext.SaveChangesAsync();

        await _handler.Handle(deleteCommand, _dbContext);

        // Assert
        var document = await _dbContext.Documents.FindAsync(deleteCommand.Id);
        Assert.Null(document);
    }
}
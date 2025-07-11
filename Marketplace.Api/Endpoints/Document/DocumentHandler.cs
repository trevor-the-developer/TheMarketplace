using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Marketplace.Core.Services;

namespace Marketplace.Api.Endpoints.Document;

[WolverineHandler]
public class DocumentHandler
{
    [Transactional]
    public async Task<DocumentResponse> Handle(DocumentRequest command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        Data.Entities.Document? document = null;
        
        if (command.DocumentId > 0)
        {
            document = await dbContext.Documents
                .FirstOrDefaultAsync(d => d.Id == command.DocumentId);
        }

        if (command.AllDocuments)
        {
            var documentsQuery = dbContext.Documents.AsQueryable();
            
            if (command.ProductDetailId.HasValue)
            {
                documentsQuery = documentsQuery.Where(d => d.ProductDetailId == command.ProductDetailId);
            }

            if (!string.IsNullOrEmpty(command.DocumentType))
            {
                documentsQuery = documentsQuery.Where(d => d.DocumentType == command.DocumentType);
            }
            
            var documents = await documentsQuery.ToListAsync();
            return new DocumentResponse() { Documents = documents };
        }

        if (command.ProductDetailId.HasValue && document == null)
        {
            document = await dbContext.Documents
                .FirstOrDefaultAsync(d => d.ProductDetailId == command.ProductDetailId);
        }

        document ??= await dbContext.Documents.FirstOrDefaultAsync();
        return new DocumentResponse()
        {
            Document = document
        };
    }

    [Transactional]
    public async Task<DocumentResponse> Handle(DocumentCreate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        var currentUser = currentUserService.GetCurrentUserName();
        var document = new Data.Entities.Document
        {
            Title = command.Title,
            Description = command.Description,
            Text = command.Text,
            Bytes = command.Bytes,
            DocumentType = command.DocumentType,
            ProductDetailId = command.ProductDetailId,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = currentUser,
            ModifiedDate = DateTime.UtcNow
        };

        dbContext.Documents.Add(document);
        await dbContext.SaveChangesAsync();

        return new DocumentResponse { Document = document };
    }

    [Transactional]
    public async Task<DocumentResponse> Handle(DocumentUpdate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        var document = await dbContext.Documents.FindAsync(command.Id);
        if (document == null)
        {
            return new DocumentResponse { Document = null };
        }

        document.Title = command.Title;
        document.Description = command.Description;
        document.Text = command.Text;
        document.Bytes = command.Bytes;
        document.DocumentType = command.DocumentType;
        document.ProductDetailId = command.ProductDetailId;
        document.ModifiedBy = currentUserService.GetCurrentUserName();
        document.ModifiedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return new DocumentResponse { Document = document };
    }

    [Transactional]
    public async Task Handle(DocumentDelete command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        var document = await dbContext.Documents.FindAsync(command.Id);
        if (document != null)
        {
            dbContext.Documents.Remove(document);
            await dbContext.SaveChangesAsync();
        }
    }
}

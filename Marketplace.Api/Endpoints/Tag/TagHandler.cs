using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Marketplace.Core.Services;

namespace Marketplace.Api.Endpoints.Tag;

[WolverineHandler]
public class TagHandler
{
    [Transactional]
    public async Task<TagResponse> Handle(TagRequest command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        Data.Entities.Tag? tag = null;
        
        if (command.TagId > 0)
        {
            tag = await dbContext.Tags
                .FirstOrDefaultAsync(t => t.Id == command.TagId);
        }

        if (command.AllTags)
        {
            var tagsQuery = dbContext.Tags.AsQueryable();
            
            if (!string.IsNullOrEmpty(command.Name))
            {
                tagsQuery = tagsQuery.Where(t => t.Name!.Contains(command.Name));
            }

            if (command.IsEnabled.HasValue)
            {
                tagsQuery = tagsQuery.Where(t => t.IsEnabled == command.IsEnabled);
            }
            
            var tags = await tagsQuery.ToListAsync();
            return new TagResponse() { Tags = tags };
        }

        if (!string.IsNullOrEmpty(command.Name) && tag == null)
        {
            tag = await dbContext.Tags
                .FirstOrDefaultAsync(t => t.Name == command.Name);
        }

        tag ??= await dbContext.Tags.FirstOrDefaultAsync();
        return new TagResponse()
        {
            Tag = tag
        };
    }

    [Transactional]
    public async Task<TagResponse> Handle(TagCreate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        var currentUser = currentUserService.GetCurrentUserName();
        var tag = new Data.Entities.Tag
        {
            Name = command.Name,
            Description = command.Description,
            IsEnabled = command.IsEnabled,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = currentUser,
            ModifiedDate = DateTime.UtcNow
        };

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        return new TagResponse { Tag = tag };
    }

    [Transactional]
    public async Task<TagResponse> Handle(TagUpdate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        var tag = await dbContext.Tags.FindAsync(command.Id);
        if (tag == null)
        {
            return new TagResponse { Tag = null };
        }

        tag.Name = command.Name;
        tag.Description = command.Description;
        tag.IsEnabled = command.IsEnabled;
        tag.ModifiedBy = currentUserService.GetCurrentUserName();
        tag.ModifiedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return new TagResponse { Tag = tag };
    }

    [Transactional]
    public async Task Handle(TagDelete command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        var tag = await dbContext.Tags.FindAsync(command.Id);
        if (tag != null)
        {
            dbContext.Tags.Remove(tag);
            await dbContext.SaveChangesAsync();
        }
    }
}

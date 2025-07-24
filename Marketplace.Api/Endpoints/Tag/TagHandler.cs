using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Core;
using Marketplace.Core.Interfaces;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Tag;
using Marketplace.Core.Validation;
using Marketplace.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wolverine.Attributes;

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
            tag = await dbContext.Tags
                .FirstOrDefaultAsync(t => t.Id == command.TagId);

        if (command.AllTags)
        {
            var tagsQuery = dbContext.Tags.AsQueryable();

            if (!string.IsNullOrEmpty(command.Name)) tagsQuery = tagsQuery.Where(t => t.Name!.Contains(command.Name));

            if (command.IsEnabled.HasValue) tagsQuery = tagsQuery.Where(t => t.IsEnabled == command.IsEnabled);

            var tags = await tagsQuery.ToListAsync();
            return new TagResponse { Tags = tags };
        }

        if (!string.IsNullOrEmpty(command.Name) && tag == null)
            tag = await dbContext.Tags
                .FirstOrDefaultAsync(t => t.Name == command.Name);

        tag ??= await dbContext.Tags.FirstOrDefaultAsync();
        return new TagResponse
        {
            Tag = tag
        };
    }

    [Transactional]
    public async Task<TagResponse> Handle(TagCreate command, MarketplaceDbContext dbContext,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new TagResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

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
    public async Task<TagResponse> Handle(TagUpdate command, MarketplaceDbContext dbContext,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new TagResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

        var tag = await dbContext.Tags.FindAsync(command.Id);
        if (tag == null) return new TagResponse { Tag = null };

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
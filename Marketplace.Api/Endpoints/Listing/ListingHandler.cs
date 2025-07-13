using Marketplace.Data.Repositories;
using Wolverine.Attributes;
using Marketplace.Core.Services;
using Marketplace.Core.Validation;
using Marketplace.Core;
using Newtonsoft.Json;

namespace Marketplace.Api.Endpoints.Listing;

[WolverineHandler]
public class ListingHandler
{
    [Transactional]
    public async Task<ListingResponse> Handle(ListingRequest command, IListingRepository listingRepository)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(listingRepository, nameof(listingRepository));

        Data.Entities.Listing? listing = null;
        if (command.ListingId > 0)
        {
            listing = await listingRepository.GetByIdAsync(command.ListingId);
        }
        
        if (command.AllListings)
        {
            var listings = await listingRepository.GetAllAsync();
            return new ListingResponse() { Listings = listings.ToList() };
        }
        
        listing ??= await listingRepository.GetFirstOrDefaultAsync(l => true);
        return new ListingResponse()
        {
            Listing = listing
        };
    }

    [Transactional]
    public async Task<ListingResponse> Handle(ListingCreate command, IListingRepository listingRepository, ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(listingRepository, nameof(listingRepository));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
        {
            return new ListingResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                    StatusCode: StatusCodes.Status400BadRequest,
                    ErrorMessage: "Validation failed",
                    StackTrace: JsonConvert.SerializeObject(validationErrors)
                )
            };
        }

        var currentUser = currentUserService.GetCurrentUserName();
        var listing = new Data.Entities.Listing
        {
            Title = command.Title,
            Description = command.Description,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = currentUser,
            ModifiedDate = DateTime.UtcNow
        };

        await listingRepository.AddAsync(listing);
        await listingRepository.SaveChangesAsync();

        return new ListingResponse { Listing = listing };
    }

    [Transactional]
    public async Task<ListingResponse> Handle(ListingUpdate command, IListingRepository listingRepository, ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(listingRepository, nameof(listingRepository));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
        {
            return new ListingResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                    StatusCode: StatusCodes.Status400BadRequest,
                    ErrorMessage: "Validation failed",
                    StackTrace: JsonConvert.SerializeObject(validationErrors)
                )
            };
        }

        var listing = await listingRepository.GetByIdAsync(command.Id);
        if (listing == null)
        {
            return new ListingResponse { Listing = null };
        }

        listing.Title = command.Title;
        listing.Description = command.Description;
        listing.ModifiedBy = currentUserService.GetCurrentUserName();
        listing.ModifiedDate = DateTime.UtcNow;

        await listingRepository.UpdateAsync(listing);
        await listingRepository.SaveChangesAsync();

        return new ListingResponse { Listing = listing };
    }

    [Transactional]
    public async Task Handle(ListingDelete command, IListingRepository listingRepository)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(listingRepository, nameof(listingRepository));

        var listing = await listingRepository.GetByIdAsync(command.Id);
        if (listing != null)
        {
            await listingRepository.DeleteAsync(listing);
            await listingRepository.SaveChangesAsync();
        }
    }
}

using Marketplace.Core;

namespace Marketplace.Api.Endpoints.Listing;
using Data.Entities;

public class ListingResponse
{
    public Listing? Listing { get; set; }
    public IEnumerable<Listing>? Listings { get; set; }
    public ApiError? ApiError { get; set; }
}

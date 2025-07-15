using Marketplace.Core;

namespace Marketplace.Api.Endpoints.Listing;

public class ListingResponse
{
    public Data.Entities.Listing? Listing { get; set; }
    public IEnumerable<Data.Entities.Listing>? Listings { get; set; }
    public ApiError? ApiError { get; set; }
}
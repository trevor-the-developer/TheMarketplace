using Marketplace.Core;

namespace Marketplace.Api.Endpoints.Media;

public class MediaResponse
{
    public Data.Entities.Media? Media { get; set; }
    public List<Data.Entities.Media>? MediaList { get; set; }
    public ApiError? ApiError { get; set; }
}
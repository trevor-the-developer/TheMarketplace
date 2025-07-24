using System.Collections.Generic;

namespace Marketplace.Core.Models.Tag;

public class TagResponse
{
    public Data.Entities.Tag? Tag { get; set; }
    public List<Data.Entities.Tag>? Tags { get; set; }
    public ApiError? ApiError { get; set; }
}
namespace Marketplace.Test.Helpers;

public static class StringHelper
{
    public static string GuidToFlattenedString()
    {
        return Guid.NewGuid().ToString().Replace("-", "");
    }
}
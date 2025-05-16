using Alba;
using JasperFx.Core;
using Microsoft.AspNetCore.Http;

namespace Marketplace.Test.Helpers;

public static class JsonHelper
{
    public static async Task<string> ReadAsJsonAsync(IScenarioResult response)
    {
        string json;
        var context = new DefaultHttpContext();
        var buffer = new MemoryStream();
        await response.Context.Response.Body.CopyToAsync(buffer);
        buffer.Position = 0;

        var copy = new MemoryStream();
        await buffer.CopyToAsync(copy);
        buffer.Position = 0;

        try
        {
            context.Request.Body = buffer; // Need to trick the MVC conneg services
            if (buffer.Length == 0) throw new EmptyResponseException();
            copy.Position = 0;
            json = await copy.ReadAllTextAsync();
        }
        finally
        {
            // This is to enable repeated reads
            copy.Position = 0;
            response.Context.Response.Body = copy;
        }
        
        return json;
    }
}
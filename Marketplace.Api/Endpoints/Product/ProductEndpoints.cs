using Wolverine;

namespace Marketplace.Api.Endpoints.Product;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/product/create", async (ProductCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<ProductResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Product")
        .WithName("Create Product")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPut("/api/product/update/{id}", async (int id, ProductUpdate command, IMessageBus bus) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<ProductResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Product")
        .WithName("Update Product")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapDelete("/api/product/delete/{id}", async (int id, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new ProductDelete { Id = id });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Product")
        .WithName("Delete Product")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost("/api/get/product/", async (ProductRequest command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<ProductResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Product")
        .WithName("Get Product(s)")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost("/api/get/product/{productId}", async (int productId, ProductRequest command, IMessageBus bus) =>
        {
            command.ProductId = productId;
            var response = await bus.InvokeAsync<ProductResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Product")
        .WithName("Get Product by Id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost("/api/get/product/all/", async (ProductRequest command, IMessageBus bus) =>
        {
            command.AllProducts = true;
            var response = await bus.InvokeAsync<ProductResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Product")
        .WithName("Get all products")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}

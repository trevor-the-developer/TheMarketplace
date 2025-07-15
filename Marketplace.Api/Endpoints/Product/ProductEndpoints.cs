using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Product;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        // POST /api/products - Create new product
        routes.MapPost(ApiConstants.ApiProducts, async (ProductCreate command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<ProductResponse>(command);
                return Results.Created($"/api/products/{response.Product?.Id}", response);
            })
            .RequireAuthorization()
            .WithTags("Products")
            .WithName("Create Product")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/products/{id} - Update existing product
        routes.MapPut(ApiConstants.ApiProductsById, async (int id, ProductUpdate command, IMessageBus bus) =>
            {
                if (id != command.Id) return Results.BadRequest();

                var response = await bus.InvokeAsync<ProductResponse>(command);
                return response.Product == null ? Results.NotFound() : Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Products")
            .WithName("Update Product")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/products/{id} - Delete product
        routes.MapDelete(ApiConstants.ApiProductsById, async (int id, IMessageBus bus) =>
            {
                await bus.InvokeAsync(new ProductDelete { Id = id });
                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("Products")
            .WithName("Delete Product")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/products - Get all products
        routes.MapGet(ApiConstants.ApiProducts, async (IMessageBus bus) =>
            {
                var command = new ProductRequest { AllProducts = true };
                var response = await bus.InvokeAsync<ProductResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Products")
            .WithName("Get All Products")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/products/{id} - Get product by ID
        routes.MapGet(ApiConstants.ApiProductsById, async (int id, IMessageBus bus) =>
            {
                var command = new ProductRequest { ProductId = id };
                var response = await bus.InvokeAsync<ProductResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Products")
            .WithName("Get Product by Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
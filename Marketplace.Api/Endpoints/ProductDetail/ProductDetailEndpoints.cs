using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.ProductDetail;

public static class ProductDetailEndpoints
{
    public static void MapProductDetailEndpoints(this IEndpointRouteBuilder routes)
    {
        // POST /api/product-details - Create new product detail
        routes.MapPost(ApiConstants.ApiProductDetails, async (ProductDetailCreate command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<ProductDetailResponse>(command);
                return Results.Created($"/api/product-details/{response.ProductDetail?.Id}", response);
            })
            .RequireAuthorization()
            .WithTags("ProductDetails")
            .WithName("Create ProductDetail")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/product-details/{id} - Update existing product detail
        routes.MapPut(ApiConstants.ApiProductDetailsById,
                async (int id, ProductDetailUpdate command, IMessageBus bus) =>
                {
                    if (id != command.Id) return Results.BadRequest();

                    var response = await bus.InvokeAsync<ProductDetailResponse>(command);
                    return response.ProductDetail == null ? Results.NotFound() : Results.Ok(response);
                })
            .RequireAuthorization()
            .WithTags("ProductDetails")
            .WithName("Update ProductDetail")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/product-details/{id} - Delete product detail
        routes.MapDelete(ApiConstants.ApiProductDetailsById, async (int id, IMessageBus bus) =>
            {
                await bus.InvokeAsync(new ProductDetailDelete { Id = id });
                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("ProductDetails")
            .WithName("Delete ProductDetail")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/product-details - Get all product details
        routes.MapGet(ApiConstants.ApiProductDetails, async (IMessageBus bus) =>
            {
                var command = new ProductDetailRequest { AllProductDetails = true };
                var response = await bus.InvokeAsync<ProductDetailResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("ProductDetails")
            .WithName("Get All ProductDetails")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/product-details/{id} - Get product detail by ID
        routes.MapGet(ApiConstants.ApiProductDetailsById, async (int id, IMessageBus bus) =>
            {
                var command = new ProductDetailRequest { ProductDetailId = id };
                var response = await bus.InvokeAsync<ProductDetailResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("ProductDetails")
            .WithName("Get ProductDetail by Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
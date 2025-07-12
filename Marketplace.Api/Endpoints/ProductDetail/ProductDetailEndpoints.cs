using Wolverine;

namespace Marketplace.Api.Endpoints.ProductDetail;

public static class ProductDetailEndpoints
{
    public static void MapProductDetailEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/productdetail/create", async (ProductDetailCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<ProductDetailResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("ProductDetail")
        .WithName("Create ProductDetail")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPut("/api/productdetail/update/{id}", async (int id, ProductDetailUpdate command, IMessageBus bus) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<ProductDetailResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("ProductDetail")
        .WithName("Update ProductDetail")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapDelete("/api/productdetail/delete/{id}", async (int id, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new ProductDetailDelete { Id = id });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("ProductDetail")
        .WithName("Delete ProductDetail")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost("/api/get/productdetail/", async (ProductDetailRequest command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<ProductDetailResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("ProductDetail")
        .WithName("Get ProductDetail(s)")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost("/api/get/productdetail/{productDetailId}", async (int productDetailId, ProductDetailRequest command, IMessageBus bus) =>
        {
            command.ProductDetailId = productDetailId;
            var response = await bus.InvokeAsync<ProductDetailResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("ProductDetail")
        .WithName("Get ProductDetail by Id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost("/api/get/productdetail/all/", async (ProductDetailRequest command, IMessageBus bus) =>
        {
            command.AllProductDetails = true;
            var response = await bus.InvokeAsync<ProductDetailResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("ProductDetail")
        .WithName("Get all product details")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}

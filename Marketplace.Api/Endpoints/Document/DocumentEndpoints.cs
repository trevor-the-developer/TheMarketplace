using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Document;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(ApiConstants.ApiSlashDocumentCreate, async (DocumentCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<DocumentResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Document")
        .WithName("Create Document")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPut(ApiConstants.ApiSlashDocumentUpdate, async (int id, DocumentUpdate command, IMessageBus bus) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<DocumentResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Document")
        .WithName("Update Document")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapDelete(ApiConstants.ApiSlashDocumentDelete, async (int id, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new DocumentDelete { Id = id });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Document")
        .WithName("Delete Document")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetDocument, async (DocumentRequest command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<DocumentResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Document")
        .WithName("Get Document(s)")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetDocumentById, async (int documentId, DocumentRequest command, IMessageBus bus) =>
        {
            command.DocumentId = documentId;
            var response = await bus.InvokeAsync<DocumentResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Document")
        .WithName("Get Document by Id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetAllDocuments, async (DocumentRequest command, IMessageBus bus) =>
        {
            command.AllDocuments = true;
            var response = await bus.InvokeAsync<DocumentResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Document")
        .WithName("Get all documents")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}

using Marketplace.Core.Constants;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Document;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Marketplace.Api.Endpoints.Document;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this IEndpointRouteBuilder routes)
    {
        // POST /api/documents - Create new document
        routes.MapPost(ApiConstants.ApiDocuments, async (DocumentCreate command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<DocumentResponse>(command);
                return Results.Created($"/api/documents/{response.Document?.Id}", response);
            })
            .RequireAuthorization()
            .WithTags("Documents")
            .WithName("Create Document")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/documents/{id} - Update existing document
        routes.MapPut(ApiConstants.ApiDocumentsById, async (int id, DocumentUpdate command, IMessageBus bus) =>
            {
                if (id != command.Id) return Results.BadRequest();

                var response = await bus.InvokeAsync<DocumentResponse>(command);
                return response.Document == null ? Results.NotFound() : Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Documents")
            .WithName("Update Document")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/documents/{id} - Delete document
        routes.MapDelete(ApiConstants.ApiDocumentsById, async (int id, IMessageBus bus) =>
            {
                await bus.InvokeAsync(new DocumentDelete { Id = id });
                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("Documents")
            .WithName("Delete Document")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/documents - Get all documents
        routes.MapGet(ApiConstants.ApiDocuments, async (IMessageBus bus) =>
            {
                var command = new DocumentRequest { AllDocuments = true };
                var response = await bus.InvokeAsync<DocumentResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Documents")
            .WithName("Get All Documents")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/documents/{id} - Get document by ID
        routes.MapGet(ApiConstants.ApiDocumentsById, async (int id, IMessageBus bus) =>
            {
                var command = new DocumentRequest { DocumentId = id };
                var response = await bus.InvokeAsync<DocumentResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Documents")
            .WithName("Get Document by Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
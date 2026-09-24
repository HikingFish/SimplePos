using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Categories.Commands.ActivateCategory;
using SimplePos.Application.Categories.Commands.CreateCategory;
using SimplePos.Application.Categories.Commands.DeactivateCategory;
using SimplePos.Application.Categories.Commands.DeleteCategory;
using SimplePos.Application.Categories.Commands.UpdateCategory;
using SimplePos.Application.Categories.Common;
using SimplePos.Application.Categories.Queries.GetCategoriesByCompany;
using SimplePos.Application.Categories.Queries.GetCategoryById;
using SimplePos.Domain.Common.ResultPattern;
using System.Security.Claims;

namespace SimplePos.WebApi.EndPoints;

public static class CategoryEndpoints
{
    public record CreateCategoryRequest(string Name);
    public record UpdateCategoryRequest(string Name);

    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        // 1. GET /api/categories - Get all categories for company
        group.MapGet("/", async (
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var query = new GetCategoriesByCompanyQuery(companyId);
            var result = await dispatcher.QueryAsync<Result<List<CategoryResponse>>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<List<CategoryResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 2. GET /api/categories/{id:guid} - Get category by ID
        group.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var query = new GetCategoryByIdQuery(id, companyId);
            var result = await dispatcher.QueryAsync<Result<CategoryResponse>>(query, ct);

            if (result.IsSuccess)
                return Results.Ok(result.Data);

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces<CategoryResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 3. POST /api/categories - Create category
        group.MapPost("/", async (
            CreateCategoryRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new CreateCategoryCommand(companyId, request.Name);
            var result = await dispatcher.SendAsync<CreateCategoryCommand, Result<Guid>>(command, ct);

            if (result.IsSuccess)
            {
                return Results.Created($"/api/categories/{result.Data}", new { id = result.Data });
            }

            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        // 4. PUT /api/categories/{id:guid} - Update category name
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateCategoryRequest request,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new UpdateCategoryCommand(id, companyId, request.Name);
            var result = await dispatcher.SendAsync<UpdateCategoryCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Validation)
                return Results.BadRequest(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // 5. PATCH /api/categories/{id:guid}/activate - Activate category
        group.MapPatch("/{id:guid}/activate", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new ActivateCategoryCommand(id, companyId);
            var result = await dispatcher.SendAsync<ActivateCategoryCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 6. PATCH /api/categories/{id:guid}/deactivate - Deactivate category
        group.MapPatch("/{id:guid}/deactivate", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new DeactivateCategoryCommand(id, companyId);
            var result = await dispatcher.SendAsync<DeactivateCategoryCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();

        // 7. DELETE /api/categories/{id:guid} - Soft delete category
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ICqrsDispatcher dispatcher,
            CancellationToken ct) =>
        {
            string? companyIdClaim = user.FindFirst("CompanyId")?.Value;
            if (!Guid.TryParse(companyIdClaim, out var companyId))
            {
                return Results.Unauthorized();
            }

            var command = new DeleteCategoryCommand(id, companyId);
            var result = await dispatcher.SendAsync<DeleteCategoryCommand, Result>(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();

            if (result.Error.Type == ErrorType.NotFound)
                return Results.NotFound(new { detail = result.Error.Description });
            if (result.Error.Type == ErrorType.Conflict)
                return Results.Conflict(new { detail = result.Error.Description });

            return Results.Problem(detail: result.Error.Description, statusCode: 500);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization();
    }
}
